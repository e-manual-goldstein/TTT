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
        }


        private static SocketHub _socketHub;
        public static SocketHub SocketHub
        {
            get
            {
                return _socketHub;
            }
        }
    }
}
