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

            //_game = GetGame();
            //_game.FrameLayout = new FrameLayout(this);

            //_game.DrawCells_Func();
            //ReloadView(_game.FrameLayout);
            AddTestButton(this);

            //Receiver.Begin(ipAddress => LoadClientSocket(ipAddress));

            //var testview = new TestLayout(this, OnClick);
            //testview.DrawButton();
            //SetContentView(testview);
            //SetContentView(Resource.Layout.activity_main);
            //MainView = testview;
            Android.Support.V7.Widget.Toolbar toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);

            //FloatingActionButton fab = FindViewById<FloatingActionButton>(Resource.Id.fab);
            //fab.Click += FabOnClick;

        }

        private void AddTestButton(MainActivity mainActivity)
        {
            var baseLayout = new FrameLayout.LayoutParams(Cell.Size, Cell.Size);
            var baseX = (_width - Cell.Size) / 2;
            var baseY = (_height - Cell.Size) / 2;
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
                Task.Run(() => Receiver.Begin(ipAddress => LoadClientSocket(ipAddress)));
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

