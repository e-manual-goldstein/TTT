using System;
using System.Collections.Generic;
using System.Text;

namespace TTT.Host
{
    public struct PortForwardSettings
    {
        public PortForwardSettings(int externalPort, int internalPort, PortType protocol, string internalIPAddress, string description)
        {
            ExternalPort = externalPort;
            InternalPort = internalPort;
            Protocol = protocol;
            InternalIPAddress = internalIPAddress;
            Description = description;
        }

        public int ExternalPort { get; private set; }
        public int InternalPort { get; private set; }
        public PortType Protocol { get; private set; }
        public string InternalIPAddress { get; private set; }
        public string Description { get; private set; }
    }
}
