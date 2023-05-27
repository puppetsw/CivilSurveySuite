using System.Collections.ObjectModel;
using CivilSurveySuite.Shared.Models;
using CivilSurveySuite.Shared.Services.Interfaces;

namespace CivilSurveySuite.UI.ViewModels
{
    /// <summary>
    /// ViewModel for SurfaceSelectView.
    /// <br/>
    /// Implements <see cref="ObservableObject" />.
    /// </summary>
    public class SelectSurfaceViewModel : ObservableObject
    {
        private readonly ICivilSelectService _civilSelectService;
        private CivilSurface _selectedSurface;
        private ObservableCollection<CivilSurface> _surfaces;

        public ObservableCollection<CivilSurface> Surfaces
        {
            get => _surfaces;
            set => SetProperty(ref _surfaces, value);
        }

        public CivilSurface SelectedSurface
        {
            get => _selectedSurface;
            set => SetProperty(ref _selectedSurface, value);
        }

        public RelayCommand SelectSurfaceCommand => new RelayCommand(SelectSurface, () => true);

        private void SelectSurface()
        {
            var surface = _civilSelectService.SelectSurface();

            if (surface == null)
                return;

            if (Surfaces.Contains(surface))
            {
                var index = Surfaces.IndexOf(surface);
                SelectedSurface = Surfaces[index];
            }
        }

        public SelectSurfaceViewModel(ICivilSelectService civilSelectService)
        {
            _civilSelectService = civilSelectService;
            Surfaces = new ObservableCollection<CivilSurface>(_civilSelectService.GetSurfaces());
            if (Surfaces.Count > 0) //Force select of first surface
            {
                SelectedSurface = Surfaces[0];
            }
        }
    }
}