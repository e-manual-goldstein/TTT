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
            _serverIP = GetIPAddress();
            _socketHub = new SocketHub();
        }
        public IPAddress GetIPAddress()
        {
            IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName()); // `Dns.Resolve()` method is deprecated.
            IPAddress ipAddress = ipHostInfo.AddressList[1];

            return ipAddress;
        }

        public Dictionary<IPAddress, IPAddress> GEtIPv4Address()
        {
            var map = new Dictionary<IPAddress, IPAddress>();

            foreach (var ni in NetworkInterface.GetAllNetworkInterfaces())
            {
                foreach (var uipi in ni.GetIPProperties().UnicastAddresses)
                {
                    if (uipi.Address.AddressFamily != AddressFamily.InterNetwork) continue;

                    if (uipi.IPv4Mask == null) continue; //ignore 127.0.0.1
                    map[uipi.Address] = uipi.IPv4Mask;
                }
            }
            return map;
        }

        IPAddress _serverIP;


        private static SocketHub _socketHub;
        public static SocketHub SocketHub
        {
            get
            {
                return _socketHub;
            }
        }

        public void CreateSocketHub()
        {
            while (true)
            {
                try
                {
                    //var guid = Guid.NewGuid();
                    //SocketHub.RequestSocketConnection(guid);
                    //SocketHub.OpenConnection(guid);
                }
                catch
                {

                }
            }
        }
    }
}
