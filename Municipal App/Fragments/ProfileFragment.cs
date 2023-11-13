using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Views;
using AndroidHUD;
using Google.Android.Material.Button;
using Google.Android.Material.TextField;
using Google.Android.Material.TextView;
using Municipal_App.Models;
using Plugin.CloudFirestore;
using Plugin.FirebaseAuth;
using Plugin.FirebaseStorage;
using Refractored.Controls;
using Square.Picasso;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Xamarin.Essentials;
using XamarinTextDrawable;
using Fragment = AndroidX.Fragment.App.Fragment;

namespace Municipal_App.Fragments
{
    public class ProfileFragment : Fragment
    {
        private Context mContext;
        private TextInputEditText Firstname;
        private TextInputEditText Lastname;
        private TextInputEditText Email;
        private TextInputEditText ET_Role;
        private MaterialTextView username;
        private MaterialTextView date;
        private MaterialTextView role;
        private CircleImageView user_picture;

        private MaterialButton BtnProfileUpdate;
        private FileResult file;
        //private string str_lname, str_fname, str_email, str_role;

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Create your fragment here
        }

        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState)
        {
            // Use this to return your custom view for this Fragment
            // return inflater.Inflate(Resource.Layout.YourFragment, container, false);

            View view = inflater.Inflate(Resource.Layout.profile_fragment, container, false);
            Init(view);
            GetUserInfo();
            UpdateUserInfo();
            UpdateProfileImage();

            return view;
        }

        private void Init(View view)
        {
            mContext = view.Context;

            username = view.FindViewById<MaterialTextView>(Resource.Id.username);
            Firstname = view.FindViewById<TextInputEditText>(Resource.Id.InputProfileFirstName);
            Lastname = view.FindViewById<TextInputEditText>(Resource.Id.InputProfileLastName);
            Email = view.FindViewById<TextInputEditText>(Resource.Id.InputProfileEmail);
            date = view.FindViewById<MaterialTextView>(Resource.Id.date);
            role = view.FindViewById<MaterialTextView>(Resource.Id.TextViewProfileRole);
            ET_Role = view.FindViewById<TextInputEditText>(Resource.Id.InputProfileRole);

            user_picture = view.FindViewById<CircleImageView>(Resource.Id.user_picture);

            BtnProfileUpdate = view.FindViewById<MaterialButton>(Resource.Id.BtnProfileUpdate);

            //BtnProfileUpdate.Click += delegate
            //{
            //    UpdateUserInfo();
            //};
        }

        private void GetUserInfo()
        {
            try
            {
                CrossCloudFirestore
                .Current
                .Instance
                .Collection("USERS")
                .Document(CrossFirebaseAuth.Current.Instance.CurrentUser.Uid)
                .AddSnapshotListener((v, e) =>
                {
                    if (v.Exists)
                    {
                        var user = v.ToObject<User>();

                        username.Text = $"{user.FirstName} {user.LastName}";
                        date.Text = CrossFirebaseAuth.Current.Instance.CurrentUser.Metadata.CreationDate.ToString();
                        Firstname.Text = user.FirstName;
                        Lastname.Text = user.LastName;
                        Email.Text = user.Email;
                        role.Text = $"ROLE: {user.Role}";
                        ET_Role.Text = user.Role;

                        if (user.Url != null)
                        {
                            Picasso
                            .Get()
                            .Load(user.Url)
                            .Fit()
                            .CenterCrop()
                            .Into(user_picture);
                        }
                        else
                        {
                            TextDrawable drawable1 = new TextDrawable.Builder()
                            .BuildRound($"{user.FirstName.Substring(0, 1)}{user.LastName.Substring(0, 1)}", Color.DeepSkyBlue);
                            user_picture.SetImageDrawable(drawable1);
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                AndHUD.Shared.ShowError(mContext, ex.Message, MaskType.None, TimeSpan.FromSeconds(3));      
            }
        }

        private async void UpdateUserInfo()
        {
            try
            {
                Dictionary<string, object> user = new Dictionary<string, object>
                {
                    { "FirstName", Firstname.Text },
                    { "LastName", Lastname.Text },
                    // Add more fields as needed
                };

                await CrossCloudFirestore
                .Current
                .Instance
                .Collection("USERS")
                .Document(CrossFirebaseAuth.Current.Instance.CurrentUser.Uid)
                .UpdateAsync(user);
                    
            }
            catch (Exception ex)
            {
                AndHUD.Shared.ShowError(mContext, ex.Message, MaskType.None, TimeSpan.FromSeconds(3));
            }
        }

        private void UpdateProfileImage()
        {
            user_picture.Click += async (s, e) =>
            {
                file = await PickAndShow();

                try
                {
                    var memoryStream = new MemoryStream();
                    var st = await file.OpenReadAsync();
                    string filename = file.FileName;

                    var results = CrossFirebaseStorage.Current
                        .Instance
                        .RootReference
                        .Child("PROFILE PICTURES")
                        .Child(filename);

                    await results.PutStreamAsync(st);

                    var url = await results.GetDownloadUrlAsync();

                    User user = new User()
                    {
                        Url = url.ToString()
                    };

                    await CrossCloudFirestore
                    .Current
                    .Instance
                    .Collection("USERS")
                    .Document(CrossFirebaseAuth.Current.Instance.CurrentUser.Uid)
                    .UpdateAsync(user);
                }
                catch (Exception ex)
                {
                    AndHUD.Shared.ShowError(mContext, ex.Message, MaskType.None, TimeSpan.FromSeconds(3));
                }
            };
        }

        private async Task<FileResult> PickAndShow()
        {
            try
            {
                var file = await FilePicker.PickAsync(new PickOptions()
                {
                    FileTypes = FilePickerFileType.Images
                });

                if(file == null)
                {
                    return file;
                }
            }catch(Exception ex)
            {
                AndHUD.Shared.ShowError(mContext, ex.Message, MaskType.None, TimeSpan.FromSeconds(3));
            }
            return null;
        }
    }
}