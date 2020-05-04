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
using Android.Views;
using Android.Widget;
using Microsoft.Extensions.DependencyInjection;
using TTT.Client.Views;
using TTT.Common;

namespace TTT.Client
{
    [Application(Name = "com.companyname.ttt.client.App", Theme = "@style/AppTheme.NoActionBar")]
    public class App : Application, Application.IActivityLifecycleCallbacks
    {
        #region App Instance

        //TODO: To be removed
        //static Lazy<App> _app = new Lazy<App>();
        ////TODO: To be removed
        //public static App Current { get => _app.Value; }

        #endregion

        #region Service Provider
        
        IServiceProvider _serviceProvider;
        ActivityManager _activityManager;
        
        #endregion

        //TODO: To be removed
        //public App()
        //{
        //    var serviceCollection = new ServiceCollection();
        //    ConfigureServices(serviceCollection);
        //    _serviceProvider = serviceCollection.BuildServiceProvider();
        //}

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
            _activityManager = _serviceProvider.GetService<ActivityManager>();
            _activityManager.RegisterAppContext(this);
            #region Start Game
            var mainMenu = _serviceProvider.GetService<MainMenu>();
            var menuView = new MainMenuView(this, mainMenu, _activityManager);
            
            //replace with Reconnect + GetGameState
                //AddGameGrid(new GameState(Guid.NewGuid(), null));
            #endregion
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
            services.AddSingleton<ControllerManager>(provider =>
            {
                return new ControllerManager(provider, provider.GetService<Logger>(), new Type[]
                {
                    typeof(StateController)
                });
            });
            services.AddSingleton<ActionService>();
            services.AddScoped<StateController>();
            services.AddScoped<MainMenu>();
            services.AddScoped<GameGrid>();
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