using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Views;
using Android.Widget;
using TTT.Common;

namespace TTT.Client
{
    public class ActivityManager
    {
        Logger _logger;
        Context _appContext;
        Dictionary<Type, FrameLayout> _activityLookup = new Dictionary<Type, FrameLayout>();
        Context _currentContext;
        View _currentView;

        public ActivityManager(Logger logger)
        {
            _logger = logger;
        }

        #region Register App Context

        public void RegisterAppContext(Context appContext)
        {
            if (_appContext != null)
                throw new Exception("Cannot register more than one App Context");
            _appContext = _currentContext = appContext;
            _logger.Log("Registered App Context");
        }

        #endregion

        #region Register Activity

        public void RegisterActivity(Activity activity)
        {
            RegisterActivity(activity.GetType());
            _logger.Log($"Registered Activity: {activity.LocalClassName}");
        }

        private void RegisterActivity(Type activityType)
        {
            ValidateActivityType(activityType);
            if (!_activityLookup.ContainsKey(activityType))
                _activityLookup[activityType] = null;
        }

        #endregion

        #region UnRegister Activity

        public void UnRegisterActivity(Activity activity)
        {
            UnRegisterActivity(activity.GetType());
            _logger.Log($"UnRegistered Activity: {activity.LocalClassName}");
        }

        private void UnRegisterActivity(Type activityType)
        {
            ValidateActivityType(activityType);
            _activityLookup.Remove(activityType);
        }

        #endregion

        #region Set View

        public void SetActivityView(Activity activity, FrameLayout layout)
        {
            SetActivityView(activity.GetType(), layout);
        }

        public void SetActivityView(Type activityType, FrameLayout layout)
        {
            ValidateActivityType(activityType);
            _activityLookup[activityType] = layout;
            _logger.Log($"Set View For Activity: {activityType.Name}");
        }

        internal void RunOnUiThread(Action<View> action)
        {
            
        }

        internal View CurrentView()
        {
            return _currentView;
        }

        #endregion

        #region Load View

        public void LoadViewForActivity(Activity activity)
        {
            var view = LoadViewForActivityType(activity.GetType());
            activity.SetContentView(view);
            SetCurrentView(view);
            SetCurrentActivity(activity);
        }

        public View LoadViewForActivityType(Type activityType)
        {
            if (!typeof(Activity).IsAssignableFrom(activityType))
                throw new ArgumentException("Cannot load view for non-Activity types");
            return _activityLookup[activityType];
        }

        private void SetCurrentActivity(Activity activity)
        {
            _currentContext = activity;
        }

        private void SetCurrentView(View view)
        {
            _currentView = view;
        }

        #endregion

        public void StartNewActivity(Type type, bool useNewTask = false)
        {
            ValidateActivityType(type);
            _logger.Log($"Starting New Activity: {type.Name}");
            SetCurrentView(_activityLookup[type]);
            if (useNewTask)
            {
                var intent = new Intent(_appContext, type);
                intent.AddFlags(ActivityFlags.NewTask);
                _appContext.StartActivity(intent);
            }
            else 
                _currentContext.StartActivity(new Intent(_currentContext, type));
        }

        #region Context

        public Context CurrentContext()
        {
            return _currentContext;
        }

        #endregion

        public void AddToActivityView(Type activityType, View childView)
        {
            ValidateActivityType(activityType);
            if (!_activityLookup.ContainsKey(activityType))
                _activityLookup[activityType].AddView(childView);
            else
                _logger.Error($"View not found for activity: {activityType}");
        }

        private void ValidateActivityType(Type type)
        {
            if (!typeof(Activity).IsAssignableFrom(type))
                throw new ArgumentException("Invalid Activity type");
        }
    }
}