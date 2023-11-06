using Android.App;
using Android.Content;
using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.Locations;
using Android.OS;
using Android.Runtime;
using Android.Systems;
using Android.Util;
using Android.Views;
using Android.Views.Animations;
using Android.Widget;
using AndroidHUD;
using AndroidX.CardView.Widget;
using AndroidX.Core.Content;
using Firebase.Firestore.Auth;
using Google.Android.Material.BottomSheet;
using Google.Android.Material.Button;
using Google.Android.Material.TextField;
using Google.Android.Material.TextView;
using Municipal_App.Models;
using OSRMLib.OSRMServices;
using Plugin.CloudFirestore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DialogFragment = AndroidX.Fragment.App.DialogFragment;
using User = Municipal_App.Models.User;

namespace Municipal_App.Dialogs
{
    public class LocationDialogFragment : DialogFragment, IOnMapReadyCallback
    {
        private Context context;
        private CardView bottomSheetCardview1;
        private TextInputEditText txt_address;
        private TextInputEditText txt_dest_address;
        private MaterialTextView reporterTv;
        private MaterialTextView EstDistanceTv;
        private MaterialTextView TxtDuration;
        private MaterialButton BtnUpdateStatus;
        private SupportMapFragment mapFrag;

        private readonly string Id;
        private double Lat;
        private double Lng;

        public LocationDialogFragment(string id, double lat, double lon)
        {
            Id = id;
            Lat = lat;
            Lng = lon;
        }

        AlphaAnimation alphaAnimation;
        TranslateAnimation slideAnimation;
        public override void OnStart()
        {
            base.OnStart();
            Dialog.Window.SetLayout(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.MatchParent);

            try
            {
                // Calculate the screen height
                int screenHeight = Resources.DisplayMetrics.HeightPixels;

                // Create a TranslateAnimation that moves the view from the bottom to the top
                TranslateAnimation slideAnimation = new TranslateAnimation(0, 0, screenHeight, 0);
                slideAnimation.Duration = 2000; // Duration of the animation in milliseconds
                bottomSheetCardview1.StartAnimation(slideAnimation); // Start the animation
            }
            catch (Exception ex)
            {
                AndHUD.Shared.ShowError(context, ex.Message, MaskType.Black, TimeSpan.FromSeconds(2));
            }
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
            View view = inflater.Inflate(Resource.Layout.location_dialog_layout, container, false);
            Init(view);
            ChangeStatus();
            GetUserInfo(Id);

            return view;
        }

        private void Init(View view)
        {
            context = view.Context;
            bottomSheetCardview1 = view.FindViewById<CardView>(Resource.Id.cardview1);
            txt_address = view.FindViewById<TextInputEditText>(Resource.Id.txt_current_address);
            txt_dest_address = view.FindViewById<TextInputEditText>(Resource.Id.txt_dest_address);
            reporterTv = view.FindViewById<MaterialTextView>(Resource.Id.reporterTv);
            EstDistanceTv = view.FindViewById<MaterialTextView>(Resource.Id.EstDistanceTv);
            TxtDuration = view.FindViewById<MaterialTextView>(Resource.Id.TxtDuration);
            BtnUpdateStatus = view.FindViewById<MaterialButton>(Resource.Id.BtnUpdateStatus);

            mapFrag = ChildFragmentManager.FindFragmentById(Resource.Id.fragMap).JavaCast<SupportMapFragment>();
            mapFrag.GetMapAsync(this);
        }

        //private void ChangeStatus()
        //{
        //    try
        //    {
        //        CrossCloudFirestore
        //            .Current
        //            .Instance
        //            .Collection("Incidents")
        //            .Document(Id)
        //            .AddSnapshotListener((snapshot, error) =>
        //            {
        //                if(snapshot.Exists)
        //                {
        //                    var d = snapshot.ToObject<Incident>();
        //                    Dictionary<string, object> dict = new Dictionary<string, object>();

        //                    if (d.Status == "PENDING")
        //                    {
        //                        BtnUpdateStatus.Text = "APPROVE";
        //                        BtnUpdateStatus.Enabled = true;
        //                        BtnUpdateStatus.Click += delegate
        //                        {
        //                            dict.Add("Status", "IN-PROGRESS");
        //                            CrossCloudFirestore
        //                            .Current
        //                            .Instance
        //                            .Collection("Incidents")
        //                            .Document(Id)
        //                            .UpdateAsync(dict);
        //                        };
        //                    }
        //                    else if(d.Status == "IN-PROGRESS")
        //                    {
        //                        BtnUpdateStatus.Text = "COMPLETE";
        //                        BtnUpdateStatus.Enabled = true;
        //                        BtnUpdateStatus.SetBackgroundColor(Android.Graphics.Color.ParseColor("#738b28"));
        //                        BtnUpdateStatus.Click += delegate
        //                        {
        //                            dict.Add("Status", "COMPLETED");
        //                            CrossCloudFirestore
        //                            .Current
        //                            .Instance
        //                            .Collection("Incidents")
        //                            .Document(Id)
        //                            .UpdateAsync(dict);
        //                        };
        //                    }
        //                    else if(d.Status == "COMPLETED")
        //                    {
        //                        BtnUpdateStatus.Enabled = false;
        //                    }
        //                }
        //            });
        //    }catch(Exception ex)
        //    {
        //        AndHUD.Shared.ShowError(context,ex.Message,MaskType.Black,TimeSpan.FromSeconds(2));
        //    }
        //}

