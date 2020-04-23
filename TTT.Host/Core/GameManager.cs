using System;
using System.Collections.Generic;
using System.Text;
using TTT.Common;
using TTT.Host.Api;
using TTT.Host.Events;

namespace TTT.Host
{
    public class GameManager
    {
        ISocketHub _socketHub;
        Game _currentGame;
        ViewManager _viewManager;

        public GameManager(ViewManager viewManager, ISocketHub socketHub)
        {
            _socketHub = socketHub;
            _viewManager = viewManager;
        }

        public Game StartNewGame()
        {
            var gameId = Guid.NewGuid();
            _currentGame = new Game(gameId, TurnTaken, EndGame);
            var canvas = _currentGame.DrawCells();
            _viewManager.SetContent(new GameView() { Content = canvas });
            _viewManager.AddButtons(canvas);

            return _currentGame;
        }

        public Game CurrentGame() 
        {
            return _currentGame;
        }

        private void TurnTaken(object sender, EventArgs eventArgs)
        {
            var game = sender as Game;
            var gameState = game.GetCurrentState();
            var subCommand = new UpdateStateCommand(gameState, false);
            _socketHub.BroadcastCommand(new GameCommand(subCommand));
        }

        private void EndGame(object sender, EndGameEventArgs eventArgs)
        {
            foreach (var cell in eventArgs.WinningSet)
            {
                cell.Active = true;
            }
        }
    }
}
