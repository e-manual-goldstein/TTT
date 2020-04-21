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
        ControllerManager _controllerManager;
        IPAddress tcpListenerAddresss;
        TcpListener _server;
        IDictionary<Guid, GameSocket> _activeSockets = new Dictionary<Guid, GameSocket>();

        public SocketHub(Logger logger, MessageHandler messageHandler, ControllerManager controllerManager)
        {
            _logger = logger;
            _messageHandler = messageHandler;
            _controllerManager = controllerManager;
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

        public async Task<Guid> ConnectAsync(bool useThreading = false)
        {
            _logger.Log($"Accepting Clients");
            var source = new CancellationTokenSource();
            var socketId = Guid.NewGuid();
            var clientId = BeginListening(socketId, source);
            if (useThreading) 
                BroadcastInvitationOnThread(socketId, source.Token);
            else
                BroadcastInvitationAsTask(socketId, source.Token);
            return await clientId;
        }

        public async Task<Guid> BeginListening(Guid socketId, CancellationTokenSource source)
        {
            await _server.AcceptTcpClientAsync().ContinueWith(async task => 
            {
                _logger.Log($"Connecting Socket");
                _activeSockets[socketId] = new GameSocket(_logger, _messageHandler, _controllerManager);
                _activeSockets[socketId].Client = task.Result;
                _logger.Log("Cancelling Token");
                source.Cancel();
                await OpenConnectionAsync(socketId);
                return;
            });
            return socketId;
        }

        private void BroadcastInvitationOnThread(Guid socketId, CancellationToken cancellationToken)
        {
            var pinger = new Pinger() { SocketId = socketId, CancellationToken = cancellationToken};
            ThreadPool.QueueUserWorkItem((p) => SendPing(p), pinger, false);
        }

        private void BroadcastInvitationAsTask(Guid socketId, CancellationToken cancellationToken)
        {
            Task.Run(() =>
            {
                using (var broadcaster = new UdpClient() { EnableBroadcast = true })
                {
                    _logger.Log($"Pinging client...");
                    while (!cancellationToken.IsCancellationRequested)
                    {
                        //TEST THIS
                        broadcaster.Send(new UdpMessage(socketId.ToString()), new IPEndPoint(IPAddress.Broadcast, Constants.SERVER_LISTEN_PORT));
                        //Change this to Async? lol

                    }
                    _logger.Log("Stopped Pinging...");
                }
            }, cancellationToken);
        }

        public class Pinger
        {
            public Guid SocketId { get; set; }
            public CancellationToken CancellationToken { get; set; }
        }
        private void SendPing(Pinger input)
        {
            Pinger pinger = input;
            CancellationToken cancellationToken = pinger.CancellationToken;
            Guid socketId = pinger.SocketId;
            using (var broadcaster = new UdpClient() { EnableBroadcast = true })
            {
                _logger.Log($"Pinging client...");
                while (!cancellationToken.IsCancellationRequested)
                {
                    //TEST THIS
                    broadcaster.Send(new UdpMessage(socketId.ToString()), new IPEndPoint(IPAddress.Broadcast, Constants.SERVER_LISTEN_PORT));
                    //Change this to Async? lol

                }
                _logger.Log("Stopped Pinging...");
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
            _activeSockets[id] = new GameSocket(_logger, _messageHandler, _controllerManager);
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

        public void BroadcastCommand(GameCommand command)
        {
            foreach (var entry in _activeSockets)
            {
                entry.Value.Send(command);
            }
        }

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
