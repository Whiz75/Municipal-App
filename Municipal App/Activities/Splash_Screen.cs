using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using AndroidX.AppCompat.App;
using Plugin.FirebaseAuth;
using System.Threading.Tasks;

namespace Municipal_App.Activities
{
    [Activity(Label = "Splash_Screen", Theme = "@style/MyTheme.Splash", MainLauncher = false)]
    public class Splash_Screen : AppCompatActivity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            

            // Create your application here
            if (savedInstanceState == null)
            {
            }
            //FirebaseApp.InitializeApp(Application.Context);

            //Task startupWork = new Task(() => { SimulateStartup(); });
            //startupWork.Start();

            CrossFirebaseAuth
                 .Current
                 .Instance
                 .AuthState += Instance_AuthState;
        }

        private void Instance_AuthState(object sender, AuthStateEventArgs e)
        {
            if (e.Auth.CurrentUser == null)
            {
                SimulateStartup();
            }
            else
            {
                StartActivity(new Intent(this, typeof(MainActivity)));
                OverridePendingTransition(Resource.Animation.left_in, Resource.Animation.left_out);
            }
        }

        // Launches the startup task
        protected override void OnResume()
        {
            base.OnResume();
        }

        // Simulates background work that happens behind the splash screen
        private async void SimulateStartup()
        {
            //await Task.Delay(3000); 
            // Simulate a bit of startup work.
            StartActivity(new Intent(this, typeof(Sign_In_Activity)));
            OverridePendingTransition(Resource.Animation.left_in, Resource.Animation.left_out);
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}