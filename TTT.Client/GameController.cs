using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TTT.Common;

namespace TTT.Client
{
    public class GameController
    {
        ClientSocket _clientSocket;
        Guid _clientId;

        public GameController(Guid clientId, ClientSocket clientSocket)
        {
            _clientId = clientId;
            _clientSocket = clientSocket;
        }

        public void TakeTurn(Cell cell)
        {
            _clientSocket.Send(new TurnCommand(_clientId,cell));
        }
    }
}