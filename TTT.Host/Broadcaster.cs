using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace TTT.Host
{
    public class Broadcaster
    {
        public Broadcaster()
        {
            var server = new UdpClient(8888);
            var responseData = Encoding.ASCII.GetBytes("SomeResponseData");

            while (true)
            {
                var clientEp = new IPEndPoint(IPAddress.Any, 0);
                var clientRequestData = server.Receive(ref clientEp);
                var clientRequest = Encoding.ASCII.GetString(clientRequestData);

                Console.WriteLine("Recived {0} from {1}, sending response", clientRequest, clientEp.Address.ToString());
                server.Send(responseData, responseData.Length, clientEp);
            }
        }
    }
}
