using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Views.Animations;
using AndroidHUD;
using AndroidX.AppCompat.Widget;
using AndroidX.CardView.Widget;
using AndroidX.RecyclerView.Widget;
using Facebook.Shimmer;
using Google.Android.Material.TextView;
using Municipal_App.Adapters;
using Municipal_App.Dialogs;
using Municipal_App.Models;
using Plugin.CloudFirestore;
using System;
using System.Collections.Generic;
using Fragment = AndroidX.Fragment.App.Fragment;

namespace Municipal_App.Fragments
{
    public class HomeFragment : Fragment
    {
        private Context mContext;

        private AppCompatImageView cancelNoOfIncident;
        private CardView cardView1;
        private ShimmerFrameLayout mFrameLayout;
        private MaterialTextView noOfIncident;
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
            cancelNoOfIncident = view.FindViewById<AppCompatImageView>(Resource.Id.cancelNoOfIncident);

            cardView1 = view.FindViewById<CardView>(Resource.Id.cardView1);

            mFrameLayout = view.FindViewById<ShimmerFrameLayout>(Resource.Id.shimmer_layout);
            noOfIncident = view.FindViewById<MaterialTextView>(Resource.Id.noOfIncident);
            recyclerView = view.FindViewById<RecyclerView>(Resource.Id.rv_incidents);

            cancelNoOfIncident.Click += delegate
            {
                // Calculate the screen height
                int screenHeight = Resources.DisplayMetrics.HeightPixels;

                // Create a TranslateAnimation that moves the view from the bottom to the top
                TranslateAnimation slideAnimation = new TranslateAnimation(0, 0, 0, screenHeight);
                slideAnimation.Duration = 2000; // Duration of the animation in milliseconds
                cardView1.StartAnimation(slideAnimation); // Start the animation

                cardView1.Visibility = ViewStates.Gone;
            };
        }

        private void LoadIncidents()
        {
            List<Incident> incidentsList = new List<Incident>();
            Adapter1 mAdapter = new Adapter1(incidentsList);

            RecyclerView.LayoutManager layoutManager = new LinearLayoutManager(mContext);
            recyclerView.SetLayoutManager(layoutManager);

            recyclerView.HasFixedSize = true;
            recyclerView.SetAdapter(mAdapter);

            mAdapter.btnClick += (s,e) =>
            {
                LocationDialogFragment location = new LocationDialogFragment(incidentsList[e.Position].Id,
                    incidentsList[e.Position].Coordinates.Latitude, incidentsList[e.Position].Coordinates.Longitude);
                location.Show(ChildFragmentManager.BeginTransaction(), "");
            };

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
                        if (!value.IsEmpty)
                        {
                            foreach (var item in value.DocumentChanges)
                            {
                                var j = item.Document.ToObject<Incident>();

                                switch (item.Type)
                                {
                                    case DocumentChangeType.Added:
                                        incidentsList.Add(j);
                                        mAdapter.NotifyDataSetChanged();
                                        break;
                                    case DocumentChangeType.Modified:
                                        if(j.Status == "COMPLETED")
                                        {
                                            incidentsList.RemoveAll(x => x.Id == j.Id);
                                        }
                                        else
                                        {
                                            incidentsList[item.OldIndex] = j;
                                        }

                                        mAdapter.NotifyDataSetChanged();
                                        break;
                                    case DocumentChangeType.Removed:
                                        break;
                                }
                            }
                            noOfIncident.Text = $"YOU HAVE {incidentsList.Count} INCIDENTS PENDING YOUR APPROVAL";
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
                mFrameLayout.Visibility = ViewStates.Gone;
            }
        }

    }
}