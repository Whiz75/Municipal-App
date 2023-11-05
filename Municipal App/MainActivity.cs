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
using AlertDialog = AndroidX.AppCompat.App.AlertDialog;
using Android.Content;
using Android.Net;
using System.Threading.Tasks;
using Xamarin.Essentials;
using static Xamarin.Essentials.Permissions;
using System;
using Android.Locations;
using Firebase.Messaging;
using Android.Gms.Extensions;

namespace Municipal_App
{
    [Activity(MainLauncher = false)]
    public class MainActivity : AppCompatActivity, IOnItemSelectedListener
    {
        private MaterialToolbar topAppBar;
        private FloatingActionButton fabAddIncident;
        private FloatingActionButton fabViewIncidents;
        private BottomNavigationView bottomNavigationView;

        protected override async void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);

            await FirebaseMessaging.Instance.SubscribeToTopic("Incidents");
            //initiate components
            Init();

            if (savedInstanceState == null)
            {
                SupportFragmentManager.BeginTransaction().Add(Resource.Id.fragHost, new HomeFragment()).Commit();
            }

            if (!isGpsAvailable())
            {
                EnableLocationFragment frag = new EnableLocationFragment();
                frag.Show(SupportFragmentManager.BeginTransaction(),"");
            }
        }

        private void Init()
        {
            topAppBar = FindViewById<MaterialToolbar>(Resource.Id.topAppBar);
            fabAddIncident = FindViewById<FloatingActionButton>(Resource.Id.fabAddIncident);
            bottomNavigationView = FindViewById<BottomNavigationView>(Resource.Id.bottom_navigation);

            topAppBar.MenuItemClick += TopAppBar_MenuItemClick;
            bottomNavigationView.SetOnItemSelectedListener(this);
            fabAddIncident.Click += delegate
            {
                AddIncidentsTypeFragment add = new AddIncidentsTypeFragment();
                add.Show(SupportFragmentManager.BeginTransaction(),"");
            };
        }

        private void TopAppBar_MenuItemClick(object sender, Toolbar.MenuItemClickEventArgs e)
        {
            switch(e.Item.ItemId)
            {
                case Resource.Id.top_bar_sign_out:
                    UserSignOut();
                    break;
                case Resource.Id.top_bar_profile:
                    break;
            }
        }

        bool IOnItemSelectedListener.OnNavigationItemSelected(IMenuItem p0)
        {
            switch (p0.ItemId)
            {
                case Resource.Id.item_home:
                    SupportFragmentManager.BeginTransaction().Replace(Resource.Id.fragHost, new HomeFragment()).Commit();
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

        private void UserSignOut()
        {
            MaterialAlertDialogBuilder materialAlert = new MaterialAlertDialogBuilder(this);
            materialAlert.SetTitle(Resources.GetString(Resource.String.sign_out_text));
            materialAlert.SetMessage(Resources.GetString(Resource.String.sign_out_question_text));
            materialAlert.SetPositiveButton(Resources.GetString(Resource.String.yes_text), (s, e) =>
            {
                var user = CrossFirebaseAuth.Current.Instance.CurrentUser;

                if (user != null)
                {
                    materialAlert.Dispose();
                }
                
            }).SetNegativeButton(Resources.GetString(Resource.String.no_text), (s, e) =>
            {
                materialAlert.Dispose();
            });

            materialAlert.Show();
            
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