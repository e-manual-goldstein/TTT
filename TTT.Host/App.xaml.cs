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
            services.AddSingleton<ViewManager>();
            services.AddSingleton<GameManager>();
            services.AddSingleton<MainWindow>();
            services.AddSingleton<ISocketHub, SocketHub>();
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
            var socketHub = _serviceProvider.GetService<ISocketHub>();
            var mainWindow = _serviceProvider.GetService<MainWindow>();
            var gameManager = _serviceProvider.GetService<GameManager>();
            var game = gameManager.StartNewGame();

            #region Menu 

            mainWindow.ConnectButtonAction = async () =>
            {
                var playerId = await socketHub.ConnectAsync();
                game.AddPlayerToGame(playerId);
                //await socketHub.OpenConnectionAsync(playerId);
            };

            mainWindow.StartButtonAction = () =>
            {
                game.StartRandomPlayer();
                game.StartAtEndGame();
                var gameState = game.GetCurrentState();
                var subCommand = new UpdateStateCommand(gameState, true);
                socketHub.BroadcastCommand(new GameCommand(subCommand));
            };

            mainWindow.TestButtonAction = async () =>
            {
                var playerId = await socketHub.ConnectAsync(true);
                game.AddPlayerToGame(playerId);
            };

            #endregion

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
