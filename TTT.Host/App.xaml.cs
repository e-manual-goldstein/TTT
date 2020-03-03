using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Windows;
using TTT.Common;
using TTT.Core;

namespace TTT.Host
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            _socketHub = new SocketHub();
            _listener = new Listener();
            _broadcaster = new Broadcaster();

            Host.MainWindow.ButtonAction = () =>
            {
                _listener.Start((msg, ep) => handleIncomingMessage(msg, ep));
            };

        }

        private void handleIncomingMessage(UdpMessage message, IPEndPoint endPoint)
        {
            if (Guid.TryParse(message.Payload, out var id))
                _broadcaster.BeginHandshake(id, endPoint, _socketHub);
        }

        Broadcaster _broadcaster;

        public Broadcaster Broadcaster
        {
            get
            {
                return _broadcaster;
            }
        }

        private static SocketHub _socketHub;
        public static SocketHub SocketHub
        {
            get
            {
                return _socketHub;
            }
        }

        private static Listener _listener;
        public static Listener Listener 
        {
            get
            {
                return _listener;
            }
        }
    }
}
