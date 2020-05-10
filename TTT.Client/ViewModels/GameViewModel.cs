using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.View.Menu;
using Android.Views;
using Android.Widget;
using TTT.Client.Menus;
using TTT.Common;
using WebSocket4Net.Command;
using LayoutParams = Android.Widget.FrameLayout.LayoutParams;

namespace TTT.Client
{
    public class GameViewModel : VMWithToolbar<GameState>
    {
        public GameViewModel(GameState game) : base(game)
        {
            
        }

        protected override void Draw()
        {
            var context = ActivityManager.CurrentContext();
            var layout = CreateGameLayout(context, Model);
            layout.AddView(DrawToolbar(context), 0);
            ActivityManager.SetActivityView(typeof(GameActivity), layout);
        }

        private FrameLayout CreateGameLayout(Context context, GameState gameState)
        {
            var layout = new FrameLayout(context);
            DrawCells(context, gameState, layout);
            return layout;
        }

        private void DrawCells(Context context, GameState gameState, FrameLayout layout)
        {
            var baseLayout = new LayoutParams(Constants.CellSizeClient, Constants.CellSizeClient);
            var baseX = (DisplayMetrics.WidthPixels - (3 * Constants.CellSizeClient)) / 2;
            var baseY = (DisplayMetrics.HeightPixels - (3 * Constants.CellSizeClient)) / 2;
            var backsquare = new FrameLayout(context);
            backsquare.SetBackgroundColor(Color.Gray);
            backsquare.LayoutParameters = new LayoutParams(6 + (Constants.CellSizeClient * 3), 6 + (Constants.CellSizeClient * 3));
            backsquare.SetX(baseX);
            backsquare.SetY(baseY);
            layout.AddView(backsquare, 0);
            foreach (var cell in Model.Cells)
            {
                var button = new Button(context);
                button.LayoutParameters = baseLayout;
                var x = baseX + (cell.I * (Constants.CellSizeClient + 3));
                var y = baseY + (cell.J * (Constants.CellSizeClient + 3));
                button.SetX(x);
                button.SetY(y);
                button.SetBackgroundColor(GetCellFromState(cell, gameState).Active ? Color.Red : Color.White);
                button.SetTextColor(Color.Black);
                button.SetTextSize(Android.Util.ComplexUnitType.Px, 50);
                button.Text = gameState == null ? null : GetCellFromState(cell, gameState).Marker.ToString();
                if (GetService<PlayerManager>().IsMyTurn(gameState))
                    button.Click += (object sender, EventArgs eventArgs) => ClickCell(cell);
                layout.AddView(button, 1);
            }
        }

        private void ClickCell(Cell cell)
        {
            if (cell.Marker == null)
            {
                GetService<ActionService>().TakeTurn(cell);
            }
        }

        private Cell GetCellFromState(Cell cell, GameState gameState)
        {
            return gameState.Cells.Single(c => c.I == cell.I && c.J == cell.J);
        }
    }
}