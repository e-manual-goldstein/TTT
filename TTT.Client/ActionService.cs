using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TTT.Common;

namespace TTT.Client
{
    public class ActionService
    {
        ClientSocket _clientSocket;
        Guid _clientId;

        public ActionService(Guid clientId, ClientSocket clientSocket)
        {
            _clientId = clientId;
            _clientSocket = clientSocket;
        }

        public void TakeTurn(Cell cell)
        {
            var turnCommand = new TurnCommand(_clientId, cell);
            _clientSocket.Send(new GameCommand(turnCommand));
        }
    }
}