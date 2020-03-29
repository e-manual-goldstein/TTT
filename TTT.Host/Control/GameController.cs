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

        public GameController(GameGrid gameGrid)
        {
            _gameGrid = gameGrid;
        }

        public void ExecuteCommand(IGameCommand gameCommand)
        {
            if (!_actionDictionary.ContainsKey(gameCommand.CommandType))
            {
                if (!FindAction(gameCommand.CommandType))
                    return;
            }
            _actionDictionary[gameCommand.CommandType].Invoke(this, new object[] { gameCommand });
        }

        private bool FindAction(Type argumentType)
        {
            _actionDictionary[argumentType] = GetType().GetMethods(BindingFlags.Public).SingleOrDefault(m => m.GetParameters().FirstOrDefault()?.ParameterType == argumentType);
            return _actionDictionary[argumentType] != null;
        }

        public void TakeTurn(TurnCommand turnCommand)
        {
            _gameGrid.GameCells[turnCommand.Cell.I, turnCommand.Cell.J].UpdateValue(turnCommand.Cell.Marker.Value);
        }
    }
}
