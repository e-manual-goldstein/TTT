using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
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
        ActivityManager _activityManager;
        ViewModelManager _viewModelManager;
        Logger _logger;

        public GameManager(Logger logger, ActivityManager activityManager, ViewModelManager viewModelManager)
        {
            _activityManager = activityManager;
            _viewModelManager = viewModelManager;
            _logger = logger;
        }

        public void LoadGameState(GameState gameState, bool isNewGame = false)
        {
            _viewModelManager.CreateViewModel(gameState);
            if (isNewGame)
            {
                _activityManager.StartNewActivity(typeof(GameActivity));
            }
            else
                _activityManager.LoadNewView(typeof(GameActivity));


        }
        
        public Cell[] CreateCells()
        {
            var cells = new List<Cell>();
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    var cell = new Cell(i, j);
                    cells.Add(cell);
                }
            }
            return cells.ToArray();
        }
    }
}