using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
    public class GameView
    {
        GameGrid _game;
        Context _gameContext;
        ActivityManager _activityManager;

        public GameView(Context gameContext, GameGrid game, ActivityManager activityManager)
        {
            _game = game;
            _gameContext = gameContext;
            _activityManager = activityManager;
        }

        public void AddGameGrid(GameState gameState)
        {
            var layout = DrawCells(_gameContext, gameState);
            _activityManager.SetActivityView(typeof(GameActivity), layout);
        }

        public FrameLayout DrawCells(Context context, GameState gameState)
        {
            var layout = new FrameLayout(context);
            var displayMetrics = context.Resources.DisplayMetrics;
            var baseLayout = new FrameLayout.LayoutParams(Constants.CellSizeClient, Constants.CellSizeClient);
            var baseX = (displayMetrics.WidthPixels - (3 * Constants.CellSizeClient)) / 2;
            var baseY = (displayMetrics.HeightPixels - (3 * Constants.CellSizeClient)) / 2;
            foreach (var cell in _game.AllCells)
            {
                var button = new Button(context);
                button.LayoutParameters = baseLayout;
                var x = baseX + (cell.I * Constants.CellSizeClient);
                var y = baseY + (cell.J * Constants.CellSizeClient);
                button.SetX(x);
                button.SetY(y);
                button.SetBackgroundColor(GetCellFromState(cell, gameState).Active ? Color.Red : Color.Gray);
                button.SetTextColor(Color.White);
                button.SetTextSize(Android.Util.ComplexUnitType.Px, 50);
                button.Text = gameState == null ? null : GetCellFromState(cell, gameState).Marker.ToString();
                if (_game.IsMyTurn(gameState))
                    button.Click += cell.ClickCell;
                layout.AddView(button);
            }
            return layout;
        }
        private Cell GetCellFromState(Cell cell, GameState gameState)
        {
            return gameState.Cells[cell.I, cell.J];
        }
    }
}