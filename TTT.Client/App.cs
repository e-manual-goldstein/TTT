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
using TTT.Common;

namespace TTT.Client
{
    [Application(Name = "com.companyname.ttt.client.App", Theme = "@style/AppTheme.NoActionBar")]
    public class App : Application
    {
        #region App Instance

        //TODO: To be removed
        static Lazy<App> _app = new Lazy<App>();
        //TODO: To be removed
        public static App Current { get => _app.Value; }

        #endregion

        #region Service Provider
        
        IServiceProvider _serviceProvider;
        public IServiceProvider ServiceProvider => _serviceProvider;

        #endregion

        //TODO: To be removed
        public App()
        {
            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);
            _serviceProvider = serviceCollection.BuildServiceProvider();
        }

        public App(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);
            _serviceProvider = serviceCollection.BuildServiceProvider();
        }

        public override void OnCreate()
        {
            base.OnCreate();
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
            services.AddScoped<GameGrid>();
        }
        
        //TODO: Is this supposed to be here?
        internal void RegisterMainActivity(MainActivity mainActivity)
        {
            _serviceProvider.GetService<ActivityManager>().LoadActivity(mainActivity);
        }
    }
}