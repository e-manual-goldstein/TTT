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
        Game _game;

        public GameController(Game game, Logger logger) : base(logger)
        {
            _game = game;
        }

        public void TakeTurn(TurnCommand turnCommand)
        {
            _logger.Log($"Player: {turnCommand.PlayerId} takes turn {turnCommand.Cell}");
            _game.TakePlayerTurn(turnCommand.PlayerId, turnCommand.Cell);
        }
    }
}
