using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using TTT.Common;

namespace TTT.Host
{
    public class Broadcaster
    {
        UdpClient _server;
        bool _awaitingConnections;
        static int _messageCount;
        Dictionary<Guid, string> _messages = new Dictionary<Guid, string>();
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

                    Print($"Listening to {targetEndPoint.Address}");

                    //await connection
                    var message = _server.ReceiveUnique(ref targetEndPoint, ref _messages);

                    Print($"Received message '{message.Payload}' from {targetEndPoint.Address}");
                    
                    //process response
                    if (Guid.TryParse(message.Payload, out Guid id))
                    {
                        //begin handshake
                        Print($"Beginning Handshake for {id}");
                        var serverIP = BeginHandshake(id, targetEndPoint);
                        Print($"Handshake completed for {id}, serverIP is {serverIP}");
                        if (serverIP != null)
                            connectClientAction(serverIP, id);
                    }
                }
            });
        }

        public IPAddress BeginHandshake(Guid clientId, IPEndPoint targetEndPoint)
        {
            //prepare data
            var data = new UdpMessage(clientId.ToString());

            Print($"Sending confirmation of clientId: {clientId}");
            //send confirmation of clientId
            _server.Send(data, targetEndPoint);
            
            //await address confirmation
            var response = _server.ReceiveUnique(ref targetEndPoint, ref _messages);
            Print($"Received message: {response.Payload}");
            //var message = HandleReceivedMessage(response);
            //if (message == null)
            //{
            //    return null;
            //}

            //while (response != null)
            //{
            //    response = _server.Receive(ref targetEndPoint);
            //    Print("response: " + Encoding.ASCII.GetString(response));
            //}
            return IPAddress.TryParse(response.Payload, out IPAddress serverIp) ? serverIp : null;
        }

        //private UdpMessage HandleReceivedMessage(byte[] messageData)
        //{
        //    var message = UdpMessage.FromByteArray(messageData);
        //    if (_messages.ContainsKey(message.Id))
        //        return null;
        //    _messages[message.Id] = message.Payload;
        //    return message;
        //}

        

        public void Print(object message)
        {
            Console.WriteLine($"{_messageCount++}: {message}");
        }
    }
}
