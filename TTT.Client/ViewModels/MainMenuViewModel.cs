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
    public class MainMenuViewModel : VMWithToolbar<MainMenu>
    {
        public MainMenuViewModel(MainMenu mainMenu) : base(mainMenu)
        {

        }

        protected override void Draw()
        {
            var context = ActivityManager.CurrentContext();
            var layout = new FrameLayout(context);
            layout.AddView(AddButtons(context, Model.CreateActionDictionary()), 0);
            layout.AddView(DrawToolbar(context),1);
            ActivityManager.SetActivityView(typeof(MenuActivity), layout);
        }

        private FrameLayout AddButtons(Context context, IDictionary<string, EventHandler> callbackDictionary)
        {
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
                frameLayout.AddView(button, 0);
                n++;
            }
            return frameLayout;
        }

    }
}