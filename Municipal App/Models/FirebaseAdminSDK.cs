using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using FirebaseAdmin.Messaging;
using Google.Apis.Auth.OAuth2;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Municipal_App.Models
{
    public static class FirebaseAdminSDK
    {
        public static FirebaseAdmin.Auth.FirebaseAuth GetFirebaseAdminAuth(Stream input)
        {
            FirebaseAdmin.Auth.FirebaseAuth auth;
            FirebaseAdmin.AppOptions options = new FirebaseAdmin.AppOptions()
            {
                Credential = GoogleCredential.FromStream(input),
                ProjectId = "municipal-f1b76",
                ServiceAccountId = "firebase-adminsdk-4rodw@municipal-f1b76.iam.gserviceaccount.com",

            };
            if (FirebaseAdmin.FirebaseApp.DefaultInstance == null)
            {
                var app = FirebaseAdmin.FirebaseApp.Create(options);
                auth = FirebaseAdmin.Auth.FirebaseAuth.GetAuth(app);
            }
            else
            {
                auth = FirebaseAdmin.Auth.FirebaseAuth.DefaultInstance;
            }
            return auth;
        }
        public static FirebaseMessaging GetFirebaseMessaging(Stream input)
        {
            FirebaseMessaging messaging;

            FirebaseAdmin.AppOptions options = new FirebaseAdmin.AppOptions()
            {
                Credential = GoogleCredential.FromStream(input),
                ProjectId = "municipal-f1b76",
                ServiceAccountId = "firebase-adminsdk-4rodw@municipal-f1b76.iam.gserviceaccount.com",

            };
            if (FirebaseAdmin.FirebaseApp.DefaultInstance == null)
            {
                var app = FirebaseAdmin.FirebaseApp.Create(options);
                messaging = FirebaseMessaging.GetMessaging(app);
            }
            else
            {
                messaging = FirebaseMessaging.DefaultInstance;//.FirebaseAuth.DefaultInstance;
            }

            return messaging;
        }
    }

}