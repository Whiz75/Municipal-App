using Android.App;
using Android.Content;
using Android.Net;
using Android.OS;
using AndroidHUD;
using AndroidX.AppCompat.App;
using Google.Android.Material.Button;
using Google.Android.Material.TextField;
using Google.Android.Material.TextView;
using ID.IonBit.IonAlertLib;
using Municipal_App.Dialogs;
using Plugin.FirebaseAuth;
using System;
using Xamarin.Essentials;

namespace Municipal_App.Activities
{
    [Activity(Label = "Sign_In_Activity")]
    public class Sign_In_Activity : AppCompatActivity
    {
        private Context context;
        private TextInputEditText InputLoginEmail;
        private TextInputEditText InputLoginPassword;
        private MaterialTextView TextViewLoginSignUp;

        private MaterialButton BtnLogin;
        private MaterialButton BtnLoginSignUp;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.login_layout);

            //initiate components
            Init();

            var user = CrossFirebaseAuth.Current.Instance.CurrentUser;
            var current = Connectivity.NetworkAccess;

            if (current == NetworkAccess.Internet)
            {
                // Connection to internet is available
                if (user != null)
                {
                    StartActivity(new Intent(Application.Context, typeof(MainActivity)));
                }
            }
            else
            {
                NoNetworkFragment noNetwork = new NoNetworkFragment();
                noNetwork.Show(SupportFragmentManager.BeginTransaction(), "");
            }
        }

        private void Init()
        {
            context = Application.Context;

            InputLoginEmail = FindViewById<TextInputEditText>(Resource.Id.EmailTextInput);
            InputLoginPassword = FindViewById<TextInputEditText>(Resource.Id.PasswordTextInput);

            BtnLogin = FindViewById<MaterialButton>(Resource.Id.BtnSignIn);
            TextViewLoginSignUp = FindViewById<MaterialTextView>(Resource.Id.TextViewLoginSignUp);

            BtnLogin.Click += delegate
            {
                LoginUser();
            };

            TextViewLoginSignUp.Click += delegate
            {
                StartActivity(new Intent(Application.Context, typeof(Sign_Up_Activity)));
            };
        }

        private async void LoginUser()
        {
            
            if (string.IsNullOrEmpty(InputLoginEmail.Text) || string.IsNullOrWhiteSpace(InputLoginEmail.Text))
            {
                InputLoginEmail.RequestFocus();
                InputLoginEmail.Error = "Provide your username or email";
                return;
            }
            else if (string.IsNullOrEmpty(InputLoginPassword.Text) || string.IsNullOrWhiteSpace(InputLoginPassword.Text))
            {
                InputLoginPassword.RequestFocus();
                InputLoginPassword.Error = "Provide your username or email";
                return;
            }
            else
            {
                try
                {
                    var result = await CrossFirebaseAuth
                    .Current
                    .Instance
                    .SignInWithEmailAndPasswordAsync(InputLoginEmail.Text, InputLoginPassword.Text);

                    if (result.User != null)
                    {
                        new IonAlert(this, IonAlert.SuccessType)
                                .SetTitleText("Successful!")
                                .SetContentText("You logged in successfully!!!")
                                .Show();

                        StartActivity(new Intent(Application.Context, typeof(MainActivity)));
                    }
                }
                catch (Exception ex)
                {
                    AndHUD.Shared
                        .ShowSuccess(Application.Context, ex.Message, MaskType.Clear, TimeSpan.FromSeconds(1));
                }
            }
        }
    }
}