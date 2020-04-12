using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using TTT.Common;
using WebSocket4Net;
using Xamarin.Essentials;

namespace TTT.Client
{
    public class HostSocket
    {
        IPAddress _serverAddress;
        Logger _logger;

        bool _isOpen;

        public HostSocket(IPAddress serverAddress, Logger logger, bool openSocket = false)
        {
            _serverAddress = serverAddress;
            _logger = logger;
            if (openSocket)
                openWebSocket();
        }

        public bool IsOpen => _isOpen;

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
            openWebSocket();
        }

        WebSocket _webSocket;
        WebSocket WebSocket
        {
            get
            {
                return _webSocket ?? (_webSocket = openWebSocket());
            }
        }

        private WebSocket openWebSocket()
        {
            var webSocketClient = new WebSocket($"ws://{_serverAddress.ToString()}:69/");
            webSocketClient.Opened += new EventHandler(webSocketClient_Opened);
            webSocketClient.Closed += new EventHandler(webSocketClient_Closed);
            webSocketClient.MessageReceived += WebSocketClient_MessageReceived;
            //webSocketClient.MessageReceived += new EventHandler<MessageReceivedEventArgs>(webSocketClient_MessageReceived);
            webSocketClient.Open();
            return webSocketClient;
        }

        private void WebSocketClient_MessageReceived(object sender, MessageReceivedEventArgs e)
        {
            _logger.Log(e.Message);
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