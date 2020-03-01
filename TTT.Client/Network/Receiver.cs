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

        public void Begin(Action<IPAddress> createNewSocket)
        {
            //prepare request data
            var requestData = Encoding.ASCII.GetBytes(_clientId.ToString());
            _client.EnableBroadcast = true;

            //send initial request data
            _client.Send(requestData, requestData.Length, new IPEndPoint(IPAddress.Broadcast, 8888));

            //receive server socket address
            var responseData = _client.Receive(ref _serverEndpoint);

            if (Guid.TryParse(Encoding.ASCII.GetString(responseData), out Guid id))
            {
                if (id == _clientId)
                {

                    //prepare contract
                    var contractData = Encoding.ASCII.GetBytes(_serverEndpoint.Address.ToString());
        
                    //send contract
                    _client.Send(contractData, contractData.Length, _serverEndpoint);
                    createNewSocket(_serverEndpoint.Address);
                    _client.Close();
                }
            }

        }
    }
}