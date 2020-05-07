using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using System.Windows;
using System.Windows.Controls;
using TTT.Common;
using TTT.Host.Api;

namespace TTT.Host
{
    public class MainMenu
    {
        private readonly Dictionary<string, Action> _menuActions;
        GameManager _gameManager;
        ISocketHub _socketHub;
        ViewManager _viewManager;
        ExternalConnectionManager _externalConnectionManager;

        public MainMenu(GameManager gameManager, ISocketHub socketHub, ViewManager viewManager, ExternalConnectionManager externalConnectionManager)
        {
            _menuActions = new Dictionary<string, Action>();
            _gameManager = gameManager;
            _socketHub = socketHub;
            _viewManager = viewManager;
            _externalConnectionManager = externalConnectionManager;
        }

        public void CreateActions()
        {
            _menuActions["Connect"] = async () => await Connect();
            _menuActions["Connect Remote"] = async () => await ConnectRemote();
            _menuActions["Start"] = () => Start();
            _menuActions["Test"] = async () => await Test();
            _menuActions["Terminate"] = async () => await _externalConnectionManager.TerminateAllActive();
        }

        public Dictionary<string, Action> MenuActions()
        {
            return _menuActions;
        }

        public async Task Connect()
        {
            var game = _gameManager.CurrentGame();
            var playerId = await _socketHub.ConnectAsync();
            game.AddPlayerToGame(playerId);
            //await socketHub.OpenConnectionAsync(playerId);
        }

        public async Task ConnectRemote()
        {
            _gameManager.CreateNewGame();
            await Task.Run(() =>
            {
                var game = _gameManager.CurrentGame();
                Task.WaitAll(
                    _externalConnectionManager.OpenExternalSocket(Guid.NewGuid()),
                    _socketHub.BeginListening().ContinueWith(task =>
                    {
                        var playerId = task.Result;
                        var gameCommand = new GameCommand(new AssignPlayerCommand() { PlayerId = playerId });
                        _socketHub.SendCommand(playerId, gameCommand);
                        game.AddPlayerToGame(task.Result);
                    })
                );;
            });
            
        }

        public void Start()
        {
            var game = _gameManager.CurrentGame();
            game.StartRandomPlayer();
            //game.StartAtEndGame();
            var gameState = game.GetCurrentState();
            var subCommand = new UpdateStateCommand(gameState, true);
            _socketHub.BroadcastCommand(new GameCommand(subCommand));
            _gameManager.StartGame();
            
        }

        public async Task Test()
        {
            var game = _gameManager.CurrentGame();
            var playerId = await _socketHub.ConnectAsync(true);
            game.AddPlayerToGame(playerId);
        }
    }
}
