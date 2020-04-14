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
using TTT.Core;
using TTT.Host.Control;

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
            services.AddSingleton<GameGrid>();
            services.AddSingleton<MainWindow>();
            services.AddSingleton<SocketHub>();
            services.AddSingleton<MessageHandler>();
            services.AddSingleton<Logger>();
            services.AddSingleton<ControllerManager>(provider =>
            {
                return new ControllerManager(provider, provider.GetService<Logger>(), new Type[]
                {
                    typeof(GameController)
                });
            });
            services.AddScoped<GameController>();
        }

        private void OnStartup(object sender, StartupEventArgs e)
        {
            var socketHub = _serviceProvider.GetService<SocketHub>();
            var mainWindow = _serviceProvider.GetService<MainWindow>();
            var game = _serviceProvider.GetService<GameGrid>();
                
            mainWindow.ConnectButtonAction = async () =>
            {
                var playerId = await socketHub.ConnectAsync();
                game.AddPlayerToGame(playerId);
                await socketHub.OpenConnectionAsync(playerId);
            };

            mainWindow.StartButtonAction = () =>
            {
                game.StartRandomPlayer();
                var gameState = game.GetCurrentState();
                var subCommand = new UpdateStateCommand(gameState, true);
                socketHub.BroadcastCommand(new GameCommand(subCommand));
            };

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
