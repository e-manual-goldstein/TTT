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

namespace TTT.Client
{
    public class GameManager
    {
        GameGrid _game;
        IServiceProvider _serviceProvider;
        ActivityManager _activityManager;
        

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

        public bool GameIsInProgress()
        {
            return _game != null;
        }
    }
}