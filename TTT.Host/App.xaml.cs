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
using TTT.Host.Control;

namespace TTT.Host
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            _logger = new Logger();
            _listener = new Listener(_logger);
            _broadcaster = new Broadcaster(_logger);
            _socketHub = new SocketHub(_logger, new MessageHandler(), new GameController(Host.MainWindow.GameGrid));    
            Host.MainWindow.ButtonAction = () =>
            {
                _listener.Start((msg, ep) => handleIncomingMessage(msg, ep));
                //_socketHub.RequestSocketConnection(IPAddress.Parse("192.168.0.10"), Guid.NewGuid());
            };
        }

        private void handleIncomingMessage(UdpMessage message, IPEndPoint endPoint)
        {
            if (Guid.TryParse(message.Payload, out var id))
            {
                _logger.Log($"Begining Handshake for ID:{id}");
                _broadcaster.BeginHandshake(id, endPoint, _socketHub);
            }
        }

        Logger _logger;
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
