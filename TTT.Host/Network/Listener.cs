using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TTT.Common;

namespace TTT.Host
{
    public class Listener
    {
        UdpClient _server;
        bool _awaitingConnections;
        Dictionary<Guid, string> _messages = new Dictionary<Guid, string>();
        Logger _logger;

        public Listener(Logger logger)
        {
            _logger = logger;
            _awaitingConnections = true;
            _server = new UdpClient(Constants.SERVER_LISTEN_PORT);
        }

        public void Start(Action<UdpMessage, IPEndPoint> handleIncomingMessage)
        {
            Task.Run(() =>
            {
                _logger.Log("Listening for connections");
                while (_awaitingConnections)
                {
                    //prepare end point
                    var targetEndPoint = new IPEndPoint(IPAddress.Any, 0);

                    //await message
                    var message = _server.ReceiveUnique(ref targetEndPoint, ref _messages);
                    _logger.Log($"Received message; ID={message.Id}");
                    handleIncomingMessage(message, targetEndPoint);
                }
            });
        }

        public void Stop()
        {
            _awaitingConnections = false;
            _server.Close();
        }
    }
}
