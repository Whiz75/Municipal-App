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
using Firebase;
using Firebase.Firestore.Auth;
using Google.Android.Material.Button;
using Google.Android.Material.TextField;
using ID.IonBit.IonAlertLib;
using Municipal_App.Models;
using Plugin.CloudFirestore;
using Google.Cloud.Datastore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DialogFragment = AndroidX.Fragment.App.DialogFragment;

namespace Municipal_App.Dialogs
{
    public class AddIncidentsTypeFragment : DialogFragment
    {
        private Context mContext;
        private AppCompatImageView close_add_incident_dialog;
        private TextInputEditText AddIncidentTypeTextInput;
        private MaterialButton BtnAddIncidentType;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            // return inflater.Inflate(Resource.Layout.YourFragment, container, false);
            View view = inflater.Inflate(Resource.Layout.add_incident_type_layout, container, false);

            mContext = view.Context;
            Init(view);

            return view;
        }

        public override void OnStart()
        {
            base.OnStart();
            Dialog.Window.SetLayout(ViewGroup.LayoutParams.MatchParent, ViewGroup.LayoutParams.WrapContent);
            Dialog.SetCanceledOnTouchOutside(false);
        }

        private void Init(View view)
        {
            close_add_incident_dialog = view.FindViewById<AppCompatImageView>(Resource.Id.close_add_incidents);
            AddIncidentTypeTextInput = view.FindViewById<TextInputEditText>(Resource.Id.AddIncidentTextInput);
            BtnAddIncidentType = view.FindViewById<MaterialButton>(Resource.Id.BtnAddIncident);

            BtnAddIncidentType.Click += BtnAddIncidentType_Click;
            close_add_incident_dialog.Click += Close_add_incident_dialog_Click;
        }

        private void Close_add_incident_dialog_Click(object sender, EventArgs e)
        {
            Dialog.Dismiss();
        }

        private void BtnAddIncidentType_Click(object sender, EventArgs e)
        {
            AddIncidentsAsync();
        }

        private async void AddIncidentsAsync()
        {
           
                if (string.IsNullOrEmpty(AddIncidentTypeTextInput.Text.Trim()))
                {
                    AndHUD.Shared.ShowError(mContext, "Incident field can not be empty", MaskType.None, TimeSpan.FromSeconds(1));
                }
                else
                {
                    IncidentType type = new IncidentType()
                    {
                        IncidentsName = AddIncidentTypeTextInput.Text
                    };

                    try
                    {
                        var result = await CrossCloudFirestore
                        .Current
                         .Instance
                         .Collection(nameof(IncidentType))
                         .AddAsync(type);

                        AndHUD.Shared.ShowSuccess(mContext, "Incident was added successfully", MaskType.None, TimeSpan.FromSeconds(1));
                        AddIncidentTypeTextInput.Text = "";

                        //if (result != null)
                        //{
                        //    IncidentType id = new IncidentType()
                        //    {
                        //        Id = result.Id
                        //    };

                        //    await CrossCloudFirestore.Current
                        //     .Instance
                        //     .Collection(nameof(IncidentType))
                        //     .Document(result.Id)
                        //     .UpdateAsync(id);
                        //}
                    }
                    catch (Exception ex)
                    {
                        AndHUD.
                          Shared
                          .ShowError(mContext, ex.Message, MaskType.Black, TimeSpan.FromSeconds(10));
                    }
                    
                }
        }
    }
}