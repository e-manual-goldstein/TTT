using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Views;
using Android.Widget;
using Microsoft.Extensions.DependencyInjection;
using TTT.Common;

namespace TTT.Client
{
    [Application(Name = "com.companyname.ttt.client.App", Theme = "@style/AppTheme.NoActionBar")]
    public class App : Application, Application.IActivityLifecycleCallbacks
    {
        #region Service Provider
        
        IServiceProvider _serviceProvider;
        ActivityManager _activityManager;
        ViewModelManager _viewModelManager;
        
        #endregion

        public App(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);
            _serviceProvider = serviceCollection.BuildServiceProvider();
        }

        public override void OnCreate()
        {
            base.OnCreate();
            RegisterActivityLifecycleCallbacks(this);
            
            _viewModelManager = _serviceProvider.GetService<ViewModelManager>();
            ConfigureViewModels();
            AttachSnackbarToLogger();

            _activityManager = _serviceProvider.GetService<ActivityManager>();
            _activityManager.RegisterAppContext(this);
            
            #region Start Game
            var mainMenu = _serviceProvider.GetService<MainMenu>();
            _viewModelManager.CreateViewModel(mainMenu);
            //var menuView = new MainMenuViewModel(this, mainMenu, _activityManager);
            
            //replace with Reconnect + GetGameState
               
            #endregion
        }

        private void AttachSnackbarToLogger()
        {
            _serviceProvider.GetService<Logger>().MessageReceived += (string msg) =>
            {
                //_activityManager.RunOnUiThread((view) => Snackbar.Make(view, msg, 5));
                var view = _activityManager.CurrentView();
                if (view != null)
                Snackbar.Make(view, msg, Snackbar.LengthLong)
                        .SetAction("Action", (Android.Views.View.IOnClickListener)null)
                        .Show();
            };
        }

        public override void OnTerminate()
        {
            base.OnTerminate();
            UnregisterActivityLifecycleCallbacks(this);
        }

        private void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<Logger>();
            services.AddSingleton<PlayerManager>();
            services.AddSingleton<ActivityManager>();
            services.AddSingleton<SocketManager>();
            services.AddSingleton<GameManager>();
            services.AddSingleton<MessageHandler>();
            services.AddSingleton<ExternalHostManager>();
            services.AddSingleton<ViewModelManager>();
            services.AddSingleton<ControllerManager>(provider =>
            {
                return new ControllerManager(provider, provider.GetService<Logger>(), new Type[]
                {
                    typeof(StateController),
                    typeof(PlayerController)
                });
            });
            services.AddSingleton<ActionService>();
            services.AddScoped<StateController>();
            services.AddScoped<PlayerController>();
            services.AddScoped<MainMenu>();
        }
       
        private void ConfigureViewModels()
        {
            _viewModelManager.RegisterViewModel<MainMenu, MainMenuViewModel>();
            _viewModelManager.RegisterViewModel<GameState, GameViewModel>();
        }

        public void OnActivityCreated(Activity activity, Bundle savedInstanceState)
        {
            _activityManager.RegisterActivity(activity);
        }

        public void OnActivityDestroyed(Activity activity)
        {
            _activityManager.UnRegisterActivity(activity);
        }

        public void OnActivityPaused(Activity activity)
        {
            
        }

        public void OnActivityResumed(Activity activity)
        {
            
        }

        public void OnActivitySaveInstanceState(Activity activity, Bundle outState)
        {
            //throw new NotImplementedException();
        }

        public void OnActivityStarted(Activity activity)
        {
            _activityManager.LoadViewForActivity(activity);
        }

        public void OnActivityStopped(Activity activity)
        {
            //Unload?
        }
    }
}