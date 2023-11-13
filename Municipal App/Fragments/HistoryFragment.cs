using Android.Content;
using Android.OS;
using Android.Views;
using AndroidHUD;
using Municipal_App.Adapters;
using Municipal_App.Models;
using Plugin.CloudFirestore;
using System;
using System.Collections.Generic;
using Fragment = AndroidX.Fragment.App.Fragment;
using LinearLayoutManager = AndroidX.RecyclerView.Widget.LinearLayoutManager;
using RecyclerView = AndroidX.RecyclerView.Widget.RecyclerView;

namespace Municipal_App.Fragments
{
    public class HistoryFragment : Fragment
    {
        private Context mContext;
        private RecyclerView historyRecyclerView;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            // return inflater.Inflate(Resource.Layout.YourFragment, container, false);
            View view = inflater.Inflate(Resource.Layout.history_fragment, container, false);
            Init(view);
            LoadIncidents();

            return view;
        }

        private void Init(View view)
        {
            mContext = view.Context;

            //mFrameLayout = view.FindViewById<ShimmerFrameLayout>(Resource.Id.shimmer_layout);
            historyRecyclerView = view.FindViewById<RecyclerView>(Resource.Id.historyRecyclerView);
        }

        private void LoadIncidents()
        {
            List<Incident> incidentsList = new List<Incident>();
            IncidentsAdapter mAdapter = new IncidentsAdapter(incidentsList, ChildFragmentManager);

            RecyclerView.LayoutManager layoutManager = new LinearLayoutManager(mContext);
            historyRecyclerView.SetLayoutManager(layoutManager);

            historyRecyclerView.HasFixedSize = true;
            historyRecyclerView.SetAdapter(mAdapter);

            //mFrameLayout.StartShimmer();
            try
            {
                        CrossCloudFirestore
                        .Current
                        .Instance
                        .Collection("Incidents")
                        .WhereEqualsTo("Status", "COMPLETED")
                        .AddSnapshotListener((snapshot, error) =>
                        {
                            if (!snapshot.IsEmpty)
                            {
                                foreach(var i in snapshot.DocumentChanges)
                                {
                                    var history = i.Document.ToObject<Incident>();
                                    switch (i.Type)
                                    {
                                        case DocumentChangeType.Added:
                                            incidentsList.Add(history);
                                            mAdapter.NotifyDataSetChanged();
                                            break;
                                        case DocumentChangeType.Modified:
                                            break;
                                        case DocumentChangeType.Removed:
                                            break;
                                    }
                                }
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