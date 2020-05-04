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

namespace TTT.Client.Views
{
    public class MainMenuView
    {
        private Context _menuContext;
        private ActivityManager _activityManager;
        public MainMenuView(Context menuContext, MainMenu mainMenu, ActivityManager activityManager)
        {
            _menuContext = menuContext;
            _activityManager = activityManager;
            AddButtons(mainMenu.CreateActionDictionary());
        }

        private void AddButtons(IDictionary<string, EventHandler> callbackDictionary)
        {
            var metrics = _menuContext.Resources.DisplayMetrics;
            var width = metrics.WidthPixels;
            var height = metrics.HeightPixels;
            var baseLayout = new FrameLayout.LayoutParams(Constants.CellSizeClient, Constants.CellSizeClient);
            var baseX = (width - Constants.CellSizeClient) / 2;
            var frameLayout = new FrameLayout(_menuContext);
            int n = 1;
            foreach (var item in callbackDictionary)
            {
                var baseY = (n * (height - Constants.CellSizeClient)) / (callbackDictionary.Count + 1);
                var button = new Button(_menuContext);
                button.LayoutParameters = baseLayout;
                button.SetX(baseX);
                button.SetY(baseY);
                button.SetBackgroundColor(Color.Gray);
                button.SetTextColor(Color.White);
                button.SetTextSize(Android.Util.ComplexUnitType.Px, 50);
                button.Text = item.Key;
                button.Click += item.Value;
                frameLayout.AddView(button);
                n++;
            }
            _activityManager.SetActivityView(typeof(MenuActivity), frameLayout);
        }

    }
}