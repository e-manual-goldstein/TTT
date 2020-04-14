using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using TTT.Common;

namespace TTT.Client
{
    public class SocketManager
    {
        HostSocket _hostSocket;
        Logger _logger;

        public SocketManager(Logger logger)
        {
            _logger = logger;
        }

        //public async Task Listen(object sender, EventArgs e)
        public async Task Listen(MainActivity mainActivity)
        {
            var targetEndPoint = new IPEndPoint(IPAddress.Any, Constants.SERVER_LISTEN_PORT);

            using (var listener = new UdpClient(targetEndPoint) { EnableBroadcast = true })
            {
                while (_hostSocket == null)
                    await listener.ReceiveAsync().ContinueWith(task =>
                    {
                        var message = UdpMessage.FromByteArray(task.Result.Buffer);
                        if (Guid.TryParse(message.Payload, out Guid clientId))
                        {
                            _hostSocket = new HostSocket(task.Result.RemoteEndPoint.Address, _logger, clientId);
                            _hostSocket.Send($"{clientId}");
                            mainActivity.AsyncAddGameGrid();
                        }
                    });
            }
        }

        public HostSocket HostSocket => _hostSocket;
    }
}