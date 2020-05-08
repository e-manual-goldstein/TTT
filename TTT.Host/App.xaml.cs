using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Windows;
using TTT.Common;
using TTT.Host.Api;
using TTT.Host.Control;
using TTT.Host.Events;

namespace TTT.Host
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private readonly ServiceProvider _serviceProvider;

        private void ConfigureServices(IServiceCollection services)
        {
            #region Base Services
            services.AddSingleton<Logger>();
            services.AddSingleton<CommandManager>();
            services.AddSingleton<MainWindow>();
            services.AddTransient<PortManager>();
            #endregion

            services.AddSingleton<ViewManager>();
            services.AddSingleton<GameManager>();
            services.AddSingleton<MainMenu>();
            services.AddSingleton<ExternalConnectionManager>();
            services.AddSingleton<ControllerManager>(provider =>
            {
                return new ControllerManager(provider, provider.GetService<Logger>(), new Type[]
                {
                    typeof(GameController)
                });
            });
            services.AddSingleton<ISocketHub, SocketHub>();

            services.AddScoped<GameController>();
        }

        private void OnStartup(object sender, StartupEventArgs e)
        {
            var mainWindow = _serviceProvider.GetService<MainWindow>();
            var menu = _serviceProvider.GetService<MainMenu>();
            menu.CreateActions();
            var menuView = new MainMenuView(menu);
            _serviceProvider.GetService<ViewManager>().SetContent(menuView);
            menuView.Show();
            mainWindow.Show();
        }

        public App()
        {
            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);
            _serviceProvider = serviceCollection.BuildServiceProvider();
        }
    }
}
