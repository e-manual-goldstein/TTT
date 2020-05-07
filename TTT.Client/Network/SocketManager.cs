using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using TTT.Common;
using WebSocket4Net;
using ErrorEventArgs = SuperSocket.ClientEngine.ErrorEventArgs;

namespace TTT.Client
{
    public class SocketManager
    {
        Logger _logger;
        ControllerManager _controllerManager;

        HostSocket _hostSocket;
        public HostSocket HostSocket => _hostSocket;

        public SocketManager(Logger logger, ControllerManager controllerManager)
        {
            _logger = logger;
            _controllerManager = controllerManager;
        }

        public async Task<Guid> Listen()
        {
            var targetEndPoint = new IPEndPoint(IPAddress.Any, Constants.SERVER_LISTEN_PORT);
            Guid clientId = Guid.Empty;
            using (var listener = new UdpClient(targetEndPoint) { EnableBroadcast = true })
            {
                if (_hostSocket == null || !_hostSocket.IsOpen)
                    await listener.ReceiveAsync().ContinueWith(task =>
                    {
                        var message = UdpMessage.FromByteArray(task.Result.Buffer);
                        if (Guid.TryParse(message.Payload, out clientId))
                        {
                            _hostSocket = new HostSocket(task.Result.RemoteEndPoint.Address, clientId, true);
                            AttachHandlers();
                        }
                    });
                //_hostSocket.Send($"{clientId}");
                return clientId;
            }
        }

        protected void processMessage(object sender, MessageReceivedEventArgs eventArgs)
        {
            if (GameCommand.TryParse(eventArgs.Message, out var gameCommand))
                _controllerManager.ExecuteCommand(gameCommand);
            else
                _logger.Log(eventArgs.Message);
        }


        internal void CreateSocket(IPEndPoint ipEndpoint)
        {
            _hostSocket = new HostSocket(ipEndpoint.Address, Guid.NewGuid(), true);
            AttachHandlers();
        }

        private void AttachHandlers()
        {
            _hostSocket.MessageReceived += processMessage;
            _hostSocket.SocketError += (object sender, EventArgs eventArgs) => _logger.Error((eventArgs as ErrorEventArgs).Exception);
            _hostSocket.SocketOpened += (object sender, EventArgs eventArgs) => _logger.Log("Connected to Host");
            _hostSocket.SocketClosed += (object sender, EventArgs eventArgs) => _logger.Log("Lost connection to Host");
        }
    }
}