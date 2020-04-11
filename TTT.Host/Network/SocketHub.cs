using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Security.Cryptography;
using System.IO;
using TTT.Host;
using TTT.Common;
using TTT.Host.Control;
using System.Threading;

namespace TTT.Core
{
    public class SocketHub
    {
        const string stdResponseGUID = "258EAFA5-E914-47DA-95CA-C5AB0DC85B11";
        const int PORT_NO = 69;
        Logger _logger;
        MessageHandler _messageHandler;
        GameController _gameController;
        IPAddress tcpListenerAddresss;
        TcpListener _server;
        IDictionary<Guid, GameSocket> _activeSockets = new Dictionary<Guid, GameSocket>();

        public SocketHub(Logger logger, MessageHandler messageHandler, GameController gameController)
        {
            _logger = logger;
            _messageHandler = messageHandler;
            _gameController = gameController;
            StartServer();
        }

        #region UDP Echo
        private void StartServer()
        {
            using (var listener = new UdpClient(Constants.SERVER_LISTEN_PORT))
            {
                var targetEndPoint = new IPEndPoint(IPAddress.Any, 0);
                //await message
                Task.Run(() => 
                {
                    var message = UdpMessage.FromByteArray(listener.Receive(ref targetEndPoint));
                    _logger.Log(message.Payload + $"{targetEndPoint.Address}:{PORT_NO}");
                    tcpListenerAddresss = targetEndPoint.Address;
                    _server = new TcpListener(targetEndPoint.Address, PORT_NO);
                    _server.Start();
                });
                using (var speaker = new UdpClient())
                {
                    speaker.Send(new UdpMessage("Starting server on: "), new IPEndPoint(IPAddress.Broadcast, Constants.SERVER_LISTEN_PORT));
                }
            }
        }

        #endregion

        #region Connect to Clients

        public async Task<TcpClient> ConnectAsync()
        {
            _logger.Log($"Accepting Clients");
            var socketId = Guid.NewGuid();
            var client = _server.AcceptTcpClientAsync().ContinueWith(task => 
            {
                _activeSockets[socketId] = new GameSocket(_logger, _messageHandler, _gameController);
                _activeSockets[socketId].Client = task.Result;
                _logger.Log($"Socket Connected");
                return _activeSockets[socketId].Client;
            });
            if (!BroadcastInvitation(socketId))
                return null;
            return await client;
        }

        private bool BroadcastInvitation(Guid socketId)
        {
            using (var broadcaster = new UdpClient() { EnableBroadcast = true })
            {
                for (int i = 0; i < 10; i++)
                {
                    if (_activeSockets.ContainsKey(socketId))
                        return true;
                    _logger.Log($"Pinging client... [{i}]");
                    broadcaster.Send(new UdpMessage(tcpListenerAddresss.ToString()), new IPEndPoint(IPAddress.Broadcast, Constants.SERVER_LISTEN_PORT));
                    Thread.Sleep(1000);
                }
                return false;
            }
        }

        #endregion

        public Guid RequestSocketConnection(Guid id)
        {
            _logger.Log($"Requesting socket connection");
            var server = _server;
            _logger.Log($"Starting Server: {server.LocalEndpoint}");
            server.Start();
            if (_activeSockets.ContainsKey(id))
            {
                _logger.Log("Found connection for same Id, disposing old connection");
                _activeSockets[id].Kill();
            }
            _activeSockets[id] = new GameSocket(_logger, _messageHandler, _gameController);
            _logger.Log($"Opening Socket for {id}");
            _activeSockets[id].Client = server.AcceptTcpClient();
            _logger.Log($"Socket Connected");
            return id;
        }

        public async Task OpenConnectionAsync(Guid userId)
        {
            await Task.Run(() => OpenConnection(userId));
        }

        public void OpenConnection(Guid id)
        {
            var socket = _activeSockets[id];
            var tcpClient = socket.Client;
            _logger.Log("Handshake started");
            while (true)
            {
                while (socket.IsActive() && tcpClient.Available < 3)
                {
                    // wait for enough bytes to be available
                }
                if (!socket.KeepAlive)
                    break;
                var socketStream = tcpClient.GetStream();
                Byte[] bytes = new Byte[tcpClient.Available];

                socketStream.Read(bytes, 0, bytes.Length);

                //translate bytes of request to string
                var data = Encoding.UTF8.GetString(bytes);

                if (Regex.IsMatch(data, "^GET"))
                {
                    Byte[] response = createWebSocketResponse(data);
                    socketStream.Write(response, 0, response.Length);
                    socket.ActiveStream = socketStream;
                    //Output.Debug("Handshake complete");
                }
                else
                {
                    socket.Receive(bytes);
                }
            }
        }
        
        //public void Broadcast(string str)
        //{
        //    foreach (var entry in _activeSockets)
        //    {
        //        entry.Value.Send(str);
        //    }
        //}

        public void SendMessage(Guid userId, string message)
        {
            _activeSockets[userId].Send(message);
        }

        public void CloseUserSocket(Guid userId)
        {
            if (_activeSockets.ContainsKey(userId))
                _activeSockets[userId].Kill();
        }

        private byte[] createWebSocketResponse(string data)
        {
            return Encoding.UTF8.GetBytes(
                "HTTP/1.1 101 Switching Protocols" + Environment.NewLine +
                "Connection: Upgrade" + Environment.NewLine +
                "Upgrade: websocket" + Environment.NewLine +
                "Sec-WebSocket-Accept: " + Convert.ToBase64String(computeWebSocketHash(data))
                + Environment.NewLine
                + Environment.NewLine);

        }

        private byte[] computeWebSocketHash(string data)
        {
            var newSHA = SHA1.Create();
            var webSocketPattern = "Sec-WebSocket-Key: (.*)";
            var webSocketKey = new Regex(webSocketPattern).Match(data).Groups[1].Value.Trim()
                + stdResponseGUID;
            return newSHA.ComputeHash(Encoding.UTF8.GetBytes(webSocketKey));
        }
    }
}
