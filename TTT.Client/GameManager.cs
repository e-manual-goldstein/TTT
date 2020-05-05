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
        IServiceProvider _serviceProvider;
        ActivityManager _activityManager;
        ActionService _actionService;
        GameState _gameState;

        public GameManager(IServiceProvider serviceProvider, ActivityManager activityManager, ActionService actionService)
        {
            _serviceProvider = serviceProvider;
            _activityManager = activityManager;
            _actionService = actionService;
        }

        public void LoadGame(GameState gameState, bool isNewGame = false)
        {
            _gameState = gameState;
            //if (isNewGame)
            //{
            //    _game = _serviceProvider.GetService<GameGrid>();
            //}
            //var currentActivity = _activityManager.GetCurrentActivity();
            //_game.FrameLayout = new FrameLayout(currentActivity);
            //_game.DrawCells(currentActivity, gameState);
            //currentActivity.RunOnUiThread(() => currentActivity.SetContentView(_game.FrameLayout));
        }
        
        public Cell[] CreateCells()
        {
            var cells = new List<Cell>();
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    var cell = new Cell(i, j);
                    cell.SetTakeTurnAction((c) => _actionService.TakeTurn(c));
                    cells.Add(cell);
                }
            }
            return cells.ToArray();
        }
    }
}