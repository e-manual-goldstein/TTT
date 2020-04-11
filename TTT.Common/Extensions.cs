using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace TTT.Common
{
    public static class Extensions
    {
        public static UdpMessage ReceiveUnique(this UdpClient server, ref IPEndPoint targetEndPoint, ref Dictionary<Guid, string> _messages)
        {
            var messageData = server.Receive(ref targetEndPoint);
            var message = UdpMessage.FromByteArray(messageData);
            while (_messages.ContainsKey(message.Id))
            {
                Console.WriteLine($"Skipping Message: {message.Id}");
                messageData = server.Receive(ref targetEndPoint);
                message = UdpMessage.FromByteArray(messageData);
            }
            _messages[message.Id] = message.Payload;
            return message;
        }

        public static int Send(this UdpClient server, UdpMessage message, IPEndPoint endPoint)
        {
            var dgram = message.ToByteArray();
            return server.Send(dgram, dgram.Length, endPoint);
        }
    }
}
