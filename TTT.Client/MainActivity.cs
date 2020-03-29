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
using TTT.Common;
using WebSocket4Net;
using Xamarin.Essentials;

namespace TTT.Client
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        int _width;
        int _height;
        public View MainView { get; set; }
        static GameGrid _game;
        
        Receiver _receiver = new Receiver(Guid.NewGuid());
        static ClientSocket _clientSocket;
        GameController _controller;

        public Receiver Receiver
        {
            get
            {
                return _receiver;
            }
        }

        void LoadClientSocket(IPAddress serverAddress)
        {
            _clientSocket = new ClientSocket(serverAddress);
            _clientSocket.Send("Connected");
            _controller = new GameController(_receiver.DeviceId, _clientSocket);
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            var metrics = Resources.DisplayMetrics;
            _width = metrics.WidthPixels;
            _height = metrics.HeightPixels;

            //OnSaveInstanceState(null);
            base.OnCreate(savedInstanceState);
            Platform.Init(this, savedInstanceState);
            if (!GameIsInProgress())
                AddConnectButton(this);
            else
                AddGameGrid(this);        

            Android.Support.V7.Widget.Toolbar toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);
        }

        bool GameIsInProgress()
        {
            return _game != null && _clientSocket != null && _clientSocket.IsOpen;
        }

        private void AddConnectButton(MainActivity mainActivity)
        {
            var baseLayout = new FrameLayout.LayoutParams(Constants.CellSizeClient, Constants.CellSizeClient);
            var baseX = (_width - Constants.CellSizeClient) / 2;
            var baseY = (_height - Constants.CellSizeClient) / 2;
            var frameLayout = new FrameLayout(mainActivity);
            var button = new Button(mainActivity);
            button.LayoutParameters = baseLayout;
            button.SetX(baseX);
            button.SetY(baseY);
            button.SetBackgroundColor(Color.Gray);
            button.SetTextColor(Color.White);
            button.SetTextSize(Android.Util.ComplexUnitType.Px, 50);
            button.Text = "CONNECT";
            button.Click += Connect;
            frameLayout.AddView(button);
            SetContentView(frameLayout);
        }

        private void AddGameGrid(MainActivity mainActivity)
        {
            _game = GetGame(_controller);
            _game.FrameLayout = new FrameLayout(mainActivity);
            
            _game.DrawCells_Func();
            ReloadView(_game.FrameLayout);
        }

        internal void Connect(object sender, EventArgs e)
        {
            if (_clientSocket == null)
                Task.Run(() => Receiver.Begin(ipAddress => LoadClientSocket(ipAddress), () => AsyncAddGameGrid(this)));
            else
                _clientSocket.Send("Still Connected");
        }

        private void AsyncAddGameGrid(MainActivity mainActivity)
        {
            RunOnUiThread(() => AddGameGrid(mainActivity));
        }

        private void ReloadView(FrameLayout frameLayout)
        {
            SetContentView(frameLayout);
        }

        public GameGrid GetGame(GameController _controller)
        {
            if (_game != null)
            {
                return _game;
            }
            //return new GameGrid(this, _width, _height);
            return new GameGrid(this, _controller, () => Resources.DisplayMetrics.WidthPixels, () => Resources.DisplayMetrics.HeightPixels);
        }
    }
}

