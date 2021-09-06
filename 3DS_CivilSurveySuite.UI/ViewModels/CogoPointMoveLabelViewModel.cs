using _3DS_CivilSurveySuite.Model;

namespace _3DS_CivilSurveySuite.UI.ViewModels
{
    public class CogoPointMoveLabelViewModel : ViewModelBase
    {
        private readonly ICogoPointMoveLabelService _cogoPointMoveLabelService;

        public RelayCommand MoveCommand => new RelayCommand(Move, () => true);

        private void Move()
        {
            _cogoPointMoveLabelService.MoveDifference = new Vector(1, 1);
        }

        public CogoPointMoveLabelViewModel(ICogoPointMoveLabelService cogoPointMoveLabelService)
        {
            _cogoPointMoveLabelService = cogoPointMoveLabelService;
        }
    }
}
