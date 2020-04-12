using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TTT.Common;

namespace TTT.Client
{
    public class ActionService
    {
        HostSocket _hostSocket;
        Guid _clientId;

        public ActionService(Guid clientId, HostSocket hostSocket)
        {
            _clientId = clientId;
            _hostSocket = hostSocket;
        }

        public void TakeTurn(Cell cell)
        {
            var turnCommand = new TurnCommand(_clientId, cell);
            _hostSocket.Send(new GameCommand(turnCommand));
        }
    }
}