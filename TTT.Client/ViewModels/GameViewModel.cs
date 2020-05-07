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

namespace TTT.Client
{
    public class GameViewModel : ViewModel<GameState>
    {

        public GameViewModel(GameState game) : base(game) { }

        protected override void Draw()
        {
            var layout = CreateGameLayout(Model);
            ActivityManager.SetActivityView(typeof(GameActivity), layout);
        }

        private FrameLayout CreateGameLayout(GameState gameState)
        {
            var context = ActivityManager.CurrentContext();
            var layout = new FrameLayout(context);
            DrawCells(context, gameState, layout);
            DrawToolbar(context, layout);
            return layout;
        }

        private void DrawCells(Context context, GameState gameState, FrameLayout layout)
        {
            var baseLayout = new FrameLayout.LayoutParams(Constants.CellSizeClient, Constants.CellSizeClient);
            var baseX = (DisplayMetrics.WidthPixels - (3 * Constants.CellSizeClient)) / 2;
            var baseY = (DisplayMetrics.HeightPixels - (3 * Constants.CellSizeClient)) / 2;
            foreach (var cell in Model.Cells)
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
                if (GetService<PlayerManager>().IsMyTurn(gameState))
                    button.Click += cell.ClickCell;
                layout.AddView(button);
            }
        }

        private void DrawToolbar(Context context, FrameLayout layout)
        {
            var button = new Button(context) { };
            button.LayoutParameters = new FrameLayout.LayoutParams(DisplayMetrics.WidthPixels, 200);
            button.SetBackgroundColor(Color.Khaki);
            button.Text = "Logs";
            button.TextAlignment = TextAlignment.ViewStart;
            //var menu = new InGameMenu(ActivityManager);
            //foreach (var actionMethod in menu.GetType().GetMenuActions())
            //{
            //    //var toolbarItem = new ToolbarItem();
            //    //toolbarItem.Text = actionMethod.Key;
            //    //toolbarItem.Clicked += (object sender, EventArgs e) =>
            //    //{
            //    //    actionMethod.Value.Invoke(menu, new object[] { });
            //    //};
            //    //toolbar.AddView(toolbarItem);
            //}
            //toolbar.Visibility = ViewStates.Visible;
            //layout.AddView(toolbar, 0);
            bool expanded = false;
            button.Click += (object sender, EventArgs eventArgs) =>
            {
                expanded = ToggleExpand(sender as Button, expanded);
            };
        }

        private bool ToggleExpand(Button button, bool expanded)
        {
            if (!expanded)
            {
                button.Text = Logger.ReadFromLog(10);
                button.LayoutParameters = new FrameLayout.LayoutParams(DisplayMetrics.WidthPixels, 800);
            }
            else 
            {
                button.Text = "Logs";
                button.LayoutParameters = new FrameLayout.LayoutParams(DisplayMetrics.WidthPixels, 200);
            };
            return !expanded;
        }

        private Cell GetCellFromState(Cell cell, GameState gameState)
        {
            return gameState.Cells.Single(c => c.I == cell.I && c.J == cell.J);
        }
    }
}