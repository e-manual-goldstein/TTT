using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using TTT.Common;

namespace TTT.Client
{
    public class GameGrid
    {
        Cell[] _allCells;
        PlayerManager _playerManager;
        float _screenWidth;
        float _screenHeight;


        public GameGrid(ActionService actionService, PlayerManager playerManager)
        {
            _allCells = CreateCells(actionService);
            _playerManager = playerManager;
        }

        public FrameLayout FrameLayout { get; set; }

        public Cell[] CreateCells(ActionService actionService)
        {
            var cells = new List<Cell>();
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    var cell = new Cell(i, j);
                    cell.SetTakeTurnAction((c) => actionService.TakeTurn(c));
                    cells.Add(cell);
                }
            }
            return cells.ToArray();
        }

        public void DrawCells(Activity context, GameState gameState)
        {
            var displayMetrics = context.Resources.DisplayMetrics;
            var baseLayout = new FrameLayout.LayoutParams(Constants.CellSizeClient, Constants.CellSizeClient);
            var baseX = (displayMetrics.WidthPixels - (3 * Constants.CellSizeClient)) / 2;
            var baseY = (displayMetrics.HeightPixels - (3 * Constants.CellSizeClient)) / 2;
            foreach (var cell in _allCells)
            {
                var button = new Button(context);
                button.LayoutParameters = baseLayout;
                var x = baseX + (cell.I * Constants.CellSizeClient);
                var y = baseY + (cell.J * Constants.CellSizeClient);
                button.SetX(x);
                button.SetY(y);
                button.SetBackgroundColor(cell.Active ? Color.Red : Color.Gray);
                button.SetTextColor(Color.White);
                button.SetTextSize(Android.Util.ComplexUnitType.Px, 50);
                button.Text = gameState == null ? null : GetCellFromState(cell, gameState).ToString();
                if (_playerManager.IsMyTurn(gameState))
                    button.Click += cell.ClickCell;
                FrameLayout.AddView(button);
            }
        }

        private Marker? GetCellFromState(Cell cell, GameState gameState)
        {
            return gameState.Cells[cell.I, cell.J].Marker;
        }
    }   
}