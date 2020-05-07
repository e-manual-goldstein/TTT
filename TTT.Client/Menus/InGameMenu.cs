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

namespace TTT.Client.Menus
{
    public class InGameMenu
    {
        ActivityManager _activityManager;

        public InGameMenu(ActivityManager activityManager)
        {
            _activityManager = activityManager;
        }

        [MenuAction]
        public void QuitGame()
        {
            _activityManager.StartNewActivity(typeof(MenuActivity));
        }
    }
}