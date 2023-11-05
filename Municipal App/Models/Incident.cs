using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Plugin.CloudFirestore;
using Plugin.CloudFirestore.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Municipal_App.Models
{
    public class Incident
    {
        [Id]
        public string Id { get; set; }
        public string Description { get; set; }
        public string IncidentTypeId { get; set; }
        public DateTime DateReported { get; set; }
        public string Status { get; set; }
        public string Severity { get; set; }
        public GeoPoint Coordinates { get; set; }
        public string UserId { get; set; }
        public string ContentUrl { get; set;}
    }
}