using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace TTT.Core
{
    public class GameSocket
    {
        public GameSocket()
        {
            KeepAlive = true;
        }

        public bool KeepAlive { get; private set; }
 
        public NetworkStream ActiveStream { get; set; }

        public TcpClient Client { get; set; }

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
                    Output.Debug("Lost Connection. Terminating Socket");
                    Kill();
                }
            }
        }

        public bool IsActive()
        {
            if (!KeepAlive)
            {
                Client.Close();
            }
            return KeepAlive;
        }


    }
}
