using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using TTT.Common;
using WebSocket4Net;

namespace TTT.Client
{
    public class HostSocket
    {
        IPAddress _serverAddress;
        Guid _clientId;
        EventHandler<MessageReceivedEventArgs> _messageHandler;
        bool _isOpen;

        public HostSocket(IPAddress serverAddress, Guid clientId, EventHandler<MessageReceivedEventArgs> messageHandler, bool openSocket = false)
        {
            _serverAddress = serverAddress;
            _messageHandler = messageHandler;
            _clientId = clientId;
            if (openSocket)
                _webSocket = openWebSocket(_messageHandler);
        }

        public bool IsOpen => _isOpen;
        public Guid ClientId => _clientId;
        public void Send(GameCommand command)
        {
            Send(JsonConvert.SerializeObject(command));
        }

        public void Send(string message)
        {
            WebSocket.Send(message);
        }

        public void OpenSocket()
        {
            openWebSocket(_messageHandler);
        }

        WebSocket _webSocket;
        WebSocket WebSocket
        {
            get
            {
                return _webSocket ?? (_webSocket = openWebSocket(_messageHandler));
            }
        }

        private WebSocket openWebSocket(EventHandler<MessageReceivedEventArgs> eventHandler)
        {
            var webSocketClient = new WebSocket($"ws://{_serverAddress.ToString()}:69/");
            webSocketClient.Opened += new EventHandler(webSocketClient_Opened);
            webSocketClient.Closed += new EventHandler(webSocketClient_Closed);
            webSocketClient.MessageReceived += eventHandler;
            webSocketClient.Error += webSocketClient_Error;
            webSocketClient.Open();
            return webSocketClient;
        }

        protected void webSocketClient_Error(object sender, SuperSocket.ClientEngine.ErrorEventArgs e)
        {
            Console.WriteLine(e.Exception);
        }

        protected void webSocketClient_Closed(object sender, EventArgs e)
        {
            _isOpen = false;
        }

        protected void webSocketClient_Opened(object sender, EventArgs e)
        {
            _isOpen = true;
        }
    }
}