using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Support.V7.Widget;
using FrameLayout = Android.Widget.FrameLayout;
using Xamarin.Essentials;

namespace TTT.Client
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true)]
    public class MenuActivity : AppCompatActivity
    {
        
        public MenuActivity()
        {
            
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            
            //OnSaveInstanceState(null);
            base.OnCreate(savedInstanceState);
            Platform.Init(this, savedInstanceState);
            
            

            Toolbar toolbar = FindViewById<Toolbar>(Resource.Id.toolbar);
            
            SetSupportActionBar(toolbar);
        }
    }
}

