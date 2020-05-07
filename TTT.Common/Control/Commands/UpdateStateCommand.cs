using System;
using System.Collections.Generic;
using System.Text;

namespace TTT.Common
{
    public class UpdateStateCommand : ISubCommand
    {
        public UpdateStateCommand(GameState gameState, bool isNewGame)
        {
            GameState = gameState;
            IsNewGame = isNewGame;
        }

        public GameState GameState { get; private set; }

        public bool IsNewGame { get; private set; }
    }
}
