using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using TTT.Common;
using TTT.Core;

namespace TTT.Host.Control
{
    public class GameController : AbstractController
    {
        GameGrid _gameGrid;

        public GameController(GameGrid gameGrid, Logger logger) : base(logger)
        {
            _gameGrid = gameGrid;
        }

        public void TakeTurn(TurnCommand turnCommand)
        {
            _logger.Log($"Player: {turnCommand.PlayerId} takes turn {turnCommand.Cell}");
            _gameGrid.TakePlayerTurn(turnCommand.PlayerId, turnCommand.Cell);
        }
    }
}
