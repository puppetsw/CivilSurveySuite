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

        public RelayCommand UpdateCommand => new RelayCommand(_ => Update(), _ => true);

        public RelayCommand SelectCommand => new RelayCommand(_ => Select(), _ => true);

        public CogoPointViewerViewModel(ICogoPointViewerService cogoPointViewerService)
        {
            _cogoPointViewerService = cogoPointViewerService;
            CogoPoints = new ObservableCollection<CivilPoint>(_cogoPointViewerService.GetPoints());
        }


        private void Select()
        {
            if (SelectedItem != null)
                _cogoPointViewerService.Select(SelectedItem);
        }


        private void Update()
        {
            if (SelectedItem != null)
                _cogoPointViewerService.Update(SelectedItem);
        }


        private void ZoomToPoint()
        {
            if (SelectedItem != null)
                _cogoPointViewerService.ZoomTo(SelectedItem);
        }

    }
}
