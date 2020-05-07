using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NATUPNPLib;
using TTT.Common;

namespace TTT.Host
{
    public class PortManager
    {
        UPnPNATClass _upnpnat;
        List<IStaticPortMapping> _portList;
        Logger _logger;

        public PortManager(Logger logger)
        {
            _logger = logger;
            _upnpnat = new UPnPNATClass();
        }
        public void EnsurePortIsForwarded(PortForwardSettings portForwardSettings)
        {
            if (PortIsForwarded(portForwardSettings))
            {
                _logger.Warning("Port is already forwarded");
                return; //No need to do anything
            }
            if (PortIsDisabled(portForwardSettings))
            {
                _logger.Warning("Port is disabled");
                EnablePort(portForwardSettings); // Just enable the port as all other settings are correct
                return;
            }
            if (PortIsInUse(portForwardSettings.ExternalPort, portForwardSettings.Protocol))
            {
                var exception = new InvalidPortException($"Port ({portForwardSettings.ExternalPort}) is already reserved for another {portForwardSettings.Protocol} use. Choose a different port");
                _logger.Error(exception);
                throw exception;
            }
            _logger.Debug("Forwarding Port for external connections");
            _upnpnat.StaticPortMappingCollection
                .Add(portForwardSettings.ExternalPort, portForwardSettings.Protocol.ToString(), 
                portForwardSettings.InternalPort, portForwardSettings.InternalIPAddress, true, portForwardSettings.Description);
            _portList = null;
        }

        private void EnablePort(int externalPort, int internalPort, PortType protocol, string internalIPAddress)
        {
            MatchingPorts(externalPort,internalPort, protocol, internalIPAddress, false).Single().Enable(true);
        }

        private void EnablePort(PortForwardSettings portForwardSettings)
        {
            _logger.Debug("Enabling Port Forward");
            MatchingPorts(portForwardSettings, false).Single().Enable(true);
        }

        private bool PortIsDisabled(int externalPort, int internalPort, PortType protocol, string internalIPAddress)
        {
            return MatchingPorts(externalPort, internalPort, protocol, internalIPAddress, false).Any();
        }
        private bool PortIsDisabled(PortForwardSettings portForwardSettings)
        {
            return MatchingPorts(portForwardSettings, false).Any();
        }

        private bool PortIsInUse(int externalPort, PortType protocol)
        {
            return GetPorts().Where(p => p.ExternalPort == externalPort && p.Protocol == protocol.ToString()).Any();
        }

        private bool PortIsForwarded(int externalPort, int internalPort, PortType protocol, string internalIPAddress)
        {
            return MatchingPorts(externalPort, internalPort, protocol, internalIPAddress, true).Any();
        }

        private bool PortIsForwarded(PortForwardSettings portForwardSettings)
        {
            return MatchingPorts(portForwardSettings, true).Any();
        }

        private IEnumerable<IStaticPortMapping> MatchingPorts(int externalPort, int internalPort, PortType protocol, string internalIPAddress, bool enabled)
        {
            return GetPorts().Where(p
                    => p.Enabled == enabled
                    && p.ExternalPort == externalPort
                    && p.InternalPort == internalPort
                    && p.Protocol == protocol.ToString()
                    && p.InternalClient == internalIPAddress
                );
        }

        private IEnumerable<IStaticPortMapping> MatchingPorts(PortForwardSettings portForwardSettings, bool enabled)
        {
            return GetPorts().Where(p
                    => p.Enabled == enabled
                    && p.ExternalPort == portForwardSettings.ExternalPort
                    && p.InternalPort == portForwardSettings.InternalPort
                    && p.Protocol == portForwardSettings.Protocol.ToString()
                    && p.InternalClient == portForwardSettings.InternalIPAddress
                );
        }

        private IStaticPortMapping[] GetPorts()
        {
            if (_portList == null)
            {
                _portList = new List<IStaticPortMapping>();
                foreach (IStaticPortMapping item in _upnpnat.StaticPortMappingCollection)
                {
                    _portList.Add(item);
                }
            }
            return _portList.ToArray();
        }

        private string[] ListAllPorts()
        {
            List<string> list = new List<string>();
            foreach (IStaticPortMapping item in _upnpnat.StaticPortMappingCollection)
            {
                var msg = $"Client = {item.InternalClient}, InternalPort = {item.InternalPort}, Protocol = {item.Protocol}, External Endpoint = {item.ExternalIPAddress}:{item.ExternalPort}, Description = {item.Description}, Enabled = {item.Enabled}";
                Console.WriteLine(msg);
                list.Add(msg);
            }
            return list.ToArray();
        }

    }
}
