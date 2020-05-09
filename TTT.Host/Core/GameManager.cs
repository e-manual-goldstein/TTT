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
        IServiceProvider _serviceProvider;
        Logger _logger;

        public GameManager(ViewManager viewManager, ISocketHub socketHub, Logger logger, IServiceProvider serviceProvider)
        {
            _socketHub = socketHub;
            _viewManager = viewManager;
            _logger = logger;
            _serviceProvider = serviceProvider;
        }
        public Game CreateNewGame()
        {
            var gameId = Guid.NewGuid();
            _currentGame = new Game(gameId, TurnTaken, EndGame);
            _logger.Debug("Created New Game");            
            return _currentGame;
        }

        public Game StartGame()
        {
            var gameView = new GameView(_currentGame, _serviceProvider);
            _viewManager.SetContent(gameView);
            _viewManager.Show();
            _viewManager.Update();
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
            _socketHub.BroadcastCommand(new GameCommand(subCommand, Guid.NewGuid()));
            _viewManager.Update();
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
