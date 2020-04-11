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
        Logger _logger;
        static SocketHub _socketHub;
        static GameGrid _gameGrid;

        public App()
        {
            _gameGrid = new GameGrid();
            _logger = new Logger();
            //_listener = new Listener(_logger);
            //_broadcaster = new Broadcaster(_logger);
            _socketHub = new SocketHub(_logger, new MessageHandler(_logger), new GameController(_gameGrid, _logger));
            Host.MainWindow.ButtonAction = async () =>
            {
                var socketId = await _socketHub.ConnectAsync();
                await _socketHub.OpenConnectionAsync(socketId);
                //Console.WriteLine(client.Result);
                //await _listener.StartAsync((msg, ep) => handleIncomingMessage(msg, ep));
                //_socketHub.RequestSocketConnection(IPAddress.Parse("192.168.0.10"), Guid.NewGuid());
            };
        }

        public static SocketHub SocketHub
        {
            get
            {
                return _socketHub;
            }
        }

        public static GameGrid GameGrid { get => _gameGrid; }

    }
}
