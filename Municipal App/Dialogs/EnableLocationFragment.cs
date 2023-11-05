using Android.Content;
using Android.Locations;
using Android.OS;
using Android.Views;
using Google;
using Google.Android.Material.Button;
using DialogFragment = AndroidX.Fragment.App.DialogFragment;
using Xamarin.Essentials;
using Android.Net;
using AndroidHUD;
using System;

namespace Municipal_App.Dialogs
{
    public class EnableLocationFragment : DialogFragment
    {
        private Context context;
        private MaterialButton BtnOpenSettings;
        public override void OnStart()
        {
            base.OnStart();

            Dialog.Window.SetLayout(ViewGroup.LayoutParams.MatchParent,ViewGroup.LayoutParams.WrapContent);
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
            // return inflater.Inflate(Resource.Layout.YourFragment, container, false);
            View view = inflater.Inflate(Resource.Layout.no_location_enabled_dialog_layout, container, false);
            Init(view);
            //CheckGps();

            return view;
        }

        private void Init(View view)
        {
            BtnOpenSettings = view.FindViewById<MaterialButton>(Resource.Id.BtnOpenSettings);
            BtnOpenSettings.Click += delegate
            {
                Intent intent = new Intent(Android.Provider.Settings.ActionLocat‌ionSourceSettings);
                intent.AddFlags(ActivityFlags.NewTask);
                Android.App.Application.Context.StartActivity(intent);
            };
        }

        private void CheckGps()
        {
            ConnectivityManager cm = (ConnectivityManager)this.Activity.GetSystemService(Context.ConnectivityService);
            bool isConnected = cm.IsDefaultNetworkActive;

            if (isConnected)
            {
                //AndHUD.Shared.ShowSuccess(context, "Network is :" + isConnected, MaskType.Black, TimeSpan.FromSeconds(3));
            }
            else
            {
               // AndHUD.Shared.ShowSuccess(context, "Network is not open", MaskType.Black, TimeSpan.FromSeconds(3));
            }
        }
    }
}