using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace TTT.Host
{
    public class Broadcaster
    {
        UdpClient _server;
        bool _awaitingConnections;
        static int _messageCount;

        public Broadcaster()
        {
            _awaitingConnections = true;
            _server = new UdpClient(8888);
        }

        public void BeginBroadcast(Action<IPAddress, Guid> connectClientAction)
        {
            Task.Run(() =>
            {
                while (_awaitingConnections)
                {
                    //prepare end point
                    var targetEndPoint = new IPEndPoint(IPAddress.Any, 0);

                    //await connection
                    var clientRequestData = _server.Receive(ref targetEndPoint);
                    
                    Print("clientRequestData: " + Encoding.ASCII.GetString(clientRequestData));
                    
                    //process response
                    if (Guid.TryParse(Encoding.ASCII.GetString(clientRequestData), out Guid id))
                    {
                        //begin handshake
                        var serverIP = BeginHandshake(id, targetEndPoint);
                        if (serverIP != null)
                            connectClientAction(serverIP, id);
                    }
                }
            });
        }

        public IPAddress BeginHandshake(Guid clientId, IPEndPoint targetEndPoint)
        {
            Print("clientId: " + clientId);

            Print("targetEndPoint.Address: " + targetEndPoint.Address);

            //prepare data
            var data = Encoding.ASCII.GetBytes(clientId.ToString());

            //send confirmation of clientId
            _server.Send(data, data.Length, targetEndPoint);

            //await address confirmation
            var response = _server.Receive(ref targetEndPoint);
            Print("response: " + Encoding.ASCII.GetString(response));
            while (response != null)
            {
                response = _server.Receive(ref targetEndPoint);
                Print("response: " + Encoding.ASCII.GetString(response));
            }

            return IPAddress.TryParse(Encoding.ASCII.GetString(response), out IPAddress serverIp) ? serverIp : null;
        }

        public void Print(object message)
        {
            Console.WriteLine($"{_messageCount++}: {message}");
        }
    }
}
