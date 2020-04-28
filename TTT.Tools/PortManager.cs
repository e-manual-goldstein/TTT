using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NATUPNPLib;

namespace TTT.Tools
{
    public class PortManager
    {
        UPnPNATClass _upnpnat;

        public PortManager()
        {
            _upnpnat = new NATUPNPLib.UPnPNATClass();

        }

        public string[] GetPorts()
        {
            List<string> list = new List<string>();
            foreach (IStaticPortMapping item in _upnpnat.StaticPortMappingCollection)
            {
                var msg = $"Client = {item.InternalClient}, InternalPort = {item.InternalPort}, Protocol = {item.Protocol}, External Endpoint = {item.ExternalIPAddress}:{item.ExternalPort}, Description = {item.Description} ";
                Console.WriteLine(msg);
                list.Add(msg);
            }
            return list.ToArray();
        }

        public void AddPort()
        {
            var mappings = _upnpnat.StaticPortMappingCollection;

            // Here's an example of forwarding the UDP traffic of Internet Port 80 to Port 8080 on a Computer on the Private Network
            mappings.Add(58008, "TCP", 58008, "192.168.0.13", true, "Inbound TCP");
        }
    }
}
