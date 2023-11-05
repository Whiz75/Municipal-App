using Android.App;
using Android.Content;
using Android.OS;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace Municipal_App.Models
{
    public class MapLaunch
    {
        public async Task NavigateToBuilding()
        {
            var location = new Location(47.645160, -122.1306032);
            var options = new MapLaunchOptions { NavigationMode = NavigationMode.Driving };

            await Map.OpenAsync(location, options);
        }
    }
}