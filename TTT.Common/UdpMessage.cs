using Newtonsoft.Json;
using System;
using System.Text;

namespace TTT.Common
{
    public class UdpMessage
    {
        public UdpMessage(string payload)
        {
            Id = Guid.NewGuid();
            Payload = payload;
        }

        public Guid Id { get; private set; }

        public string Payload { get; private set; }

        public byte[] ToByteArray()
        {
            return Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(this));
        }

        public static UdpMessage FromByteArray(byte[] messageData)
        {
            return JsonConvert.DeserializeObject<UdpMessage>(Encoding.ASCII.GetString(messageData));
        }
    }
}
