using Android.Content;
using Android.Graphics;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.Widget;
using AndroidX.RecyclerView.Widget;
using Google.Android.Material.Button;
using Google.Android.Material.TextView;
using Municipal_App.Models;
using Plugin.CloudFirestore;
using Square.Picasso;
using System;
using System.Collections.Generic;

namespace Municipal_App.Adapters
{
    internal class Adapter1 : RecyclerView.Adapter
    {
        public event EventHandler<Adapter1ClickEventArgs> ItemClick;
        public event EventHandler<Adapter1ClickEventArgs> btnClick;
        public event EventHandler<Adapter1ClickEventArgs> ItemLongClick;

        private Context mContext;
        private List<Incident> incidents = new List<Incident>();
        private AndroidX.Fragment.App.FragmentManager childFragmentManager;


        public Adapter1(List<Incident> incidents)
        {
            this.incidents = incidents;
            //this.childFragmentManager = childFragmentManager;
        }

        // Create new views (invoked by the layout manager)
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {

            //Setup your layout here
            View itemView = null;
            var id = Resource.Layout.item_row;
            itemView = LayoutInflater.From(parent.Context).
                   Inflate(id, parent, false);

            var vh = new Adapter1ViewHolder(itemView, OnClick, OnLongClick, OnBtnClick);
            return vh;
        }

        // Replace the contents of a view (invoked by the layout manager)
        public override void OnBindViewHolder(RecyclerView.ViewHolder viewHolder, int position)
        {
            //var item = items[position];

            // Replace the contents of the view with that element
            //var holder = viewHolder as Adapter1ViewHolder;
            //holder.TextView.Text = items[position];

            var vh = viewHolder as Adapter1ViewHolder;

            mContext = vh.ItemView.Context;
            var incident = incidents[position];

            vh.severity.Text = incident.Severity;
            vh.date.Text = $"DATE REPORTED:{incident.DateReported}";

            if (incident.Description != null)
            {
                vh.description.Text = incident.Description;
            }
            else
            {
                vh.description.Visibility = ViewStates.Gone;
                vh.comment.Visibility = ViewStates.Gone;
            }

            vh.status.Text = incident.Status;

            if (incident.Status != null)
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

            //vh.BtnReviewIncident.Click += delegate
            //{
            //    LocationDialogFragment location = new LocationDialogFragment(incident.Id, incident.Coordinates.Latitude,
            //        incident.Coordinates.Longitude);
            //    location.Show(childFragmentManager.BeginTransaction(), "");
            //};

            try
            {
                CrossCloudFirestore
                .Current
                .Instance
                .Collection("IncidentType")
                .Document(incident.IncidentTypeId)
                .AddSnapshotListener((snapshot, error) =>
                {
                    if (snapshot.Exists)
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
            catch (Exception ex)
            {
                Toast.MakeText(mContext, ex.Message, ToastLength.Short).Show();
            }
        }

        public override int ItemCount => incidents.Count;

        void OnClick(Adapter1ClickEventArgs args) => ItemClick?.Invoke(this, args);
        void OnBtnClick(Adapter1ClickEventArgs args) => btnClick?.Invoke(this, args);
        void OnLongClick(Adapter1ClickEventArgs args) => ItemLongClick?.Invoke(this, args);

    }

    public class Adapter1ViewHolder : RecyclerView.ViewHolder
    {
        //public TextView TextView { get; set; }
        public MaterialTextView severity;
        public MaterialTextView date;
        public MaterialTextView description;
        public MaterialTextView comment;
        public MaterialTextView type;
        public MaterialTextView status;
        public AppCompatImageView image;
        public MaterialButton BtnReviewIncident;

        public Adapter1ViewHolder(View itemView, Action<Adapter1ClickEventArgs> clickListener,
                            Action<Adapter1ClickEventArgs> longClickListener, 
                            Action<Adapter1ClickEventArgs> btnClickListener) : base(itemView)
        {
            //TextView = v;
            severity = itemView.FindViewById<MaterialTextView>(Resource.Id.severity);
            date = itemView.FindViewById<MaterialTextView>(Resource.Id.date);
            description = itemView.FindViewById<MaterialTextView>(Resource.Id.description);
            comment = itemView.FindViewById<MaterialTextView>(Resource.Id.comment);
            type = itemView.FindViewById<MaterialTextView>(Resource.Id.type);
            status = itemView.FindViewById<MaterialTextView>(Resource.Id.status);
            image = itemView.FindViewById<AppCompatImageView>(Resource.Id.img);

            BtnReviewIncident = itemView.FindViewById<MaterialButton>(Resource.Id.BtnReviewIncident);

            itemView.Click += (sender, e) => clickListener(new Adapter1ClickEventArgs { View = itemView, Position = AbsoluteAdapterPosition });
            BtnReviewIncident.Click += (sender, e) => btnClickListener(new Adapter1ClickEventArgs { View = itemView, Position = AbsoluteAdapterPosition });
            itemView.LongClick += (sender, e) => longClickListener(new Adapter1ClickEventArgs { View = itemView, Position = AbsoluteAdapterPosition });
        }
    }

    public class Adapter1ClickEventArgs : EventArgs
    {
        public View View { get; set; }
        public int Position { get; set; }
    }
}