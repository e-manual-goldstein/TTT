using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using Microsoft.Extensions.DependencyInjection;
using TTT.Common;

namespace TTT.Client
{
    public class GameManager
    {
        GameGrid _game;
        IServiceProvider _serviceProvider;
        ActivityManager _activityManager;
        GameState _gameState;

        public GameManager(IServiceProvider serviceProvider, ActivityManager activityManager)
        {
            _serviceProvider = serviceProvider;
            _activityManager = activityManager;
        }

        public GameGrid GetGame()
        {
            if (_game != null)
            {
                return _game;
            }
            return _game = new GameGrid(_serviceProvider.GetService<ActionService>());
        }

        public void LoadGame(GameState gameState, bool isNewGame = false)
        {
            _gameState = gameState;
            if (isNewGame)
            {
                _game = new GameGrid(_serviceProvider.GetService<ActionService>());
            }
            var currentActivity = _activityManager.GetCurrentActivity();
            _game.FrameLayout = new FrameLayout(currentActivity);
            _game.DrawCells(currentActivity, gameState);
            currentActivity.RunOnUiThread(() => currentActivity.SetContentView(_game.FrameLayout));
        }

        public bool GameIsInProgress()
        {
            return _game != null;
        }

    }
}