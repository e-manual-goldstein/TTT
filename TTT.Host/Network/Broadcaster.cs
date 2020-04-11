using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using TTT.Common;
using TTT.Core;

namespace TTT.Host
{
    public class Broadcaster
    {
        bool _awaitingConnections;
        static int _messageCount;
        Dictionary<Guid, string> _messages = new Dictionary<Guid, string>();
        Logger _logger;
        public Broadcaster(Logger logger)
        {
            _logger = logger;
            _awaitingConnections = true;
        }

        public void BeginBroadcast(Action<IPAddress, Guid> connectClientAction)
        {
            //Task.Run(() =>
            //{
            //    while (_awaitingConnections)
            //    {
            //        //prepare end point
            //        var targetEndPoint = new IPEndPoint(IPAddress.Any, 0);

            //        Print($"Listening to {targetEndPoint.Address}");

            //        //await connection
            //        var message = _server.ReceiveUnique(ref targetEndPoint, ref _messages);

            //        Print($"Received message '{message.Payload}' from {targetEndPoint.Address}");
                    
            //        //process response
            //        if (Guid.TryParse(message.Payload, out Guid id))
            //        {
            //            //begin handshake
            //            Print($"Beginning Handshake for {id}");
            //            var serverIP = BeginHandshake(id, targetEndPoint);
            //            Print($"Handshake completed for {id}, serverIP is {serverIP}");
            //            if (serverIP != null)
            //                connectClientAction(serverIP, id);
            //        }
            //    }
            //});
        }

        public void BeginBroadcastAsync()
        {
            ////await connection
            //_server.ReceiveAsync()
            //    .ContinueWith(async (task) =>
            //{
            //    var result = await task;
            //    var message = UdpMessage.FromByteArray(result.Buffer);
            //    Console.WriteLine(message.Payload);
            //    BeginBroadcastAsync();
            //});
            
            //return message;
        }

        //public IPAddress BeginHandshake(Guid clientId, IPEndPoint targetEndPoint)
        //{
            ////prepare data
            //var data = new UdpMessage(clientId.ToString());

            //Print($"Sending confirmation of clientId: {clientId}");
            ////send confirmation of clientId
            //_server.Send(data, targetEndPoint);
            
            ////await address confirmation
            //var response = _server.ReceiveUnique(ref targetEndPoint, ref _messages);
            //Print($"Received message: {response.Payload}");
            ////var message = HandleReceivedMessage(response);
            ////if (message == null)
            ////{
            ////    return null;
            ////}

            ////while (response != null)
            ////{
            ////    response = _server.Receive(ref targetEndPoint);
            ////    Print("response: " + Encoding.ASCII.GetString(response));
            ////}
            //return IPAddress.TryParse(response.Payload, out IPAddress serverIp) ? serverIp : null;
        //}

        //public void BeginHandshake(Guid clientId, IPEndPoint targetEndPoint, SocketHub socketHub)
        //{
        //    using (var server = new UdpClient())
        //    {
        //        _logger.Log($"Created new UdpCLient: {server.Client.LocalEndPoint}");

        //        _logger.Log($"Sending message to {targetEndPoint.Address}:{targetEndPoint.Port}");
        //        server.Send(new UdpMessage(clientId.ToString()), targetEndPoint);

        //        //await connection
        //        server.ReceiveAsync()
        //            .ContinueWith(async (task) =>
        //            {
        //                var result = await task;
        //                var message = UdpMessage.FromByteArray(result.Buffer);
        //                _logger.Log($"Received message '{message.Payload}'");
        //                if (IPAddress.TryParse(message.Payload, out var address))
        //                {
        //                    socketHub.RequestSocketConnection(address, clientId);
        //                    socketHub.OpenConnectionAsync(clientId);
        //                }
        //            });
        //    }
        //}

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
