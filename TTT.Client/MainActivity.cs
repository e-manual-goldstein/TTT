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
        ClientSocket _clientSocket;
        Guid _deviceId;


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
            _clientSocket.WebSocket.Send("Connected");
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            var metrics = Resources.DisplayMetrics;
            _width = metrics.WidthPixels;
            _height = metrics.HeightPixels;


            base.OnCreate(savedInstanceState);
            Platform.Init(this, savedInstanceState);

            AddConnectButton(this);
            //AddTestButton(this);
            //AddGameGrid(this);

        

            Android.Support.V7.Widget.Toolbar toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);
        }

        private void AddGameGrid(MainActivity mainActivity)
        {
            _game = GetGame();
            _game.FrameLayout = new FrameLayout(mainActivity);

            _game.DrawCells_Func();
            ReloadView(_game.FrameLayout);
        }

        private void AsyncAddGameGrid(MainActivity mainActivity)
        {
            RunOnUiThread(() => AddGameGrid(mainActivity));
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

        internal void Connect(object sender, EventArgs e)
        {
            //Task.Run(() => AsyncAddGameGrid(this));
            if (_clientSocket == null)
                Task.Run(() => Receiver.Begin(ipAddress => LoadClientSocket(ipAddress), () => AsyncAddGameGrid(this)));
            else
                _clientSocket.WebSocket.Send("Still Connected");
        }

        private void AddTestButton(MainActivity mainActivity)
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
            button.Text = "TEST";
            button.Click += TestButton;
            frameLayout.AddView(button);
            SetContentView(frameLayout);
        }

        internal void TestButton(object sender, EventArgs e)
        {
            if (_clientSocket == null)
                Task.Run(() => Receiver.Begin(ipAddress => LoadClientSocket(ipAddress), null));
            else
                _clientSocket.WebSocket.Send("Still Connected");
        }

        private void ReloadView(FrameLayout frameLayout)
        {
            SetContentView(frameLayout);
        }

        public GameGrid GetGame()
        {
            if (_game != null)
            {
                return _game;
            }
            //return new GameGrid(this, _width, _height);
            return new GameGrid(this, () => Resources.DisplayMetrics.WidthPixels, () => Resources.DisplayMetrics.HeightPixels);
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.menu_main, menu);
            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            int id = item.ItemId;
            if (id == Resource.Id.action_settings)
            {
                return true;
            }

            return base.OnOptionsItemSelected(item);
        }

        private void OnClick(object sender, EventArgs eventArgs)
        {
            

            //Test();
            //Test2();
            Snackbar.Make(MainView, "Socket Opened", Snackbar.LengthLong)
                .SetAction("Action", (View.IOnClickListener)null).Show();
        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

    }
}

