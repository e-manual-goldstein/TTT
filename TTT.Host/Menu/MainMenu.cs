using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using TTT.Common;
using TTT.Host.Api;

namespace TTT.Host
{
    public class MainMenu
    {
        
        public MainMenu(GameManager gameManager, ISocketHub socketHub)
        {
            var game = gameManager.CurrentGame();

            ConnectButtonAction = async () =>
            {
                var playerId = await socketHub.ConnectAsync();
                game.AddPlayerToGame(playerId);
                //await socketHub.OpenConnectionAsync(playerId);
            };

            StartButtonAction = () =>
            {
                game.StartRandomPlayer();
                //game.StartAtEndGame();
                var gameState = game.GetCurrentState();
                var subCommand = new UpdateStateCommand(gameState, true);
                socketHub.BroadcastCommand(new GameCommand(subCommand));
                gameManager.StartGame();
            };

            TestButtonAction = async () =>
            {
                var playerId = await socketHub.ConnectAsync(true);
                game.AddPlayerToGame(playerId);
            };
        }
        public Action ConnectButtonAction { get; set; }
        public Action StartButtonAction { get; set; }
        public Action TestButtonAction { get; internal set; }
    }
}
