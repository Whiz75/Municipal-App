using Android.Content;
using Android.OS;
using Android.Views;
using Google.Android.Material.Button;
using DialogFragment = AndroidX.Fragment.App.DialogFragment;

namespace Municipal_App.Dialogs
{
    public class NoNetworkFragment : DialogFragment
    {
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
            View view = inflater.Inflate(Resource.Layout.no_network_dialog_layout, container, false);
            Init(view);

            return view;
        }

        private void Init(View view)
        {
            BtnOpenSettings = view.FindViewById<MaterialButton>(Resource.Id.BtnOpenSettings);
            BtnOpenSettings.Click += delegate
            {
                Intent intent = new Intent(Android.Provider.Settings.ActionNetworkOperatorSettings);
                intent.AddFlags(ActivityFlags.NewTask);
                Android.App.Application.Context.StartActivity(intent);
            };
        }
    }
}