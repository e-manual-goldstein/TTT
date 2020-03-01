﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using TTT.Common;

namespace TTT.Client
{
    public class Receiver
    {
        public Receiver(Guid clientId)
        {
            _clientId = clientId;
        }

        UdpClient _client = new UdpClient();
        IPEndPoint _serverEndpoint = new IPEndPoint(IPAddress.Any, 0);
        Guid _clientId;
        Dictionary<Guid, string> _messages = new Dictionary<Guid, string>();


        public void Begin(Action<IPAddress> createNewSocket)
        {
            //prepare request data
            var request = new UdpMessage(_clientId.ToString());
            _client.EnableBroadcast = true;

            //send initial request data
            _client.Send(request, new IPEndPoint(IPAddress.Broadcast, 8888));

            //receive server socket address
            var responseData = _client.ReceiveUnique(ref _serverEndpoint, ref _messages);

            if (Guid.TryParse(responseData.Payload, out Guid id))
            {
                if (id == _clientId)
                {
                    //send contract
                    _client.Send(new UdpMessage(_serverEndpoint.Address.ToString()), _serverEndpoint);
                    createNewSocket(_serverEndpoint.Address);
                    _client.Close();
                }
            }

        }
    }
}