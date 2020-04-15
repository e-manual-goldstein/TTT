using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using TTT.Common;

namespace TTT.Client
{
    public class PlayerManager
    {
        Guid _playerId;
        Logger _logger;

        public PlayerManager(Logger logger)
        {
            _logger = logger;
        }

        public void SetPlayerId(Guid playerId)
        {
            _playerId = playerId;
        }

        public Guid GetPlayerId()
        {
            return _playerId;
        }

        public bool IsMyTurn(GameState gameState)
        {
            return gameState.CurrentPlayerId == _playerId;
        }
    }
}