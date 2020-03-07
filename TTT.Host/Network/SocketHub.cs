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

namespace TTT.Core
{
    public class SocketHub
    {
        const string stdResponseGUID = "258EAFA5-E914-47DA-95CA-C5AB0DC85B11";
        const int PORT_NO = 69;
        Logger _logger;
        public SocketHub(Logger logger)
        {
            _logger = logger;
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
            _activeSockets[id] = new GameSocket();
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
                    handleIncomingMessage(bytes);
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

        private void handleIncomingMessage(byte[] bytes)
        {
            bool fin = (bytes[0] & 0b10000000) != 0,
                    mask = (bytes[1] & 0b10000000) != 0; // must be true, "All messages from the client to the server have this bit set"

            int opcode = bytes[0] & 0b00001111, // expecting 1 - text message
                msglen = bytes[1] - 128, // & 0111 1111
                offset = 2;

            if (msglen == 126)
            {
                // was ToUInt16(bytes, offset) but the result is incorrect
                msglen = BitConverter.ToUInt16(new byte[] { bytes[3], bytes[2] }, 0);
                offset = 4;
            }
            else if (msglen == 127)
            {
                _logger.Log("TODO: msglen == 127, needs qword to store msglen");
                // i don't really know the byte order, please edit this
                // msglen = BitConverter.ToUInt64(new byte[] { bytes[5], bytes[4], bytes[3], bytes[2], bytes[9], bytes[8], bytes[7], bytes[6] }, 0);
                // offset = 10;
            }

            if (msglen == 0)
                _logger.Log("msglen == 0");
            else if (mask)
            {
                string messageReceived = decodeMessage(bytes, offset, msglen);
                _logger.Log($"Received message: {messageReceived}");
                //Broadcast(messageReceived);
            }
            else
                _logger.Log("mask bit not set");
        }

        private string decodeMessage(byte[] bytes, int offset, int msglen)
        {
            byte[] decoded = new byte[msglen];
            byte[] masks = new byte[4] { bytes[offset], bytes[offset + 1], bytes[offset + 2], bytes[offset + 3] };
            offset += 4;

            for (int i = 0; i < msglen; ++i)
                decoded[i] = (byte)(bytes[offset + i] ^ masks[i % 4]);

            return Encoding.UTF8.GetString(decoded);
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
