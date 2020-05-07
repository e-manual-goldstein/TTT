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
    public class MainMenuViewModel : ViewModel<MainMenu>
    {
        public MainMenuViewModel(MainMenu mainMenu) : base(mainMenu)
        {

        }

        protected override void Draw()
        {
            AddButtons(Model.CreateActionDictionary());
        }

        private void AddButtons(IDictionary<string, EventHandler> callbackDictionary)
        {
            var context = ActivityManager.CurrentContext();
            var width = DisplayMetrics.WidthPixels;
            var height = DisplayMetrics.HeightPixels;
            var baseLayout = new FrameLayout.LayoutParams(Constants.CellSizeClient, Constants.CellSizeClient);
            var baseX = (width - Constants.CellSizeClient) / 2;
            var frameLayout = new FrameLayout(context);
            int n = 1;
            foreach (var item in callbackDictionary)
            {
                var baseY = (n * (height - Constants.CellSizeClient)) / (callbackDictionary.Count + 1);
                var button = new Button(context);
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
            ActivityManager.SetActivityView(typeof(MenuActivity), frameLayout);
        }

    }
}