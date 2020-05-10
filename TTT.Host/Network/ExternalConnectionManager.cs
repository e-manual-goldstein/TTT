using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TTT.Common;
using TTT.Host.Api;

namespace TTT.Host
{
    public class ExternalConnectionManager
    {
        PortManager _portManager;
        ISocketHub _socketHub;
        Logger _logger;
        bool _isOpened;

        public ExternalConnectionManager(PortManager portManager, ISocketHub socketHub, Logger logger)
        {
            _portManager = portManager;
            _socketHub = socketHub;
            _logger = logger;
        }

        public async Task EnsureExternalSocketOpen(Guid portId)
        {
            if (!_isOpened)
                await OpenExternalSocket(portId);
        }

        private async Task OpenExternalSocket(Guid portId)
        {
            _logger.Debug("Opening External Socket");
            _portManager.EnsurePortIsForwarded(new PortForwardSettings(58008, _socketHub.ServerPort, PortType.TCP, _socketHub.ServerAddress.ToString(), portId.ToString()));
            _logger.Debug("Publishing socket information to webhost");
            await PublishHostSocket();
            _isOpened = true;
        }

        private async Task PublishHostSocket()
        {
            using (var client = new HttpClient())
            {
                await client.GetAsync("http://www.goldstein.somee.com/host/submit");
            }
        }

        public async Task TerminateAllActive()
        {
            _logger.Warning("Terminating all Active External Sockets");
            using (var client = new HttpClient())
            {
                await client.GetAsync("http://www.goldstein.somee.com/host/TerminateAll");
            }
        }
    }
}
