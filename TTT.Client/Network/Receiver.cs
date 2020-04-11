using System;
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
        public Receiver(Guid clientId, Logger logger)
        {
            _clientId = clientId;
            _logger = logger;
        }

        Logger _logger;
        IPEndPoint _serverEndpoint = new IPEndPoint(IPAddress.Any, 0);
        Guid _clientId;
        Dictionary<Guid, string> _messages = new Dictionary<Guid, string>();

        public Guid DeviceId => _clientId;

        public void Begin(Action<IPAddress> createNewSocket, Action newConnectionCreated)
        {
            _logger.Log("Preparing request data");
            //prepare request data
            var request = new UdpMessage(_clientId.ToString());

            _logger.Log($"Sending request data to {IPAddress.Broadcast}:{Constants.SERVER_LISTEN_PORT}");
            using (var _client = new UdpClient() { EnableBroadcast = true })
            {
                //send initial request data
                _client.Send(request, new IPEndPoint(IPAddress.Broadcast, Constants.SERVER_LISTEN_PORT));

                _logger.Log($"Awaiting message on {_client.Client.LocalEndPoint}");
                //receive clientId Confirmation
                var responseData = _client.ReceiveUnique(ref _serverEndpoint, ref _messages);

                _logger.Log($"Message received {responseData.Id}");

                if (Guid.TryParse(responseData.Payload, out Guid id))
                {
                    if (id == _clientId)
                    {
                        //send contract 
                        _client.Send(new UdpMessage(_serverEndpoint.Address.ToString()), _serverEndpoint);
                        createNewSocket(_serverEndpoint.Address);
                        _client.Close();
                        newConnectionCreated();
                    }
                }
            }
        }
    }
}