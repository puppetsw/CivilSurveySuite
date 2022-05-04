using System.Threading.Tasks;
using System.Windows;
using AroFloApi;
using GMap.NET.MapProviders;

namespace GMap.NET
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            GoogleMapProvider.Instance.ApiKey = "AIzaSyBmSh5fIQb4pD8ywwCNdmzIIkX0na7V0mQ";

            MainMap.MapProvider = GMapProviders.OpenStreetMap;

            UpdateMapPosition();
        }

        private async Task UpdateMapPosition()
        {
            ILocationService locationService = new LocationService();
            var location = await locationService.GetLocationAsync("JSYqKyBSXFggCg==", default);

            MainMap.Position = new PointLatLng(location.Latitude, location.Longitude);
            MainMap.Zoom = 100;
        }
    }
}
