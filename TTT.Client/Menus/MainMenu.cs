using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using TTT.Common;

namespace TTT.Client
{
    public class MainMenu
    {
        ExternalHostManager _externalHostManager;
        SocketManager _socketManager;
        PlayerManager _playerManager;
        ActivityManager _activityManager;
        GameManager _gameManager;
        ViewModelManager _viewModelManager;
        Logger _logger;

        public MainMenu(ExternalHostManager externalHostManager, 
                        SocketManager socketManager, 
                        PlayerManager playerManager, 
                        ActivityManager activityManager, 
                        GameManager gameManager,
                        ViewModelManager viewModelManager,
                        Logger logger)
        {
            _externalHostManager = externalHostManager;
            _socketManager = socketManager;
            _playerManager = playerManager;
            _activityManager = activityManager;
            _gameManager = gameManager;
            _viewModelManager = viewModelManager;
            _logger = logger;
        }

        private void Connect(object sender, EventArgs e)
        {
            Task.Run(async () => {
                var ipEndpoint = await _externalHostManager.FindOnlineGame();
                _socketManager.CreateSocket(ipEndpoint);
            });
        }

        private void RunOnUiThread(Action action)
        {
            _activityManager.RunOnUiThread(action);
        }

        private void SayHello(object sender, EventArgs e)
        {
            _logger.Log("Hello");
        }

        private void ListenForGames(object sender, EventArgs e)
        {
            if (_socketManager.HostSocket == null || !_socketManager.HostSocket.IsOpen)
            {
                RunOnUiThread(async () => {
                    var playerId = await _socketManager.Listen();
                    _playerManager.SetPlayerId(playerId);
                });
            }
        }

        private void TestNonUIThread(object sender, EventArgs e)
        {
            if (_socketManager.HostSocket == null || !_socketManager.HostSocket.IsOpen)
            {
                Task.Run(async () => {
                    var playerId = await _socketManager.Listen();
                    _playerManager.SetPlayerId(playerId);
                });
            }
        }

        private void StartGame(object sender, EventArgs e)
        {
            //var game = _gameManager.GetGame();
            var gameView = _viewModelManager
                .CreateViewModel(new GameState(Guid.NewGuid(), _gameManager.CreateCells())
            );
            _activityManager.StartNewActivity(typeof(GameActivity));
        }
        
        public Dictionary<string, EventHandler> CreateActionDictionary()
        {
            return new Dictionary<string, EventHandler>()
            {
                { "HELLO", new EventHandler(SayHello) },
                { "START", new EventHandler(StartGame) },
                { "CONNECT", new EventHandler(Connect) }
            };
        }
    }
}