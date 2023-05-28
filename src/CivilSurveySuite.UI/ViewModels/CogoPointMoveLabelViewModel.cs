using CivilSurveySuite.Common.Models;
using CivilSurveySuite.Common.Services.Interfaces;

namespace CivilSurveySuite.UI.ViewModels
{
    /// <summary>
    /// ViewModel for CogoPointMoveLabelView
    /// </summary>
    public class CogoPointMoveLabelViewModel : ObservableObject
    {
        private readonly ICogoPointService _cogoPointService;
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
            _cogoPointService.MoveLabels(DeltaX, DeltaY);
        }

        public CogoPointMoveLabelViewModel(ICogoPointService cogoPointService)
        {
            _cogoPointService = cogoPointService;
        }
    }
}
