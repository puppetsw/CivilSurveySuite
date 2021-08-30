// Copyright Scott Whitney. All Rights Reserved.
// Reproduction or transmission in whole or in part, any form or by any
// means, electronic, mechanical or otherwise, is prohibited without the
// prior written consent of the copyright owner.

using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace _3DS_CivilSurveySuite.ViewModels
{
    /// <summary>
    /// ViewModel for SurfaceSelectView.
    /// <br/>
    /// Implements <see cref="ViewModelBase" />.
    /// </summary>
    public class SurfaceSelectViewModel : ViewModelBase
    {
        private ObservableCollection<string> _surfaceNames;
        private string _selectedSurfaceName;

        public ObservableCollection<string> SurfaceNames
        {
            get => _surfaceNames;
            set => SetProperty(ref _surfaceNames, value);
        }

        public string SelectedSurfaceName
        {
            get => _selectedSurfaceName;
            set => SetProperty(ref _selectedSurfaceName, value);
        }

        public SurfaceSelectViewModel(IEnumerable<string> surfaceNames)
        {
            SurfaceNames = new ObservableCollection<string>(surfaceNames);
        }
    }
}