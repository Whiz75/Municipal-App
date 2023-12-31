﻿using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using AndroidHUD;
using AndroidX.AppCompat.App;
using Google.Android.Material.Button;
using Google.Android.Material.TextField;
using Google.Android.Material.TextView;
using ID.IonBit.IonAlertLib;
using Municipal_App.Models;
using Plugin.CloudFirestore;
using Plugin.FirebaseAuth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace Municipal_App.Activities
{
    [Activity(Label = "Sign_Up_Activity")]
    public class Sign_Up_Activity : AppCompatActivity
    {
        private TextInputEditText FirstName;
        private TextInputEditText LastName;
        private TextInputEditText EmailAddress;
        private TextInputEditText Password;
        private TextInputEditText ConfirmPassword;

        private MaterialButton BtnSignUp;
        private MaterialTextView TextViewSignUpSignIn;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your application here
            SetContentView(Resource.Layout.sign_up_layout);

            //initialize components
            Init();
        }

        private void Init()
        {
            FirstName = FindViewById<TextInputEditText>(Resource.Id.RegFirstNameTextInput);
            LastName = FindViewById<TextInputEditText>(Resource.Id.RegLastNameTextInput);
            EmailAddress = FindViewById<TextInputEditText>(Resource.Id.RegEmailTextInput);
            Password = FindViewById<TextInputEditText>(Resource.Id.RegPasswordTextInput);
            ConfirmPassword = FindViewById<TextInputEditText>(Resource.Id.RegConfirmPasswordTextInput);

            BtnSignUp = FindViewById<MaterialButton>(Resource.Id.BtnSignUp);
            TextViewSignUpSignIn = FindViewById<MaterialTextView>(Resource.Id.TextViewSignUpSignIn);

            BtnSignUp.Click += async delegate
            {
               await SignUpUserAsync();
            };

            TextViewSignUpSignIn.Click += delegate
            {
                StartActivity(new Intent(Application.Context, typeof(Sign_In_Activity)));
            };
        }

        private async Task SignUpUserAsync()
        {
            if (string.IsNullOrEmpty(FirstName.Text) || string.IsNullOrWhiteSpace(FirstName.Text))
            {
                FirstName.RequestFocus();
                FirstName.Error = "Provide your Firstname";
                return;
            }
            else if (string.IsNullOrEmpty(LastName.Text) || string.IsNullOrWhiteSpace(LastName.Text))
            {
                LastName.RequestFocus();
                LastName.Error = "Provide your Lastname";
                return;
            }
            else if (string.IsNullOrEmpty(EmailAddress.Text) || string.IsNullOrWhiteSpace(EmailAddress.Text))
            {
                EmailAddress.RequestFocus();
                EmailAddress.Error = "Provide your Email";
                return;
            }
            else if (string.IsNullOrEmpty(Password.Text) || string.IsNullOrWhiteSpace(Password.Text))
            {
                Password.RequestFocus();
                Password.Error = "Provide a password";
                return;
            }
            else if (string.IsNullOrEmpty(ConfirmPassword.Text) || string.IsNullOrWhiteSpace(ConfirmPassword.Text))
            {
                ConfirmPassword.RequestFocus();
                ConfirmPassword.Error = "Provide a password confirmation";
                return;
            }
            else if (!Password.Text.Equals(ConfirmPassword.Text))
            {
                ConfirmPassword.RequestFocus();
                ConfirmPassword.Error = "Password do not match";
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

                    var auth = await CrossFirebaseAuth.
                        Current.Instance
                        .CreateUserWithEmailAndPasswordAsync(EmailAddress.Text.Trim(), Password.Text.Trim());


                    if (auth.User != null)
                    {
                        User user = new User()
                        {
                            FirstName = FirstName.Text,
                            LastName = LastName.Text,
                            Email = EmailAddress.Text,
                            Role = "Admin"
                        };

                        await CrossCloudFirestore.Current
                            .Instance
                            .Collection("USERS")
                            .Document(auth.User.Uid)
                            .SetAsync(user);
                        StartActivity(new Intent(Application.Context, typeof(MainActivity)));

                        AndHUD.
                            Shared
                            .ShowSuccess(this, "Your account has been successfully created!!!", MaskType.Black, TimeSpan.FromSeconds(4));

                        //SendEmail("Registration successful",$" Congrats n/{user.FirstName} {user.LastName}", new List<string> { $"{user.Email}" });
                    }

                }
                catch (FirebaseAuthException ex)
                {
                    AndHUD.Shared.ShowError(this, ex.Message, MaskType.Black, TimeSpan.FromSeconds(4));
                }
                finally
                {
                    loadingDialog.Dismiss();
                }
            }
        }

        private void SendEmail(string subject, string body, List<string> recipients)
        {
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                try
                {
                    var emailMessage = new EmailMessage
                    {
                        Subject = subject,
                        Body = body,
                        To = recipients,
                        // You can also add Cc and Bcc recipients if needed:
                        // Cc = new List<string> { "cc@example.com" },
                        // Bcc = new List<string> { "bcc@example.com" }
                    };

                    await Email.ComposeAsync(emailMessage);
                }
                catch (FeatureNotSupportedException ex)
                {
                    // Email is not supported on this device
                    // Handle accordingly
                    AndHUD.Shared.ShowError(this, ex.Message, MaskType.Black,TimeSpan.FromSeconds(3));
                }
                catch (Exception ex)
                {
                    // Other error occurred
                    // Handle accordingly
                    AndHUD.Shared.ShowError(this, ex.Message, MaskType.Black, TimeSpan.FromSeconds(3));
                }
            });

        }
    }
}