        private void ChangeStatus()
        {
            try
            {
                CrossCloudFirestore
                    .Current.Instance
                    .Collection("Incidents")
                    .Document(Id)
                    .AddSnapshotListener((snapshot, error) =>
                    {
                        if (error != null)
                        {
                            AndHUD.Shared.ShowError(context, error.Message, MaskType.Black, TimeSpan.FromSeconds(2));
                            return;
                        }

                        if (snapshot.Exists)
                        {
                            var incident = snapshot.ToObject<Incident>();
                            UpdateUIForIncidentStatus(incident.Status);
                        }
                    });
            }
            catch (Exception ex)
            {
                AndHUD.Shared.ShowError(context, ex.Message, MaskType.Black, TimeSpan.FromSeconds(2));
            }
        }

        //
        private void UpdateUIForIncidentStatus(string status)
        {
            BtnUpdateStatus.Enabled = true;

            switch (status)
            {
                case "PENDING":
                    BtnUpdateStatus.Text = "APPROVE";
                    BtnUpdateStatus.SetBackgroundColor(Android.Graphics.Color.ParseColor("#ffb74d"));
                    BtnUpdateStatus.Click += delegate
                    {
                        UpdateIncidentStatus("IN-PROGRESS");
                    };
                    break;
                case "IN-PROGRESS":
                    BtnUpdateStatus.Text = "COMPLETE";
                    BtnUpdateStatus.SetBackgroundColor(Android.Graphics.Color.ParseColor("#4caf50"));
                    BtnUpdateStatus.Click += delegate
                    {
                        UpdateIncidentStatus("COMPLETED");
                    };
                    break;
                case "COMPLETED":
                    BtnUpdateStatus.Enabled = false;
                    BtnUpdateStatus.Text = "DISMISS";
                    BtnUpdateStatus.SetBackgroundColor(Android.Graphics.Color.ParseColor("#e0e0e0"));
                    break;
            }
        }

        //method to update status
        private async void UpdateIncidentStatus(string newStatus)
        {

            var dict = new Dictionary<string, object> { { "Status", newStatus } };

            await CrossCloudFirestore
                .Current
                .Instance
                .Collection("Incidents")
                .Document(Id)
                .UpdateAsync(dict);
        }


        private GoogleMap googleMap;
        private LatLng latLng;
        private LatLng LatLon2;

        public   async void OnMapReady(GoogleMap googleMap)
        {
            this.googleMap = googleMap;
            try
            {
                this.googleMap.UiSettings.ZoomControlsEnabled = true;

                var location = await Xamarin.Essentials.Geolocation.GetLocationAsync();

                if (location != null)
                {
                    this.googleMap.AnimateCamera(CameraUpdateFactory.NewLatLngZoom(new LatLng(location.Latitude, location.Longitude), 17));
                    latLng = new LatLng(location.Latitude, location.Longitude);
                    LatLon2 = new LatLng(Lat, Lng);

                    var current = new OSRMLib.Helpers.Location(location.Latitude, location.Longitude);
                    var dest = new OSRMLib.Helpers.Location(Lat, Lng);

                    try
                    {
                        //latLng = new LatLng(location.Latitude, .Longitude);
                        var result = await ReverseGeocodeCurrentLocation(location.Latitude, location.Longitude);
                        var result1 = await ReverseGeocodeCurrentLocation(Lat, Lng);

                        txt_address.Text = result?.GetAddressLine(0);
                        txt_dest_address.Text = result1?.GetAddressLine(0);
                    }
                    catch (Exception ex)
                    {
                        Toast.MakeText(context, ex.Message, ToastLength.Long).Show();
                    }

                    GetRoute(current, dest);
                }
            }
            catch (Exception ex)
            {
                AndHUD.Shared.ShowError(context,ex.Message,MaskType.None,TimeSpan.FromSeconds(3));
            }
        }

        async Task<Address> ReverseGeocodeCurrentLocation(double lat, double lon)
        {
            Geocoder geocoder = new Geocoder(context);
            IList<Address> addressList = await geocoder.GetFromLocationAsync(lat, lon, 5);

            Address address = addressList?.FirstOrDefault();
            return address;
        }

