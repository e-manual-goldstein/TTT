using System;
using System.Threading;
using Android.App;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using WebSocket4Net;

namespace TTT.Client
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        
        public View MainView { get; set; }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_main);
            
            Android.Support.V7.Widget.Toolbar toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);

            FloatingActionButton fab = FindViewById<FloatingActionButton>(Resource.Id.fab);
            fab.Click += FabOnClick;
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

        WebSocket _clientSocket;
        public WebSocket ClientSocket {
            get
            {
                return _clientSocket ?? (_clientSocket = openWebSocket());
            } 

        }

        private WebSocket openWebSocket()
        {
            var webSocketClient = new WebSocket("ws://192.168.0.15:69/");
            //webSocketClient.AllowUnstrustedCertificate = true;
            webSocketClient.Opened += new EventHandler(webSocketClient_Opened);
            webSocketClient.Closed += new EventHandler(webSocketClient_Closed);
            webSocketClient.MessageReceived += new EventHandler<MessageReceivedEventArgs>(webSocketClient_MessageReceived);
            webSocketClient.Open();
            return webSocketClient;
        }

        private void FabOnClick(object sender, EventArgs eventArgs)
        {
            MainView = (View)sender;
            Test();
            //using (var ws = new WebSocket("ws://192.168.0.15:69/"))
            //{
            //    //ws.MessageReceived += Ws_MessageReceived;
            //    //ws.Closed += Ws_Closed;
            //    ws.Open();
               
            //    ws.Send("Hello");
            //    //Console.ReadKey(true);
            //}
            Snackbar.Make(MainView, "Socket Opened", Snackbar.LengthLong)
                .SetAction("Action", (Android.Views.View.IOnClickListener)null).Show();
        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        public void Test()
        {
            var webSocketClient = ClientSocket;

            if (!m_OpenedEvent.WaitOne(5000))
            {
               //Assert.Fail("Failed to Opened session ontime");
            }

            //Assert.AreEqual(WebSocketState.Open, webSocketClient.State);

            for (var i = 0; i < 10; i++)
            {
                var message = Guid.NewGuid().ToString();

                webSocketClient.Send(message);

                if (!m_MessageReceiveEvent.WaitOne(5000))
                {
                    //Assert.Fail("Failed to get echo messsage on time");
                    break;
                }

                //Assert.AreEqual(m_CurrentMessage, message);
            }

            //webSocketClient.Close();

            //if (!m_CloseEvent.WaitOne(5000))
            //{
            //    //Assert.Fail("Failed to close session ontime");
            //}

            //Assert.AreEqual(WebSocketState.Closed, webSocketClient.State);
        }
        protected AutoResetEvent m_MessageReceiveEvent = new AutoResetEvent(false);
        protected AutoResetEvent m_OpenedEvent = new AutoResetEvent(false);
        protected AutoResetEvent m_CloseEvent = new AutoResetEvent(false);
        protected string m_CurrentMessage = string.Empty;

        protected void webSocketClient_MessageReceived(object sender, MessageReceivedEventArgs e)
        {
            Snackbar.Make(MainView, e.Message, Snackbar.LengthLong)
                .SetAction("Action", (Android.Views.View.IOnClickListener)null).Show();
        }

        protected void webSocketClient_Closed(object sender, EventArgs e)
        {
            m_CloseEvent.Set();
        }

        protected void webSocketClient_Opened(object sender, EventArgs e)
        {
            m_OpenedEvent.Set();
        }
    
	}
}

