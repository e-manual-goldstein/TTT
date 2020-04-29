using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Android.App;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Microsoft.Extensions.DependencyInjection;
using TTT.Common;
using WebSocket4Net;
using Xamarin.Essentials;

namespace TTT.Client
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        public int _width;
        public int _height;
        
        protected override void OnCreate(Bundle savedInstanceState)
        {
            var metrics = Resources.DisplayMetrics;
            _width = metrics.WidthPixels;
            _height = metrics.HeightPixels;

            var app = App.Current;
            app.RegisterMainActivity(this);
            //OnSaveInstanceState(null);
            base.OnCreate(savedInstanceState);
            Platform.Init(this, savedInstanceState);
            var gameManager = app.ServiceProvider.GetService<GameManager>();
            var dict = createActionDictionary();
            if (!gameManager.GameIsInProgress())
                AddButtons(dict);
            else
                //replace with Reconnect + GetGameState
                AddGameGrid(new GameState(Guid.NewGuid(), null));
            Android.Support.V7.Widget.Toolbar toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            
            SetSupportActionBar(toolbar);
        }

        private Dictionary<string, EventHandler> createActionDictionary()
        {
            return new Dictionary<string, EventHandler>() 
            { 
                { "LISTEN", new EventHandler(EventListener) },
                { "TEST", new EventHandler(Test) },
                { "CONNECT", new EventHandler(Connect) }
            };
        }

        public void ReloadView(FrameLayout frameLayout)
        {
            SetContentView(frameLayout);
        }

        public void AddButtons(IDictionary<string, EventHandler> callbackDictionary)
        {
            var baseLayout = new FrameLayout.LayoutParams(Constants.CellSizeClient, Constants.CellSizeClient);
            var baseX = (_width - Constants.CellSizeClient) / 2;
                var frameLayout = new FrameLayout(this);
            int n = 1;
            foreach (var item in callbackDictionary)
            {   
                var baseY = (n * (_height - Constants.CellSizeClient)) / (callbackDictionary.Count + 1);
                var button = new Button(this);
                button.LayoutParameters = baseLayout;
                button.SetX(baseX);
                button.SetY(baseY);
                button.SetBackgroundColor(Color.Gray);
                button.SetTextColor(Color.White);
                button.SetTextSize(Android.Util.ComplexUnitType.Px, 50);
                button.Text = item.Key;
                button.Click += item.Value;
                frameLayout.AddView(button);
                n++;
            }
            SetContentView(frameLayout);
        }

        private void Connect(object sender, EventArgs e)
        {
            var extConnector = App.Current.ServiceProvider.GetService<ExternalHostManager>();
            var socketManager = App.Current.ServiceProvider.GetService<SocketManager>();
            RunOnUiThread(async () => {
                var ipEndpoint = await extConnector.FindOnlineGame();
                socketManager.CreateSocket(ipEndpoint);
            });
        }

        private void EventListener(object sender, EventArgs e)
        {
            var socketManager = App.Current.ServiceProvider.GetService<SocketManager>();
            var playerManager = App.Current.ServiceProvider.GetService<PlayerManager>();
            if (socketManager.HostSocket ==  null || !socketManager.HostSocket.IsOpen)
            {
                RunOnUiThread(async () => {
                    var playerId = await socketManager.Listen();
                    playerManager.SetPlayerId(playerId);
                });
            }
        }

        private void Test(object sender, EventArgs e)
        {
            var socketManager = App.Current.ServiceProvider.GetService<SocketManager>();
            var playerManager = App.Current.ServiceProvider.GetService<PlayerManager>();
            if (socketManager.HostSocket == null || !socketManager.HostSocket.IsOpen)
            {
                Task.Run(async () => {
                    var playerId = await socketManager.Listen();
                    playerManager.SetPlayerId(playerId);
                });
            }
        }

        public void AddGameGrid(GameState gameState)
        {
            var gameManager = App.Current.ServiceProvider.GetService<GameManager>();
            var game = gameManager.GetGame();

            game.FrameLayout = new FrameLayout(this);
            game.DrawCells(this, gameState);
            ReloadView(game.FrameLayout);
        }
    }
}

