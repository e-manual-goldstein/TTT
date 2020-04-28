using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using TTT.Common;
using WebSocket4Net;

namespace TTT.Tools
{
    public class SocketManager
    {
        HostSocket _hostSocket;
        Logger _logger;
        ControllerManager _controllerManager;
        bool _listening;

        public SocketManager(Logger logger, ControllerManager controllerManager)
        {
            _logger = logger;
            _controllerManager = controllerManager;
        }

        public async Task<Guid> ListenForHostInvitation()
        {
            _listening = true;
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
                            _hostSocket = new HostSocket(task.Result.RemoteEndPoint.Address, clientId, new EventHandler<MessageReceivedEventArgs>(processMessage), true);
                            _listening = false;
                            _logger.Log("Connected!");
                        }
                    });
                //_hostSocket.Send($"{clientId}");
                return clientId;
            }
        }


        public async Task<UdpMessage> JustListen()
        {
            var targetEndPoint = new IPEndPoint(IPAddress.Any, Constants.SERVER_LISTEN_PORT);
            using (var listener = new UdpClient(targetEndPoint) { EnableBroadcast = true })
            {
                return await listener.ReceiveAsync().ContinueWith(task =>
                {
                    return UdpMessage.FromByteArray(task.Result.Buffer);
                });
            }
        }

        protected void processMessage(object sender, MessageReceivedEventArgs eventArgs)
        {
            //if (GameCommand.TryParse(eventArgs.Message, out var gameCommand))
            //    _controllerManager.ExecuteCommand(gameCommand);
            //else
                _logger.Log(eventArgs.Message);
        }

        public HostSocket HostSocket => _hostSocket;
    }
}