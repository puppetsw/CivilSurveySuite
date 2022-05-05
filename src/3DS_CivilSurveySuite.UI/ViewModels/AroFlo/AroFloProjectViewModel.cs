using System.Threading.Tasks;
using System.Windows.Input;
using AroFloApi;
using GMapControl;

namespace _3DS_CivilSurveySuite.UI.ViewModels.AroFlo
{
    public class AroFloProjectViewModel : ObservableObject
    {
        private string _description;
        private string _client;
        private string _location;
        private LatLong _position;
        private int _projectNumber;

        public string Description
        {
            get => _description;
            set => SetProperty(ref _description, value);
        }

        public string Client
        {
            get => _client;
            set => SetProperty(ref _client, value);
        }

        public string Location
        {
            get => _location;
            set => SetProperty(ref _location, value);
        }

        public LatLong Position
        {
            get => _position;
            set => SetProperty(ref _position, value);
        }

        public int ProjectNumber
        {
            get => _projectNumber;
            set => SetProperty(ref _projectNumber, value);
        }

        public ICommand FindJobCommand { get; private set; }

        public AroFloProjectViewModel()
        {
            InitCommands();
        }

        private void InitCommands()
        {
            FindJobCommand = new AsyncRelayCommand(GetJob);
        }

        private async Task GetJob()
        {
            var projectService = new ProjectService();
            var locationService = new LocationService();
            var project = await projectService.GetProjectAsync(ProjectNumber);

            if (project == null)
            {
                return;
            }

            var location = await locationService.GetLocationAsync(project.Location.LocationId);
            Position = new LatLong { Latitude = location.Latitude, Longitude = location.Longitude };
            Location = project.Location.LocationName;
            Client = project.Client.Name;
            Description = project.Description;
        }
    }
}
