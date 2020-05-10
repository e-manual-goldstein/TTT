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
using LayoutParams = Android.Widget.FrameLayout.LayoutParams;

namespace TTT.Client
{
    public abstract class VMWithToolbar<TModel> : ViewModel<TModel> where TModel : class
    {
        public VMWithToolbar(TModel model) : base(model)
        {

        }

        protected FrameLayout DrawToolbar(Context context)
        {
            var toolbarLayout = new FrameLayout(context) { };
            toolbarLayout.Visibility = ViewStates.Visible;


            var retractedLayout = new LayoutParams((int)(DisplayMetrics.WidthPixels * 0.9), 200, GravityFlags.Center);
            var textBox = new TextView(context);

            RetractToolbar(toolbarLayout, textBox, retractedLayout);

            bool expanded = false;
            toolbarLayout.Click += (object sender, EventArgs eventArgs) =>
            {
                var expandedLayout = new LayoutParams((int)(DisplayMetrics.WidthPixels * 0.9), Constants.ClientToolbarExtentedSize, GravityFlags.Center);
                expanded = ToggleExpand(textBox, expanded, expandedLayout, retractedLayout);
            };

            toolbarLayout.AddView(textBox, 0);

            return toolbarLayout;
        }
        private bool ToggleExpand(TextView textBox, bool expanded, LayoutParams expandedLayout, LayoutParams retractedLayout)
        {
            var toolbarLayout = textBox.Parent as FrameLayout;
            if (!expanded)
            {
                ExpandToolbar(toolbarLayout, textBox, expandedLayout);
            }
            else
            {
                RetractToolbar(toolbarLayout, textBox, retractedLayout);
            };
            return !expanded;
        }

        private void RetractToolbar(FrameLayout toolbarLayout, TextView textBox, LayoutParams retractedLayout)
        {
            textBox.Text = "Logs";
            textBox.LayoutParameters = retractedLayout;
            textBox.Gravity = GravityFlags.Left | GravityFlags.CenterVertical;
            toolbarLayout.SetBackgroundColor(Color.Khaki);
            toolbarLayout.LayoutParameters = new LayoutParams(DisplayMetrics.WidthPixels, 200);
        }

        private void ExpandToolbar(FrameLayout toolbarLayout, TextView textBox, LayoutParams expandedLayout)
        {
            textBox.Text = Logger.ReadFromLog();
            textBox.LayoutParameters = expandedLayout;
            textBox.Gravity = GravityFlags.Left | GravityFlags.Bottom;
            toolbarLayout.SetBackgroundColor(Color.DarkSeaGreen);
            toolbarLayout.LayoutParameters = new LayoutParams(DisplayMetrics.WidthPixels, Constants.ClientToolbarExtentedSize);
        }
    }
}