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
    public class StateController : AbstractController
    {
        GameManager _gameManager;

        public StateController(Logger logger, GameManager gameManager) : base(logger)
        {
            _gameManager = gameManager;
        }

        public void UpdateState(UpdateStateCommand updateStateCommand)
        {
            _gameManager.LoadGameState(updateStateCommand.GameState, updateStateCommand.IsNewGame);
        }
    }
}