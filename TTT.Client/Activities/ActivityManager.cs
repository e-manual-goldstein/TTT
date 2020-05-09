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
using Xamarin.Forms.Platform.Android;

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
            _logger.Debug("Registered App Context");
        }

        #endregion

        #region Register Activity

        public void RegisterActivity(Activity activity)
        {
            RegisterActivity(activity.GetType());
            _logger.Debug($"Registered Activity: {activity.LocalClassName}");
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
            _logger.Debug($"UnRegistered Activity: {activity.LocalClassName}");
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
            _logger.Debug($"Set View For Activity: {activityType.Name}");
        }

        internal void RunOnUiThread(Action action)
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
            _logger.Debug($"Loaded View for Activity: {activity.LocalClassName}");
            _logger.Debug($"Setting Content View: {view.ContentDescription}");
            activity.SetContentView(view);

            SetCurrentView(view);
            SetCurrentActivity(activity);
            ViewUpdatedAction = (View newView) =>
            {
                _logger.Debug($"Firing ViewUpdated Event");
                activity.RunOnUiThread(() =>
                {
                    _logger.Debug($"Running on UI Thread");
                    _logger.Debug($"Removing View {newView}");
                    newView.RemoveFromParent();
                    _logger.Debug($"Setting Content View for Activity: {activity.LocalClassName}");
                    activity.SetContentView(newView);
                    view.Dispose();
                });
            };
        }

        public View LoadViewForActivityType(Type activityType)
        {
            if (!typeof(Activity).IsAssignableFrom(activityType))
                throw new ArgumentException("Cannot load view for non-Activity types");
            _logger.Debug($"Loading View for Activity Type: {activityType.Name}");
            return _activityLookup[activityType];
        }

        public void LoadNewView(Type activityType)
        {
            ValidateActivityType(activityType);
            ViewUpdatedAction(_activityLookup[activityType]);
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

        #region View Updated Event

        private Action<View> ViewUpdatedAction;

        #endregion

        public void StartNewActivity(Type type, bool useNewTask = false)
        {
            ValidateActivityType(type);
            _logger.Debug($"Starting New Activity: {type.Name}");
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
                _logger.Warning($"View not found for activity: {activityType}");
        }

        private void ValidateActivityType(Type type)
        {
            if (!typeof(Activity).IsAssignableFrom(type))
                throw new ArgumentException("Invalid Activity type");
        }
    }
}