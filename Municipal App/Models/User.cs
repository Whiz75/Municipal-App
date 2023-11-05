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
    public class User
    {
        [Id]
        public string Id { get; set; }
        public string FirstName { get; set; }   
        public string LastName { get; set; }   
        public string Email { get; set; }
        public string Role { get; set; }
        public string Url { get; set; }
        public IDocumentReference Document { get; set; }
    }
}