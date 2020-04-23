using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using TTT.Common;
using TTT.Host.Control;

namespace TTT.Host
{
    public class GameSocket
    {
        Logger _logger;

        public delegate void CommandReceivedEventHandler(object sender, CommandReceivedEventArgs gameCommand);

        event CommandReceivedEventHandler _receiveCommandEvent;

        public GameSocket(Logger logger)
        {
            _logger = logger;
            KeepAlive = true;
        }

        public bool KeepAlive { get; private set; }
 
        public NetworkStream ActiveStream { get; set; }

        public TcpClient Client { get; set; }

        public event CommandReceivedEventHandler CommandReceived 
        { 
            add { _receiveCommandEvent += value; } 
            remove { _receiveCommandEvent -= value; }
        }

        public void Kill()
        {
            KeepAlive = false;
            ActiveStream = null;
        }
        public void Send(string message)
        {
            var socketStream = ActiveStream;
            if (socketStream != null)
            {
                var buf = Encoding.UTF8.GetBytes(message);
                int frameSize = 64;

                var parts = buf.Select((b, i) => new { b, i })
                               .GroupBy(x => x.i / (frameSize - 1))
                               .Select(x => x.Select(y => y.b).ToArray())
                               .ToList();

                try
                {
                    for (int i = 0; i < parts.Count; i++)
                    {
                        byte cmd = 0;
                        if (i == 0) cmd |= 1;
                        if (i == parts.Count - 1) cmd |= 0x80;
                        socketStream.WriteByte(cmd);
                        socketStream.WriteByte((byte)parts[i].Length);
                        socketStream.Write(parts[i], 0, parts[i].Length);
                    }

                    socketStream.Flush();
                }
                catch
                {
                    Kill();
                }
            }
        }

        public void Send(GameCommand command)
        {
            Send(JsonConvert.SerializeObject(command));
        }

        public bool IsActive()
        {
            if (!KeepAlive)
            {
                Client.Close();
            }
            return KeepAlive;
        }

        public void Receive(byte[] bytes)
        {
            bool fin = (bytes[0] & 0b10000000) != 0,
                    mask = (bytes[1] & 0b10000000) != 0; // must be true, "All messages from the client to the server have this bit set"

            int opcode = bytes[0] & 0b00001111, // expecting 1 - text message
                msglen = bytes[1] - 128, // & 0111 1111
                offset = 2;

            if (msglen == 126)
            {
                // was ToUInt16(bytes, offset) but the result is incorrect
                msglen = BitConverter.ToUInt16(new byte[] { bytes[3], bytes[2] }, 0);
                offset = 4;
            }
            else if (msglen == 127)
            {
                _logger.Warning("TODO: msglen == 127, needs qword to store msglen");
                // i don't really know the byte order, please edit this
                // msglen = BitConverter.ToUInt64(new byte[] { bytes[5], bytes[4], bytes[3], bytes[2], bytes[9], bytes[8], bytes[7], bytes[6] }, 0);
                // offset = 10;
            }

            if (msglen == 0)
                _logger.Warning("msglen == 0");
            else if (mask)
            {
                string messageReceived = decodeMessage(bytes, offset, msglen);
                if (GameCommand.TryParse(messageReceived, out GameCommand gameCommand))
                {
                    _receiveCommandEvent.Invoke(this, new CommandReceivedEventArgs(gameCommand));
                }
                else
                    _logger.Log($"Received message: {messageReceived}");
            }
            else
                _logger.Warning("mask bit not set");
        }

        private string decodeMessage(byte[] bytes, int offset, int msglen)
        {
            byte[] decoded = new byte[msglen];
            byte[] masks = new byte[4] { bytes[offset], bytes[offset + 1], bytes[offset + 2], bytes[offset + 3] };
            offset += 4;

            for (int i = 0; i < msglen; ++i)
                decoded[i] = (byte)(bytes[offset + i] ^ masks[i % 4]);

            return Encoding.UTF8.GetString(decoded);
        }


    }
}
