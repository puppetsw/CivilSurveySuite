// Copyright Scott Whitney. All Rights Reserved.
// Reproduction or transmission in whole or in part, any form or by any
// means, electronic, mechanical or otherwise, is prohibited without the
// prior written consent of the copyright owner.

using System.Collections.ObjectModel;
using _3DS_CivilSurveySuite.Model;

namespace _3DS_CivilSurveySuite.UI.ViewModels
{
    /// <summary>
    /// ViewModel for SurfaceSelectView.
    /// <br/>
    /// Implements <see cref="ViewModelBase" />.
    /// </summary>
    public class SurfaceSelectViewModel : ViewModelBase
    {
        private readonly ISurfaceSelectService _surfaceSelectService;
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
            set
            {
                SetProperty(ref _selectedSurface, value);
                _surfaceSelectService.Surface = _selectedSurface;
            }
        }

        public RelayCommand SelectSurfaceCommand => new RelayCommand(SelectSurface, () => true);

        private void SelectSurface()
        {
            var surface = _surfaceSelectService.SelectSurface();

            if (surface == null)
                return;
            
            if (Surfaces.Contains(surface))
            {
                var index = Surfaces.IndexOf(surface);
                SelectedSurface = Surfaces[index];
            }
        }

        public SurfaceSelectViewModel(ISurfaceSelectService surfaceSelectService)
        {
            _surfaceSelectService = surfaceSelectService;
            Surfaces = new ObservableCollection<CivilSurface>(surfaceSelectService.GetSurfaces());
            if (Surfaces.Count > 0) //Force select of first surface
            {
                SelectedSurface = Surfaces[0];
            }
        }
    }
}