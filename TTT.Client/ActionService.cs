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
        CommandManager _commandManager;
        PlayerManager _playerManager;
        public ActionService(SocketManager socketManager, CommandManager commandManager, PlayerManager playerManager)
        {
            _socketManager = socketManager;
            _commandManager = commandManager;
            _playerManager = playerManager;
        }

        public void TakeTurn(Cell cell)
        {
            var hostSocket = _socketManager.HostSocket;
            var turnCommand = new TurnCommand(_playerManager.GetPlayerId(), cell);
            var gameCommand = _commandManager.CreateCommand(turnCommand);
            _commandManager.SendVerify(hostSocket, gameCommand);
        }
    }
}