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
    public class PlayerController : AbstractController
    {
        PlayerManager _playerManager;
        public PlayerController(Logger logger, PlayerManager playerManager) : base(logger)
        {
            _playerManager = playerManager;
        }

        public void AssignPlayerId(AssignPlayerCommand assignPlayerCommand)
        {
            _logger.Debug($"Player ID Assigned: {assignPlayerCommand.PlayerId}");
            _playerManager.SetPlayerId(assignPlayerCommand.PlayerId);
        }
    }
}