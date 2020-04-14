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
    public class ActivityManager
    {
        Logger _logger;
        Activity[] _activities = new Activity[0];
        Activity _currentActivity;

        public ActivityManager(Logger logger)
        {
            _logger = logger;
        }

        public void RegisterActivity(Activity activity)
        {
            _activities = _activities.ToList().Append(activity).ToArray();
        }

        public void LoadActivity(Activity activity)
        {
            if (!_activities.Contains(activity))
                RegisterActivity(activity);
            _currentActivity = activity;
        }

        public Activity GetCurrentActivity()
        {
            return _currentActivity;
        }
    }
}