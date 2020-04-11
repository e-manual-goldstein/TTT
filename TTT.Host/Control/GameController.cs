using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using TTT.Common;
using TTT.Core;

namespace TTT.Host.Control
{
    public class GameController
    {
        Dictionary<Type, MethodInfo> _actionDictionary = new Dictionary<Type, MethodInfo>();
        GameGrid _gameGrid;
        Logger _logger;
        Action<string> PostBackAction;

        public GameController(GameGrid gameGrid, Logger logger)
        {
            _gameGrid = gameGrid;
            _logger = logger;
        }

        public void ExecuteCommand(GameCommand gameCommand, Action<string> postbackMessage)
        {
            if (!_actionDictionary.ContainsKey(gameCommand.CommandType))
            {
                if (!FindAction(gameCommand.CommandType))
                    return;
            }
            PostBackAction = postbackMessage;
            _actionDictionary[gameCommand.CommandType].Invoke(this, new object[] { gameCommand.SubCommand() });
            PostBackAction = null;
        }

        private bool FindAction(Type argumentType)
        {
            _actionDictionary[argumentType] = GetType().GetMethods().SingleOrDefault(m => m.GetParameters().FirstOrDefault()?.ParameterType == argumentType);
            return _actionDictionary[argumentType] != null;
        }

        public void TakeTurn(TurnCommand turnCommand)
        {
            _logger.Log($"Player: {turnCommand.PlayerId} takes turn {turnCommand.Cell}");
            _gameGrid.GameCells[turnCommand.Cell.I, turnCommand.Cell.J].UpdateValue(turnCommand.Cell.Marker.Value);
            PostBackAction("Message received");
        }
    }
}
