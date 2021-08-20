// Copyright Scott Whitney. All Rights Reserved.
// Reproduction or transmission in whole or in part, any form or by any
// means, electronic, mechanical or otherwise, is prohibited without the
// prior written consent of the copyright owner.

using System.Collections.ObjectModel;
using _3DS_CivilSurveySuite.Model;

namespace _3DS_CivilSurveySuite.ViewModels
{
    /// <summary>
    /// ViewModel for CogoPointViewer.
    /// </summary>
    public class CogoPointViewerViewModel : ViewModelBase
    {
        private readonly ICogoPointViewerService _cogoPointViewerService;
        private CivilPoint _selectedCivilPoint;

        public ObservableCollection<CivilPoint> CogoPoints { get; }

        public CivilPoint SelectedItem
        {
            get => _selectedCivilPoint;
            set => SetProperty(ref _selectedCivilPoint, value);
        }

        public RelayCommand ZoomToCommand => new RelayCommand(_ => ZoomToPoint(), _ => true);

        public CogoPointViewerViewModel(ICogoPointViewerService cogoPointViewerService)
        {
            _cogoPointViewerService = cogoPointViewerService;
            CogoPoints = new ObservableCollection<CivilPoint>(_cogoPointViewerService.GetPoints());
        }


        private void ZoomToPoint()
        {
            if (SelectedItem != null)
                _cogoPointViewerService.ZoomTo(SelectedItem);
        }

    }
}
