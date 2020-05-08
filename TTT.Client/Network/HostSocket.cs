using Java.Net;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using TTT.Common;
using WebSocket4Net;
using ErrorEventArgs = SuperSocket.ClientEngine.ErrorEventArgs;
namespace TTT.Client
{
    public class HostSocket : ISocket
    {
        IPAddress _serverAddress;
        WebSocket _webSocket;
        bool _isOpen;

        public HostSocket(IPAddress serverAddress, bool openSocket = false)
        {
            _serverAddress = serverAddress;
            if (openSocket)
                _webSocket = openWebSocket();
        }

        public bool IsOpen => _isOpen;
        
        public void Send(GameCommand command)
        {
            Send(JsonConvert.SerializeObject(command));
        }

        public void Send(string message)
        {
            _webSocket.Send(message);
        }

        private WebSocket openWebSocket()
        {
            var webSocketClient = new WebSocket($"ws://{_serverAddress}:58008/");
            webSocketClient.Opened += webSocketClient_Opened;
            webSocketClient.Closed += webSocketClient_Closed;
            webSocketClient.Error += webSocketClient_Error;
            webSocketClient.MessageReceived += (object sender, MessageReceivedEventArgs eventArgs) => MessageReceived(sender, eventArgs);
            webSocketClient.Open();
            return webSocketClient;
        }

        private void webSocketClient_Error(object sender, ErrorEventArgs e)
        {
            SocketError(sender, e);
        }

        protected void webSocketClient_Closed(object sender, EventArgs e)
        {
            if (_isOpen)
                SocketClosed(sender, e);
            _isOpen = false;
        }

        public void SendReceipt(GameCommand gameCommand)
        {
            Send(gameCommand.CommandId.ToString());
        }

        protected void webSocketClient_Opened(object sender, EventArgs e)
        {
            _isOpen = true;
            SocketOpened(sender, e);
        }

        public delegate void SocketEventHandler(object sender, EventArgs eventArgs);

        public event SocketEventHandler SocketOpened;
        public event SocketEventHandler SocketClosed;
        public event SocketEventHandler SocketError;

        public delegate void MessageReceivedHandler(object sender, MessageReceivedEventArgs eventArgs);
        public event MessageReceivedHandler MessageReceived;
    }
}