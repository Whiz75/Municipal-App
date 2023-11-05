using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.Widget;
using Google.Android.Material.Button;
using Municipal_App.Models;
using Plugin.CloudFirestore;
using System;
using System.Threading.Tasks;
using Xamarin.Essentials;
using DialogFragment = AndroidX.Fragment.App.DialogFragment;

namespace Municipal_App.Dialogs
{
    public class StatusUpdateDialogFragment : DialogFragment
    {
        private Context mContext;
        public string Id;
        private AppCompatImageView ic_cancel;
        private RadioGroup radioGroup1;
        private MaterialButton BtnReviewIncident;

        public StatusUpdateDialogFragment(string Id)
        {
            this.Id = Id;
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            View view = inflater.Inflate(Resource.Layout.status_update_layout, container, false);
            Init(view);

            return view;
        }

        private void Init(View view)
        {
            ic_cancel = view.FindViewById<AppCompatImageView>(Resource.Id.img_cancel);

            BtnReviewIncident = view.FindViewById<MaterialButton>(Resource.Id.BtnReviewIncident);

            ic_cancel.Click += delegate
            {
                Dialog.Dismiss();
            };

            BtnReviewIncident.Click += delegate
            {
                //NavigateToBuilding();
            };
        }

        private void StatusUpdate()
        {
            try
            {
                Incident i = new Incident()
                {
                    Status = "IN PROGRESS"
                };

                CrossCloudFirestore
                    .Current
                    .Instance
                    .Collection("Incidents")
                    .Document(Id)
                    .AddSnapshotListener((snapshot, error) =>
                    {
                        if (snapshot.Exists)
                        {
                            CrossCloudFirestore
                            .Current
                            .Instance
                            .Collection("Incidents")
                            .Document(Id)
                            .UpdateAsync(i);
                        }
                    });
            }
            catch (Exception ex)
            {
                Toast.MakeText(mContext,ex.Message,ToastLength.Long).Show();
            }
        }
    }
}