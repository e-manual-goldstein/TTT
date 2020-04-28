using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TTT.Host.Api;

namespace TTT.Host
{
    public class ExternalConnectionManager
    {
        PortManager _portManager;
        ISocketHub _socketHub;

        public ExternalConnectionManager(PortManager portManager, ISocketHub socketHub)
        {
            _portManager = portManager;
            _socketHub = socketHub;
        }

        public async Task OpenExternalSocket(Guid portId)
        {
            _portManager.EnsurePortIsForwarded(new PortForwardSettings(58008, _socketHub.ServerPort, PortType.TCP, _socketHub.ServerAddress.ToString(), portId.ToString()));
            await PublishHostSocket();
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
            using (var client = new HttpClient())
            {
                await client.GetAsync("http://www.goldstein.somee.com/host/TerminateAll");
            }
        }
    }
}
