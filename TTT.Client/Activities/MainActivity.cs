using System;
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
            if (!gameManager.GameIsInProgress())
                AddListenButton();
            else
                //replace with Reconnect + GetGameState
                AddGameGrid();
            Android.Support.V7.Widget.Toolbar toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);
        }

        public void ReloadView(FrameLayout frameLayout)
        {
            SetContentView(frameLayout);
        }

        public void AddListenButton()
        {
            var baseLayout = new FrameLayout.LayoutParams(Constants.CellSizeClient, Constants.CellSizeClient);
            var baseX = (_width - Constants.CellSizeClient) / 2;
            var baseY = (_height - Constants.CellSizeClient) / 2;
            var frameLayout = new FrameLayout(this);
            var button = new Button(this);
            button.LayoutParameters = baseLayout;
            button.SetX(baseX);
            button.SetY(baseY);
            button.SetBackgroundColor(Color.Gray);
            button.SetTextColor(Color.White);
            button.SetTextSize(Android.Util.ComplexUnitType.Px, 50);
            button.Text = "LISTEN";
            button.Click += EventListener;
            frameLayout.AddView(button);
            SetContentView(frameLayout);
        }

        private void EventListener(object sender, EventArgs e)
        {
            var socketManager = App.Current.ServiceProvider.GetService<SocketManager>();
            RunOnUiThread(async () => await socketManager.Listen(this));
        }

        public void AsyncAddGameGrid()
        {
            RunOnUiThread(() => AddGameGrid());
        }
        public void AddGameGrid()
        {
            var gameManager = App.Current.ServiceProvider.GetService<GameManager>();
            var game = gameManager.GetGame();

            game.FrameLayout = new FrameLayout(this);
            game.DrawCells(this);
            ReloadView(game.FrameLayout);
        }
    }
}

