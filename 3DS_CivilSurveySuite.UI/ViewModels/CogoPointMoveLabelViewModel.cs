// Copyright Scott Whitney. All Rights Reserved.
// Reproduction or transmission in whole or in part, any form or by any
// means, electronic, mechanical or otherwise, is prohibited without the
// prior written consent of the copyright owner.

using _3DS_CivilSurveySuite.UI.Services;

namespace _3DS_CivilSurveySuite.UI.ViewModels
{
    /// <summary>
    /// ViewModel for CogoPointMoveLabelView
    /// </summary>
    public class CogoPointMoveLabelViewModel : ViewModelBase
    {
        private readonly ICogoPointMoveLabelService _cogoPointMoveLabelService;
        private double _deltaY;
        private double _deltaX;

        public double DeltaX
        {
            get => _deltaX;
            set => SetProperty(ref _deltaX, value);
        }

        public double DeltaY
        {
            get => _deltaY;
            set => SetProperty(ref _deltaY, value);
        }


        public RelayCommand MoveCommand => new RelayCommand(Move, () => true);

        private void Move()
        {
            _cogoPointMoveLabelService.MoveLabels(DeltaX, DeltaY);
        }

        public CogoPointMoveLabelViewModel(ICogoPointMoveLabelService cogoPointMoveLabelService)
        {
            _cogoPointMoveLabelService = cogoPointMoveLabelService;
        }
    }
}
