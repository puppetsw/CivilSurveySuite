using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using GMap.NET.MapProviders;
using GMap.NET.WindowsPresentation;

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

            MainMap.MapProvider = GMapProviders.GoogleMap;

            UpdateMapPosition();
        }

        private async Task UpdateMapPosition()
        {
            // CancellationTokenSource cts = new CancellationTokenSource();
            // cts.CancelAfter(TimeSpan.FromSeconds(15));
            // IProjectService projectService = new ProjectService();
            // var project = await projectService.GetProjectAsync("834", cts.Token);
            //
            // ILocationService locationService = new LocationService();
            // var location = await locationService.GetLocationAsync(project.Location.LocationId, cts.Token);
            //
            // MainMap.Position = new PointLatLng(location.Latitude, location.Longitude);
            //
            // var currentMarker = new GMapMarker(MainMap.Position);
            // {
            //     currentMarker.Shape = new CustomMarkerRed(this, currentMarker, $"{location.LocationName}");
            //     currentMarker.Offset = new Point(-15, -15);
            //     currentMarker.ZIndex = int.MaxValue;
            //     MainMap.Markers.Add(currentMarker);
            // }
            //
            //
            // MainMap.Zoom = 16;
        }
    }
}
