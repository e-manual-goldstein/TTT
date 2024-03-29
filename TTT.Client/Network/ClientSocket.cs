﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using WebSocket4Net;
using Xamarin.Essentials;

namespace TTT.Client
{
    public class ClientSocket
    {
        IPAddress _serverAddress;

        public ClientSocket(IPAddress serverAddress)
        {
            _serverAddress = serverAddress;
        }

        protected AutoResetEvent m_MessageReceiveEvent = new AutoResetEvent(false);
        protected AutoResetEvent m_OpenedEvent = new AutoResetEvent(false);
        protected AutoResetEvent m_CloseEvent = new AutoResetEvent(false);

        WebSocket _webSocket;
        public WebSocket WebSocket
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
            webSocketClient.MessageReceived += new EventHandler<MessageReceivedEventArgs>(webSocketClient_MessageReceived);
            webSocketClient.Open();
            return webSocketClient;
        }

        public void Test()
        {
            var webSocketClient = WebSocket;

            if (!m_OpenedEvent.WaitOne(5000))
            {
                //Assert.Fail("Failed to Opened session ontime");
            }

            for (var i = 0; i < 10; i++)
            {
                var message = Guid.NewGuid().ToString();

                webSocketClient.Send(message);

                if (!m_MessageReceiveEvent.WaitOne(5000))
                {
                    //Assert.Fail("Failed to get echo messsage on time");
                    break;
                }
            }
        }


        protected void webSocketClient_MessageReceived(object sender, MessageReceivedEventArgs e)
        {
            //Snackbar.Make(MainView, e.Message, Snackbar.LengthLong)
            //    .SetAction("Action", (Android.Views.View.IOnClickListener)null).Show();
        }

        protected void webSocketClient_Closed(object sender, EventArgs e)
        {
            m_CloseEvent.Set();
        }

        protected void webSocketClient_Opened(object sender, EventArgs e)
        {
            m_OpenedEvent.Set();
        }

    }
}