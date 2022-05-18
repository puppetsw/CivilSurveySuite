using System;
using System.Threading.Tasks;
using System.Windows.Input;
using _3DS_CivilSurveySuite.UI.Services.Interfaces;
using AroFloApi;
using MapControl;

namespace _3DS_CivilSurveySuite.UI.ViewModels.AroFlo
{
    public class AroFloProjectViewModel : ObservableObject
    {
        private readonly IMessageBoxService _messageBoxService;
        private string _description;
        private string _client;
        private string _location;
        private Position _position;
        private int _projectNumber;
        private bool _isBusy;

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

        public Position Position
        {
            get => _position;
            set => SetProperty(ref _position, value);
        }

        public int ProjectNumber
        {
            get => _projectNumber;
            set => SetProperty(ref _projectNumber, value);
        }

        public bool IsBusy
        {
            get => _isBusy;
            set => SetProperty(ref _isBusy, value);
        }

        public ICommand FindJobCommand { get; private set; }

        public AroFloProjectViewModel(IMessageBoxService messageBoxService)
        {
            _messageBoxService = messageBoxService;
            InitCommands();
            AroFloSetup();
        }

        private void InitCommands()
        {
            FindJobCommand = new AsyncRelayCommand(GetJob);
        }

        private static void AroFloSetup()
        {
            AroFloConfiguration.SECRET_KEY = "RHIzTUFiUlJhSUpPenNQaFA2WHBzcGMzYXJlM1RxMCtDVW5uNkRKdnhITzI1S0krNW4vM0NZdk45SnlnNFFTaG1wcnB0WXBlRGMzNlFYeDEwVE9Wbmc9PQ==";
            AroFloConfiguration.U_ENCODE = "PjZPQjtBSEM7RihdOjI6JDJMKlwgJiohQ0AxTVw4Klg9Jzk6NDUpWiwK";
            AroFloConfiguration.P_ENCODE = "cTdod3FkODFlNnI0TGVk";
            AroFloConfiguration.ORG_ENCODE = "JSc6TyBQLFAgCg==";
        }

        private async Task GetJob()
        {
            try
            {
                IsBusy = true;
                var projectService = new ProjectController();
                var locationService = new LocationService();
                var project = await projectService.GetProjectAsync(ProjectNumber);

                if (project == null)
                {
                    _messageBoxService.ShowAlert($"Unable to find job. {ProjectNumber}");
                    return;
                }

                var location = await locationService.GetLocationAsync(project.Location.LocationId);
                Position = new Position { Latitude = location.Latitude, Longitude = location.Longitude };
                Location = project.Location.LocationName;
                Client = project.Client.Name;
                Description = project.Description;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            finally
            {
                IsBusy = false;
            }

        }
    }
}
