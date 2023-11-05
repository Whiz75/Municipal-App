using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Firebase.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Municipal_App.Models
{
    [Service]
    //[IntentFilter(new[] { "com.google.firebase.MESSAGING_EVENT" })]
    public class FirebaseNotifications: FirebaseMessagingService
    {
        public override void OnMessageReceived(RemoteMessage message)
        {
            base.OnMessageReceived(message);
        }
    }
}