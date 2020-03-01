using Newtonsoft.Json;
using System;
using System.Text;

namespace TTT.Common
{
    public class UdpMessage
    {
        public Guid MessageId { get; set; }

        public string Message { get; set; }

        public byte[] ToByteArray()
        {
            return Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(this));
        }
    }
}
