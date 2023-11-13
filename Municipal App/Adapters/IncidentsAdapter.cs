using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.Widget;
using AndroidX.RecyclerView.Widget;
using Firebase.Firestore.Auth;
using Google.Android.Material.Button;
using Google.Android.Material.TextField;
using Google.Android.Material.TextView;
using IsmaelDiVita.ChipNavigationLib.Utils;
using Municipal_App.Dialogs;
using Municipal_App.Models;
using Plugin.CloudFirestore;
using Square.Picasso;
using System;
using System.Collections.Generic;
using XamarinTextDrawable;
using User = Municipal_App.Models.User;

namespace Municipal_App.Adapters
{
    public class IncidentsAdapter : RecyclerView.Adapter
    {
        private Context mContext;
        private List<Incident> incidents;
        private AndroidX.Fragment.App.FragmentManager childFragmentManager;

        public IncidentsAdapter(List<Incident> incidents, AndroidX.Fragment.App.FragmentManager childFragmentManager)
        {
            this.incidents = incidents;
            this.childFragmentManager = childFragmentManager;
        }

        public override int ItemCount => incidents.Count;

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            IncidentViewHolder vh = holder as IncidentViewHolder;

            mContext = vh.ItemView.Context;
            var incident = incidents[position];

            vh.severity.Text = incident.Severity;
            vh.date.Text = $"DATE REPORTED:{incident.DateReported}";

            if(incident.Description != null)
            {
                vh.description.Text = incident.Description;
            }
            else
            {
                vh.description.Visibility = ViewStates.Gone;
                vh.comment.Visibility = ViewStates.Gone;
            }
            
            vh.status.Text = incident.Status;

            if(incident.Status != null)
            {
                //handle incident status colors
                if (incident.Status == "PENDING")
                {
                    //red
                    vh.status.SetTextColor(Color.ParseColor("#FF8C00"));
                }
                else if (incident.Status == "IN-PROGRESS")
                {
                    //orange
                    vh.status.SetTextColor(Color.ParseColor("#4caf50"));

                }
                else if (incident.Status == "COMPLETED")
                {
                    //green
                    vh.status.SetTextColor(Color.ParseColor("#A9A9A9"));
                    vh.BtnReviewIncident.Visibility = ViewStates.Gone;
                }
            }

            vh.BtnReviewIncident.Click += delegate
            {
                LocationDialogFragment location = new LocationDialogFragment(incident.Id, incident.Coordinates.Latitude,
                    incident.Coordinates.Longitude);
                location.Show(childFragmentManager.BeginTransaction(), "");
            };

            try
            {
                CrossCloudFirestore
                .Current
                .Instance
                .Collection("IncidentType")
                .Document(incident.IncidentTypeId)
                .AddSnapshotListener((snapshot, error) =>
                {
                    if(snapshot.Exists)
                    {
                        var typ = snapshot.ToObject<IncidentType>();
                        vh.type.Text = typ.IncidentsName;
                    }
                });

                if (incident.ContentUrl != null)
                {
                    Picasso.Get().Load(incident.ContentUrl).Fit()
                        .CenterCrop()
                        .Into(vh.image);
                }
                else
                {
                    // Set the drawable to the ImageView
                    vh.image.SetImageResource(Resource.Drawable.no_image);
                }
            }
            catch(Exception ex)
            {
                Toast.MakeText(mContext,ex.Message, ToastLength.Short).Show();
            }
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View view = LayoutInflater.FromContext(parent.Context).Inflate(Resource.Layout.item_row, parent, false);
            IncidentViewHolder vh = new IncidentViewHolder(view);
            return vh;
        }

        public class IncidentViewHolder: RecyclerView.ViewHolder
        {
            public MaterialTextView severity;
            public MaterialTextView date;
            public MaterialTextView description;
            public MaterialTextView comment;
            public MaterialTextView type;
            public MaterialTextView status;
            public AppCompatImageView image;
            public MaterialButton BtnReviewIncident;

            public IncidentViewHolder(View itemView) : base(itemView)
            {
                severity = itemView.FindViewById<MaterialTextView>(Resource.Id.severity);
                date = itemView.FindViewById<MaterialTextView>(Resource.Id.date);
                description = itemView.FindViewById<MaterialTextView>(Resource.Id.description);
                comment = itemView.FindViewById<MaterialTextView>(Resource.Id.comment);
                type = itemView.FindViewById<MaterialTextView>(Resource.Id.type);
                status = itemView.FindViewById<MaterialTextView>(Resource.Id.status);
                image = itemView.FindViewById<AppCompatImageView>(Resource.Id.img);

                BtnReviewIncident = itemView.FindViewById<MaterialButton>(Resource.Id.BtnReviewIncident);
            }
        }
    }
}