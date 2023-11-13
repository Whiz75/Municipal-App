using Android.Content;
using Android.Views;
using Android.Widget;
using AndroidX.AppCompat.Widget;
using AndroidX.RecyclerView.Widget;
using Google.Android.Material.TextView;
using Municipal_App.Dialogs;
using Municipal_App.Models;
using System.Collections.Generic;

namespace Municipal_App.Adapters
{
    public class IncidentTypeAdapter : RecyclerView.Adapter
    {
        private Context mContext;
        private List<IncidentType> incidents;
        private AndroidX.Fragment.App.FragmentManager childFragmentManager;

        public IncidentTypeAdapter(List<IncidentType> incidentType, AndroidX.Fragment.App.FragmentManager childFragmentManager)
        {
            this.incidents = incidentType;
            this.childFragmentManager = childFragmentManager;
        }

        public override int ItemCount => incidents.Count;

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            IncidentTypeViewHolder vh = holder as IncidentTypeViewHolder;

            mContext = vh.ItemView.Context;
            var incident = incidents[position];

            vh.name.Text = incident.IncidentsName;
            vh.img_edit_incident.Click += (s, e) =>
            {
                //display the incident type
                Toast.MakeText(mContext,incident.Id,ToastLength.Long).Show();

                UpdateIncidentTypeDialogFragment type = new UpdateIncidentTypeDialogFragment(incident.Id);
                type.Show(childFragmentManager.BeginTransaction(), "");
            };
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            View view = LayoutInflater.FromContext(parent.Context).Inflate(Resource.Layout.incident_type_item_row, parent, false);
            IncidentTypeViewHolder vh = new IncidentTypeViewHolder(view);
            return vh;
        }

        public class IncidentTypeViewHolder: RecyclerView.ViewHolder
        {
            public MaterialTextView name;
            public AppCompatImageView img_edit_incident;

            public IncidentTypeViewHolder(View itemView) : base(itemView)
            {
                name = itemView.FindViewById<MaterialTextView>(Resource.Id.incident_type_name);
                img_edit_incident = itemView.FindViewById<AppCompatImageView>(Resource.Id.img_edit_incident);
            }
        }
    }
}