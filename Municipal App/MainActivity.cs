using Android.App;
using Android.OS;
using Android.Views;
using AndroidX.AppCompat.App;
using Google.Android.Material.AppBar;
using Google.Android.Material.BottomNavigation;
using Google.Android.Material.FloatingActionButton;
using Municipal_App.Dialogs;
using Municipal_App.Fragments;
using static Google.Android.Material.Navigation.NavigationBarView;
using AndroidX.AppCompat.Widget;
using Android.Content.Res;
using Google.Android.Material.Dialog;
using Plugin.FirebaseAuth;
using Android.Content;
using Android.Locations;
using Firebase.Messaging;
using Android.Gms.Extensions;
using Plugin.FirebasePushNotification;
using AndroidHUD;
using System;

namespace Municipal_App
{
    [Activity(Theme = "@style/AppTheme", MainLauncher = false)]
    public class MainActivity : AppCompatActivity, IOnItemSelectedListener
    {
        private MaterialToolbar topAppBar;
        
        private FloatingActionButton fabViewIncidents;
        private BottomNavigationView bottomNavigationView;

        protected async override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);

            if (savedInstanceState == null)
            {
                SupportFragmentManager.BeginTransaction().Add(Resource.Id.fragHost, new HomeFragment()).Commit();
            }

            //CrossFirebasePushNotification.Current.OnNotificationReceived += ((s,e) =>
            //{
            //    AndHUD.Shared.ShowSuccess(this, "it ran",MaskType.None,TimeSpan.FromSeconds(3));
            //});

            //initiate components
            Init();
            await Xamarin.Essentials.Geolocation.GetLocationAsync();


            //if (!isGpsAvailable())
            //{
            //    EnableLocationFragment frag = new EnableLocationFragment();
            //    frag.Show(SupportFragmentManager.BeginTransaction(),"");
            //}
        }

        private void Init()
        {
            
            bottomNavigationView = FindViewById<BottomNavigationView>(Resource.Id.bottom_navigation);

            bottomNavigationView.SetOnItemSelectedListener(this);
            
        }

        bool IOnItemSelectedListener.OnNavigationItemSelected(IMenuItem p0)
        {
            switch (p0.ItemId)
            {
                case Resource.Id.item_home:
                    SupportFragmentManager.BeginTransaction().Replace(Resource.Id.fragHost, new HomeFragment()).Commit();
                    break;
                case Resource.Id.item_incident_type:
                    SupportFragmentManager.BeginTransaction().Replace(Resource.Id.fragHost, new IncidentTypeFragment()).Commit();
                    break;
                case Resource.Id.item_history:
                    SupportFragmentManager.BeginTransaction().Replace(Resource.Id.fragHost, new HistoryFragment()).Commit();
                    break;
                case Resource.Id.item_profile:
                    SupportFragmentManager.BeginTransaction().Replace(Resource.Id.fragHost, new ProfileFragment()).Commit();
                    break;
                default:
                    SupportFragmentManager.BeginTransaction().Replace(Resource.Id.fragHost, new HomeFragment()).Commit();
                    break;
            }
            return true;
        }

        public bool isGpsAvailable()
        {
            bool value = false;

            LocationManager manager = (LocationManager)Application.Context.GetSystemService(Context.LocationService);
            if (!manager.IsProviderEnabled(LocationManager.GpsProvider))
            {
                //gps disable
                value = false;
            }
            else
            {
                //Gps enable
                value = true;
            }
            return value;
        }
    }
}