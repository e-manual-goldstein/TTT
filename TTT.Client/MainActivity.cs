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

        static GameGrid _game;
        static Lazy<Logger> _logger = new Lazy<Logger>();
        static HostSocket _hostSocket;
        ActionService _actionService;

        public Logger Logger { get => _logger.Value; }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            var metrics = Resources.DisplayMetrics;
            _width = metrics.WidthPixels;
            _height = metrics.HeightPixels;

            //OnSaveInstanceState(null);
            base.OnCreate(savedInstanceState);
            Platform.Init(this, savedInstanceState);
            if (!GameIsInProgress())
                AddListenButton(this);
            else
                AddGameGrid(this);
            Android.Support.V7.Widget.Toolbar toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);
        }

        bool GameIsInProgress()
        {
            return _game != null && _hostSocket != null && _hostSocket.IsOpen;
        }

        private void AddListenButton(MainActivity mainActivity)
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
            button.Text = "LISTEN";
            button.Click += EventListener;
            frameLayout.AddView(button);
            SetContentView(frameLayout);
        }

        private void EventListener(object sender, EventArgs e)
        {
            RunOnUiThread(async () => await Listen(sender, e));
        }

        private async Task Listen(object sender, EventArgs e)
        {
            var targetEndPoint = new IPEndPoint(IPAddress.Any, Constants.SERVER_LISTEN_PORT);

            using (var listener = new UdpClient(targetEndPoint) { EnableBroadcast = true })
            {
                while (_hostSocket == null)
                await listener.ReceiveAsync().ContinueWith(task =>
                {
                    var message = UdpMessage.FromByteArray(task.Result.Buffer);
                    if (Guid.TryParse(message.Payload, out Guid clientId))
                    {
                        _hostSocket = new HostSocket(task.Result.RemoteEndPoint.Address, Logger);
                        _hostSocket.Send($"{clientId}");
                        _actionService = new ActionService(clientId, _hostSocket);
                        AsyncAddGameGrid(this);
                    }
                });
            }
        }

        private void AddGameGrid(MainActivity mainActivity)
        {
            _game = GetGame(_actionService);
            _game.FrameLayout = new FrameLayout(mainActivity);
            
            _game.DrawCells();
            ReloadView(_game.FrameLayout);
        }

        private void AsyncAddGameGrid(MainActivity mainActivity)
        {
            RunOnUiThread(() => AddGameGrid(mainActivity));
        }

        private void ReloadView(FrameLayout frameLayout)
        {
            SetContentView(frameLayout);
        }

        public GameGrid GetGame(ActionService actionService)
        {
            if (_game != null)
            {
                return _game;
            }
            //return new GameGrid(this, _width, _height);
            return new GameGrid(this, actionService, () => Resources.DisplayMetrics.WidthPixels, () => Resources.DisplayMetrics.HeightPixels);
        }
    }
}