        RouteService routeS = new RouteService();
        private double CalculatedTripPrice = 0.0;
        private double CalculatedTripDistance = 0.0;
        public async void GetRoute(OSRMLib.Helpers.Location startPos, OSRMLib.Helpers.Location endPos)
        {
            googleMap.Clear();

            try
            {
                routeS.Coordinates = new List<OSRMLib.Helpers.Location> { startPos, endPos };

                var response = await routeS.Call();

                var points = response.Routes[0].Geometry;
                Java.Util.ArrayList routeList = new Java.Util.ArrayList();
                foreach (var point in points)
                {
                    routeList.Add(new LatLng(point.Latitude, point.Longitude));
                }
                PolylineOptions polylineOptions = new PolylineOptions()
                    .AddAll(routeList)
                    .InvokeWidth(10)
                    .InvokeColor(Resource.Color.material_blue_grey_800)
                    .InvokeStartCap(new SquareCap())
                    .InvokeEndCap(new SquareCap())
                    .InvokeJointType(JointType.Round)
                    .Geodesic(true);
                googleMap.AddPolyline(polylineOptions);

                LatLng firstpoint = new LatLng(startPos.Latitude, startPos.Longitude);
                LatLng lastpoint = new LatLng(endPos.Latitude, endPos.Longitude);

                //Pickup marker options
                MarkerOptions pickupMarkerOptions = new MarkerOptions();
                pickupMarkerOptions.SetPosition(firstpoint);
                //pickupMarkerOptions.SetTitle("Pickup Location");

                pickupMarkerOptions.SetIcon(BitmapDescriptorFactory.DefaultMarker(BitmapDescriptorFactory.HueGreen));


                //Destination marker options
                MarkerOptions destinationMarkerOptions = new MarkerOptions();
                destinationMarkerOptions.SetPosition(lastpoint);
                destinationMarkerOptions.SetTitle("Destination");
                destinationMarkerOptions.SetIcon(BitmapDescriptorFactory.DefaultMarker(BitmapDescriptorFactory.HueRed));

                Marker pickupMarker = googleMap.AddMarker(pickupMarkerOptions);

                googleMap.AddMarker(destinationMarkerOptions);

                double radiusDegrees = 0.10;
                LatLng northEast = new LatLng(startPos.Latitude + radiusDegrees, startPos.Longitude + radiusDegrees);
                LatLng southWest = new LatLng(endPos.Latitude - radiusDegrees, endPos.Longitude - radiusDegrees);
                LatLngBounds bounds = new LatLngBounds(southWest, northEast);
                googleMap.AnimateCamera(CameraUpdateFactory.NewLatLngZoom(firstpoint, 15));
                googleMap.SetPadding(40, 100, 40, 70);
                pickupMarker.ShowInfoWindow();

                //calculate estimated distance
                double distance = Math.Round(response.Routes[0].Legs[0].Distance / 1000, 2);
                EstDistanceTv.Text = $"Est. Distance: {distance} KM";
                CalculatedTripDistance = distance;

                //calculate estimate duration
                TxtDuration.Text = $"Est. Duration:{Math.Round(response.Routes[0].Duration / 60)} Minutes";
                pickupMarkerOptions.SetTitle($"{Math.Round(response.Routes[0].Duration / 60)} Min");
                //AndHUD.Shared.ShowSuccess(context,$"R{CalculatePrice(distance)}", MaskType.Black,TimeSpan.FromSeconds(3));
            }
            catch (Exception ex)
            {
                AndHUD.Shared.ShowError(context,ex.Message, MaskType.Black,TimeSpan.FromSeconds(5));
            }

        }

        private double CalculatePrice(double distance)
        {
            //price per kilometer.
            double pricePerKilometer = 6.50; // R6.50 per kilometer
            double price = distance * pricePerKilometer;

            return price;
        }

        private void GetUserInfo(string id)
        {
            CrossCloudFirestore
            .Current
            .Instance
            .Collection("Incidents")
            .Document(id)
            .AddSnapshotListener(async (incidentSnapshot, incidentError) =>
            {
                if (incidentError != null || !incidentSnapshot.Exists)
                {
                    // Handle errors or no snapshot
                    return;
                }

                var incident = incidentSnapshot.ToObject<Incident>();

                if (incident == null)
                {
                    // Handle null incident
                    return;
                }

                if(incident.UserId != null)
                {
                    try
                    {
                        CrossCloudFirestore
                        .Current
                        .Instance
                        .Collection("USERS")
                        .Document(incident.UserId)
                        .AddSnapshotListener((snapshot, error) =>
                        {
                            if (snapshot.Exists)
                            {
                                var user = snapshot.ToObject<User>();
                                reporterTv.Text = $"{user.FirstName} {user.LastName}";
                            }
                        });
                    }
                    catch (Exception ex)
                    {
                        AndHUD.Shared.ShowError(context, ex.Message, MaskType.None, TimeSpan.FromSeconds(2));
                    }
                }
                
            });
        }

    }
}