using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using AndroidHUD;
using AndroidX.Fragment.App;
using AndroidX.RecyclerView.Widget;
using Facebook.Shimmer;
using IO.SuperCharge.ShimmerLayoutLib;
using Municipal_App.Activities;
using Municipal_App.Adapters;
using Municipal_App.Models;
using Plugin.CloudFirestore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Fragment = AndroidX.Fragment.App.Fragment;

namespace Municipal_App.Fragments
{
    public class HomeFragment : Fragment
    {
        private Context mContext;
        private ShimmerFrameLayout mFrameLayout;
        private RecyclerView recyclerView;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            // return inflater.Inflate(Resource.Layout.YourFragment, container, false);

            View view = inflater.Inflate(Resource.Layout.home_fragment, container, false);
            Init(view);
            LoadIncidents();

            return view;
        }

        private void Init(View view)
        {
            mContext = view.Context;

            mFrameLayout = view.FindViewById<ShimmerFrameLayout>(Resource.Id.shimmer_layout);
            recyclerView = view.FindViewById<RecyclerView>(Resource.Id.rv_incidents);
        }

        private void LoadIncidents()
        {
            List<Incident> incidentsList = new List<Incident>();
            IncidentsAdapter mAdapter = new IncidentsAdapter(incidentsList, ChildFragmentManager);

            RecyclerView.LayoutManager layoutManager = new LinearLayoutManager(mContext);
            recyclerView.SetLayoutManager(layoutManager);

            recyclerView.HasFixedSize = true;
            recyclerView.SetAdapter(mAdapter);

            mFrameLayout.StartShimmer();
            
            try
            {
                CrossCloudFirestore
                    .Current
                    .Instance
                    .Collection("Incidents")
                    .WhereIn("Status", new object[] { "IN-PROGRESS", "PENDING" })
                    .AddSnapshotListener((value, error) =>
                    {
                        if(!value.IsEmpty)
                        {
                            foreach(var item in value.DocumentChanges)
                            {
                                var j = item.Document.ToObject<Incident>();
                                switch (item.Type)
                                {
                                    case DocumentChangeType.Added:
                                        incidentsList.Add(j);
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
            finally
            {
                mFrameLayout.StopShimmer();
            }
        }
    }
}