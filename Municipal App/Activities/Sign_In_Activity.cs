using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using AndroidHUD;
using AndroidX.AppCompat.App;
using Google.Android.Material.Button;
using Google.Android.Material.TextField;
using Google.Android.Material.TextView;
using ID.IonBit.IonAlertLib;
using Municipal_App.Dialogs;
using Plugin.FirebaseAuth;
using System;

namespace Municipal_App.Activities
{
    [Activity(Label = "Sign_In_Activity", Theme = "@style/AppTheme", MainLauncher = true)]
    public class Sign_In_Activity : AppCompatActivity
    {
        private Context context;
        private TextInputEditText InputLoginEmail;
        private TextInputEditText InputLoginPassword;
        private MaterialTextView TextViewLoginSignUp;
        private MaterialTextView TextViewResetPassword;

        private MaterialButton BtnLogin;
        private MaterialButton BtnLoginSignUp;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.login_layout);

            //initiate components
            Init();


        }

        private void Init()
        {
            context = Application.Context;

            InputLoginEmail = FindViewById<TextInputEditText>(Resource.Id.EmailTextInput);
            InputLoginPassword = FindViewById<TextInputEditText>(Resource.Id.PasswordTextInput);

            BtnLogin = FindViewById<MaterialButton>(Resource.Id.BtnSignIn);
            TextViewLoginSignUp = FindViewById<MaterialTextView>(Resource.Id.TextViewLoginSignUp);
            TextViewResetPassword = FindViewById<MaterialTextView>(Resource.Id.TextViewResetPassword);

            BtnLogin.Click += delegate
            {
                LoginUser();
            };

            TextViewLoginSignUp.Click += delegate
            {
                StartActivity(new Intent(Application.Context, typeof(Sign_Up_Activity)));
            };

            TextViewResetPassword.Click += delegate
            {
                ResetPasswordFragment reset = new ResetPasswordFragment();
                reset.Show(SupportFragmentManager.BeginTransaction(), "");
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
                var loadingDialog = new IonAlert(this, IonAlert.ProgressType);
                loadingDialog.SetSpinKit("WanderingCubes")
                    .ShowCancelButton(false)
                    .Show();
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
                catch (FirebaseAuthException ex)
                {
                    AndHUD.Shared
                        .ShowSuccess(this, ex.Message, MaskType.Clear, TimeSpan.FromSeconds(5));
                }
                finally
                {
                    loadingDialog.Dismiss();
                }
            }
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}