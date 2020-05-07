using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using TTT.Common;

namespace TTT.Host.Control
{
    public class GameController : AbstractController
    {
        GameManager _gameManager;

        public GameController(GameManager gameManager, Logger logger) : base(logger)
        {
            _gameManager = gameManager;
        }

        public void TakeTurn(TurnCommand turnCommand)
        {
            _logger.Debug($"Player: {turnCommand.PlayerId} takes turn {turnCommand.Cell}");
            _gameManager.CurrentGame().TakePlayerTurn(turnCommand.PlayerId, turnCommand.Cell);
        }
    }
}
