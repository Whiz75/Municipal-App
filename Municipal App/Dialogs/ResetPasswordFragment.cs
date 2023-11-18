using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using AndroidHUD;
using AndroidX.AppCompat.Widget;
using Google.Android.Material.Button;
using Google.Android.Material.TextField;
using ID.IonBit.IonAlertLib;
using Plugin.FirebaseAuth;
using System;
using System.Threading.Tasks;
using DialogFragment = AndroidX.Fragment.App.DialogFragment;

namespace Municipal_App.Dialogs
{
    public class ResetPasswordFragment : DialogFragment
    {
        private Context mContext;

        private AppCompatImageView img_cancel;
        private TextInputEditText email_txt;
        private MaterialButton BtnResetPassword;

        public override void OnStart()
        {
            base.OnStart();
            Dialog.Window.SetLayout(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent);
            Dialog.SetCanceledOnTouchOutside(false);
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            View view = inflater.Inflate(Resource.Layout.forgot_password_layout, container, false);
            Init(view);

            return view;
        }

        private void Init(View view)
        {
            mContext = view.Context;

            img_cancel = view.FindViewById<AppCompatImageView>(Resource.Id.img_cancel);

            email_txt = view.FindViewById<TextInputEditText>(Resource.Id.emailResetInput);

            BtnResetPassword = view.FindViewById<MaterialButton>(Resource.Id.BtnResetPassword);

            img_cancel.Click += delegate
            {
                Dialog.Dismiss();
            };

            BtnResetPassword.Click += async delegate
            {
                await resetPassword();
            };
        }

        private async Task resetPassword()
        {

            if (string.IsNullOrEmpty(email_txt.Text.Trim()) || string.IsNullOrWhiteSpace(email_txt.Text.Trim()))
            {
                email_txt.RequestFocus();
                email_txt.Error = "Email address field can not be empty";
            }
            else
            {
                var loadingDialog = new IonAlert(mContext, IonAlert.SuccessType);
                loadingDialog.SetTitleText("Success");
                loadingDialog.SetContentText("Email was successfully sent")
                .Show();

                try
                {
                    await CrossFirebaseAuth.Current.Instance.SendPasswordResetEmailAsync(email_txt.Text.Trim());
                }
                catch (Exception ex)
                {
                    AndHUD.Shared.ShowError(mContext,ex.Message,MaskType.Clear,TimeSpan.FromSeconds(3));
                }
                finally
                {
                    loadingDialog.Dismiss();
                    Dialog.Dismiss();
                }
                
            }
        }
    }
}