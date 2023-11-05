using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Text;
using Android.Util;
using Android.Views;
using Android.Widget;
using AndroidHUD;
using AndroidX.AppCompat.Widget;
using AndroidX.Fragment.App;
using Firebase.Firestore.Auth;
using Google.Android.Material.Button;
using Google.Android.Material.Dialog;
using Google.Android.Material.TextView;
using Municipal_App.Models;
using Plugin.CloudFirestore;
using Square.Picasso;
using System;
using System.Collections.Generic;
using Xamarin.Essentials;
using DialogFragment = AndroidX.Fragment.App.DialogFragment;
using User = Municipal_App.Models.User;

namespace Municipal_App.Dialogs
{
    public class DisplayIncidentOverviewFragment : DialogFragment
    {
        private Context mContext;

        private readonly string Id;
        private AppCompatImageView img_close_dialog;
        private AppCompatImageView img;
        private MaterialTextView status;
        private MaterialTextView username;
        private MaterialTextView date;
        private MaterialTextView type;
        private MaterialTextView description;
        private MaterialTextView coordinates;

        private MaterialButton BtnChangeStatus;
        private MaterialButton BtnLocation;

        private double lat, lon;
        public DisplayIncidentOverviewFragment(String Id,double lat, double lon)
        {
            this.Id = Id;
            this.lat = lat;
            this.lon = lon;
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override void OnStart()
        {
            base.OnStart();
            Dialog.Window.SetLayout(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent);
            Dialog.SetCanceledOnTouchOutside(false);
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            // return inflater.Inflate(Resource.Layout.YourFragment, container, false);
            View view = inflater.Inflate(Resource.Layout.incident_row, container, false);

            Init(view);
            GetIncidentInfo(view);
            GetUserInfo(Id);

            return view;
        }

        private void Init(View view)
        {
            mContext = view.Context;
            img_close_dialog = view.FindViewById<AppCompatImageView>(Resource.Id.img_close_dialog);
            img = view.FindViewById<AppCompatImageView>(Resource.Id.img);
            status = view.FindViewById<MaterialTextView>(Resource.Id.status);
            username = view.FindViewById<MaterialTextView>(Resource.Id.username);
            date = view.FindViewById<MaterialTextView>(Resource.Id.date);
            type = view.FindViewById<MaterialTextView>(Resource.Id.type);
            description = view.FindViewById<MaterialTextView>(Resource.Id.description);
            coordinates = view.FindViewById<MaterialTextView>(Resource.Id.coordinates);

            BtnChangeStatus = view.FindViewById<MaterialButton>(Resource.Id.BtnChangeStatus);
            BtnLocation = view.FindViewById<MaterialButton>(Resource.Id.BtnLocation);

            img_close_dialog.Click += delegate
            {
                Dialog.Dismiss();
            };

            BtnChangeStatus.Click += delegate
            {
                ChangeStatus();
            };

            BtnLocation.Click += delegate
            {
                NavigateToBuilding(lat,lon);
            };
        }

        private void GetIncidentInfo(View view)
        {
            try
            {
                

                CrossCloudFirestore
                .Current
                .Instance
                .Collection("Incidents")
                .Document(Id)
                .AddSnapshotListener((snapshot, error) =>
                {
                    if (error != null || !snapshot.Exists)
                    {
                        // Handle errors or no snapshot
                        return;
                    }

                    var incident = snapshot.ToObject<Incident>();

                    if (incident == null)
                    {
                        // Handle null incident
                        return;
                    }

                    status.Text = incident.Status;
                    date.Text = incident.DateReported.ToString();
                    description.Text = incident.Description;
                    coordinates.Text = $"{incident.Coordinates.Latitude},{incident.Coordinates.Longitude}";

                    if (incident.IncidentTypeId != null)
                    {
                        CrossCloudFirestore
                        .Current
                        .Instance
                        .Collection("IncidentType")
                        .Document(incident.IncidentTypeId)
                        .AddSnapshotListener((s, e) =>
                        {
                            if (e == null && s.Exists)
                            {
                                var t = s.ToObject<IncidentType>();
                                type.Text = t.IncidentsName;
                            }
                        });
                    }

                    if (incident.ContentUrl != null)
                    {
                        Picasso.Get()
                               .Load(incident.ContentUrl)
                               .Fit()
                               .CenterCrop()
                               .Into(img);
                    }
                });


            }
            catch (Exception ex)
            {
                AndHUD.Shared.ShowError(mContext, ex.Message, MaskType.Black, TimeSpan.FromSeconds(3));
            }
        }

        private void GetUserInfo(string id)
        {
            CrossCloudFirestore
            .Current
            .Instance
            .Collection("Incidents")
            .Document(id)
            .AddSnapshotListener(async (snapshot, error) =>
            { 
                if (snapshot.Exists)
                {
                    var i = snapshot.ToObject<Incident>();

                    if(i != null)
                    {
                        CrossCloudFirestore
                        .Current
                        .Instance
                        .Collection("USERS")
                        .Document(i.UserId)
                        .AddSnapshotListener((snapshot, error) =>
                        {
                            if (snapshot.Exists)
                            {
                                var doc = snapshot.ToObject<User>();
                                username.Text = $"{doc.FirstName} {doc.LastName}";
                            }
                        });
                    }
                }
            });
        }

        private void ChangeStatus()
        {
            Dictionary<string, object> dict = new Dictionary<string, object>();

            MaterialAlertDialogBuilder builder = new MaterialAlertDialogBuilder(mContext);
            builder.SetTitle("Change Status");
            builder.SetMessage("Change the incident report...");
            builder.SetPositiveButton("Complete", (s, e) =>
            {
                dict.Add("Status", "COMPLETED");
                CrossCloudFirestore
                .Current
                .Instance
                .Collection("Incidents")
                .Document(Id)
                .UpdateAsync(dict);

            }).SetNegativeButton("In-progress", (s, e) =>
            {
                dict.Add("Status", "IN-PROGRESS");

                CrossCloudFirestore
                .Current
                .Instance
                .Collection("Incidents")
                .Document(Id)
                .UpdateAsync(dict);
            }).SetNeutralButton("Cancel", (s, e) =>
            {
                builder.Dispose();
            });
            builder.Show();
        }

        public async void NavigateToBuilding(double lat, double lon)
        {
            try
            {
                var location = new Location(lat, lon);
                var options = new MapLaunchOptions { NavigationMode = NavigationMode.Driving };

                await Map.OpenAsync(location, options);
            }
            catch (Exception ex)
            {
                Toast.MakeText(mContext, ex.Message, ToastLength.Long);
            }
        }
    }
}