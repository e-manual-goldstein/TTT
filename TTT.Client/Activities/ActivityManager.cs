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
        Context _appContext;
        Dictionary<Type, View> _activityLookup = new Dictionary<Type, View>();
        Context _currentContext;

        public ActivityManager(Logger logger)
        {
            _logger = logger;
        }

        #region Register App Context

        public void RegisterAppContext(Context appContext)
        {
            _appContext = appContext;
        }

        #endregion

        #region Register Activity

        public void RegisterActivity(Activity activity)
        {
            RegisterActivity(activity.GetType());
        }

        private void RegisterActivity(Type activityType)
        {
            if (!typeof(Activity).IsAssignableFrom(activityType))
                throw new ArgumentException("Cannot register non-Activity types");
            if (!_activityLookup.ContainsKey(activityType))
                _activityLookup[activityType] = null;
        }

        #endregion

        #region UnRegister Activity

        public void UnRegisterActivity(Activity activity)
        {
            UnRegisterActivity(activity.GetType());
        }

        private void UnRegisterActivity(Type activityType)
        {
            if (!typeof(Activity).IsAssignableFrom(activityType))
                throw new ArgumentException("Cannot unregister view for non-Activity types");
            _activityLookup.Remove(activityType);
        }

        #endregion

        #region Set View

        public void SetActivityView(Activity activity, View view)
        {
            SetActivityView(activity.GetType(), view);
        }

        public void SetActivityView(Type activityType, View view)
        {
            if (!typeof(Activity).IsAssignableFrom(activityType))
                throw new ArgumentException("Cannot set view for non-Activity types");
            _activityLookup[activityType] = view;
        }

        #endregion

        #region Load View

        public void LoadViewForActivity(Activity activity)
        {
            activity.SetContentView(LoadViewForActivityType(activity.GetType()));
            ShowActivity(activity);
        }

        private void ShowActivity(Activity activity)
        {
            _currentContext = activity;
        }

        public void StartNewActivity(Type type, bool useNewTask = false)
        {
            if (!typeof(Activity).IsAssignableFrom(type))
                throw new ArgumentException("Invalid Activity type");
            if (useNewTask)
            {
                var intent = new Intent(_appContext, type);
                intent.AddFlags(ActivityFlags.NewTask);
                _appContext.StartActivity(intent);
            }
            else 
                _currentContext.StartActivity(new Intent(_currentContext, type));
        }

        public View LoadViewForActivityType(Type activityType)
        {
            if (!typeof(Activity).IsAssignableFrom(activityType))
                throw new ArgumentException("Cannot load view for non-Activity types");
            return _activityLookup[activityType];
        }

        #endregion

    }
}