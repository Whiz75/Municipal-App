using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using AndroidHUD;
using AndroidX.AppCompat.Widget;
using AndroidX.Fragment.App;
using Google.Android.Material.Button;
using Google.Android.Material.TextField;
using Google.Android.Material.TextView;
using Municipal_App.Models;
using Plugin.CloudFirestore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DialogFragment = AndroidX.Fragment.App.DialogFragment;

namespace Municipal_App.Dialogs
{
    public class UpdateIncidentTypeDialogFragment : DialogFragment
    {
        private Context mContext;

        private string Id;

        public UpdateIncidentTypeDialogFragment(string Id)
        {
            this.Id = Id;
        }
        private AppCompatImageView close_update_incident_type;
        private MaterialTextView incident_type_name;
        private TextInputEditText UpdateIncidentTextInput;
        private MaterialButton BtnUpdateIncident;

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

            View view = inflater.Inflate(Resource.Layout.update_incident_type_layout,container,false);
            Init(view);
            ShowIncidentTypeName();

            return view;
        }

        private void Init(View v)
        {
            mContext = v.Context;

            close_update_incident_type = v.FindViewById<AppCompatImageView>(Resource.Id.close_update_incident_type);

            close_update_incident_type.Click += delegate
            {
                Dialog.Dismiss();
            };

            incident_type_name = v.FindViewById<MaterialTextView>(Resource.Id.incident_type_name);
            UpdateIncidentTextInput = v.FindViewById<TextInputEditText>(Resource.Id.UpdateIncidentTextInput);

            BtnUpdateIncident = v.FindViewById<MaterialButton>(Resource.Id.BtnUpdateIncident);

            BtnUpdateIncident.Click += (s, e) =>
            {
                UpdateIncidentType();
            };
        }

        private void ShowIncidentTypeName()
        {
            //display the incident type
            CrossCloudFirestore
            .Current
            .Instance
            .Collection("IncidentType")
            .Document(Id)
            .AddSnapshotListener((value, error) =>
            {
                if (value.Exists)
                {
                    var t = value.ToObject<IncidentType>();
                    incident_type_name.Text = $"INCIDENT NAME:{t.IncidentsName}";
                }
            });
        }

        private async void UpdateIncidentType()
        {
            // Create a dictionary with the incident type fields
            Dictionary<string, object> type = new Dictionary<string, object>
            {
                { "IncidentsName", UpdateIncidentTextInput.Text }
            };

            try
            {
                //update incident type
                await CrossCloudFirestore.Current
                         .Instance
                         .Collection("IncidentType")
                         .Document(Id)
                         .UpdateAsync(type);

                AndHUD.Shared.ShowSuccess(mContext,"Incident type successfully updated!!!");
            }
            catch(Exception e)
            {
                Toast.MakeText(mContext,e.Message,ToastLength.Long).Show();
            }
        }
    }
}