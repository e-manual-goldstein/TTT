﻿using System;
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

        public MainMenu(ExternalHostManager externalHostManager, SocketManager socketManager, PlayerManager playerManager, ActivityManager activityManager, GameManager gameManager)
        {
            _externalHostManager = externalHostManager;
            _socketManager = socketManager;
            _playerManager = playerManager;
            _activityManager = activityManager;
            _gameManager = gameManager;
        }

        private void Connect(object sender, EventArgs e)
        {
            RunOnUiThread(async () => {
                var ipEndpoint = await _externalHostManager.FindOnlineGame();
                _socketManager.CreateSocket(ipEndpoint);
            });
        }

        private void RunOnUiThread(Action action)
        {
            //_serviceProvider.GetService<ActivityManager>()
            //    .GetCurrentActivity().RunOnUiThread(action);
        }

        private void EventListener(object sender, EventArgs e)
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

        private void Test(object sender, EventArgs e)
        {
            var game = _gameManager.GetGame();
            _activityManager.StartNewActivity(typeof(GameActivity));
        }
        
        public Dictionary<string, EventHandler> CreateActionDictionary()
        {
            return new Dictionary<string, EventHandler>()
            {
                { "LISTEN", new EventHandler(EventListener) },
                { "TEST", new EventHandler(Test) },
                { "CONNECT", new EventHandler(Connect) }
            };
        }
    }
}