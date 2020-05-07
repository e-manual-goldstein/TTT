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
        ViewModelManager _viewModelManager;
        GameState _gameState;

        public GameManager(IServiceProvider serviceProvider, ActivityManager activityManager, ActionService actionService, ViewModelManager viewModelManager)
        {
            _serviceProvider = serviceProvider;
            _activityManager = activityManager;
            _actionService = actionService;
            _viewModelManager = viewModelManager;
        }

        public void LoadGameState(GameState gameState, bool isNewGame = false)
        {
            _viewModelManager.CreateViewModel(gameState);
            if (isNewGame)
            {
                _activityManager.StartNewActivity(typeof(GameActivity));
            }
            
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