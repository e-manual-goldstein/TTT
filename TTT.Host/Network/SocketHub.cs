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

namespace TTT.Core
{
    public class SocketHub
    {
        const string stdResponseGUID = "258EAFA5-E914-47DA-95CA-C5AB0DC85B11";
        const int PORT_NO = 69;
        Logger _logger;
        MessageHandler _messageHandler;
        GameController _gameController;

        public SocketHub(Logger logger, MessageHandler messageHandler, GameController gameController)
        {
            _logger = logger;
            _messageHandler = messageHandler;
            _gameController = gameController;
            //_broadcaster.BeginBroadcast((address, id) => RequestSocketConnection(address, id));

        }
        
        Dictionary<string, TcpListener> _serverDictionary = new Dictionary<string, TcpListener>();
        

        public TcpListener GetServer(IPAddress ipAddress)
        {
            if (ipAddress != null)
            {
                return GetServer(ipAddress.ToString());
            }
            return null;
        }

        public TcpListener GetServer(string ipAddress)
        {
            if (_serverDictionary.ContainsKey(ipAddress))
            {
                return _serverDictionary[ipAddress];
            }
            _logger.Log($"Creating new server on {ipAddress}:{PORT_NO}");
            return _serverDictionary[ipAddress] = new TcpListener(IPAddress.Parse(ipAddress), PORT_NO);
        }

        IDictionary<Guid, GameSocket> _activeSockets = new Dictionary<Guid, GameSocket>();

        public Guid RequestSocketConnection(IPAddress ipAddress, Guid id)
        {
            _logger.Log($"Requesting socket connection");
            var server = GetServer(ipAddress);
            _logger.Log($"Starting Server: {server.LocalEndpoint}");
            server.Start();
            if (_activeSockets.ContainsKey(id))
            {
                _logger.Log("Found connection for same Id, disposing old connection");
                _activeSockets[id].Kill();
            }
            _activeSockets[id] = new GameSocket(_logger,_messageHandler,_gameController);
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
