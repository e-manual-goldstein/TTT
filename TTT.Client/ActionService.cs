using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TTT.Common;

namespace TTT.Client
{
    public class ActionService
    {
        SocketManager _socketManager;


        public ActionService(SocketManager socketManager)
        {
            _socketManager = socketManager;
        }

        public void TakeTurn(Cell cell)
        {
            var hostSocket = _socketManager.HostSocket;
            var turnCommand = new TurnCommand(hostSocket.ClientId, cell);
            hostSocket.Send(new GameCommand(turnCommand));
        }
    }
}