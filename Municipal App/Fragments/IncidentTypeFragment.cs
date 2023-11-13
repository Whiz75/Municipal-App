using Android.OS;
using Android.Views;
using AndroidHUD;
using Municipal_App.Adapters;
using Municipal_App.Models;
using Plugin.CloudFirestore;
using System.Collections.Generic;
using System;
using Fragment = AndroidX.Fragment.App.Fragment;
using RecyclerView = AndroidX.RecyclerView.Widget.RecyclerView;
using Android.Content;
using LinearLayoutManager = AndroidX.RecyclerView.Widget.LinearLayoutManager;
using Google.Android.Material.TextView;

namespace Municipal_App.Fragments
{
    public class IncidentTypeFragment : Fragment
    {
        Context mContext;
        private List<IncidentType> incidentTypeList;

        private MaterialTextView noOfIncidentType;
        private RecyclerView incidentTypeRecyclerView;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            // return inflater.Inflate(Resource.Layout.YourFragment, container, false);
            View view = inflater.Inflate(Resource.Layout.incident_type_layout, container, false);
            Init(view);
            LoadIncidentType();

            return view;
        }

        private void Init(View v)
        {
            mContext = v.Context;

            noOfIncidentType = v.FindViewById<MaterialTextView>(Resource.Id.noOfIncidentType);
            incidentTypeRecyclerView = v.FindViewById<RecyclerView>(Resource.Id.incidentTypeRecyclerView);
        }

        private void LoadIncidentType()
        {
            incidentTypeList = new List<IncidentType>();
            IncidentTypeAdapter mAdapter = new IncidentTypeAdapter(incidentTypeList, ChildFragmentManager);

            RecyclerView.LayoutManager layoutManager = new LinearLayoutManager(mContext);
            incidentTypeRecyclerView.SetLayoutManager(layoutManager);

            incidentTypeRecyclerView.HasFixedSize = true;
            incidentTypeRecyclerView.SetAdapter(mAdapter);

            try
            {
                CrossCloudFirestore
                .Current
                .Instance
                .Collection("IncidentType")
                .AddSnapshotListener((snapshot, error) =>
                {
                    if (!snapshot.IsEmpty)
                    {
                        foreach (var item in snapshot.DocumentChanges)
                        {
                            var type = item.Document.ToObject<IncidentType>();
                            switch (item.Type)
                            {
                                case DocumentChangeType.Added:
                                    incidentTypeList.Add(type);
                                    mAdapter.NotifyDataSetChanged();
                                    break;
                                case DocumentChangeType.Modified:
                                    break;
                                case DocumentChangeType.Removed:
                                    break;
                            }
                        }

                        //No. of incident types in the database
                        noOfIncidentType.Text = $"NO. OF INCIDENT TYPES:{incidentTypeList.Count}";
                    }
                });

                mAdapter.NotifyDataSetChanged();
            }
            catch (Exception ex)
            {
                AndHUD.Shared.ShowError(mContext, ex.Message, MaskType.Black, TimeSpan.FromSeconds(3));
            }
        }
    }
}