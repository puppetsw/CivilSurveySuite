using System.Collections.ObjectModel;
using CivilSurveySuite.Common.Models;
using CivilSurveySuite.Common.Services.Interfaces;

namespace CivilSurveySuite.UI.ViewModels
{
    public class SelectAlignmentViewModel : ObservableObject
    {
        private readonly ICivilSelectService _civilSelectService;
        private ObservableCollection<CivilAlignment> _alignments;
        private CivilAlignment _selectedAlignment;

        public ObservableCollection<CivilAlignment> Alignments
        {
            get => _alignments;
            set => SetProperty(ref _alignments, value);
        }

        public CivilAlignment SelectedAlignment
        {
            get => _selectedAlignment;
            set => SetProperty(ref _selectedAlignment, value);
        }

        public RelayCommand SelectAlignmentCommand => new RelayCommand(SelectAlignment, () => true);

        public SelectAlignmentViewModel(ICivilSelectService civilSelectService)
        {
            _civilSelectService = civilSelectService;

            Alignments = new ObservableCollection<CivilAlignment>(_civilSelectService.GetAlignments());

            if (Alignments.Count > 0)
            {
                SelectedAlignment = Alignments[0];
            }
        }

        private void SelectAlignment()
        {
            var alignment = _civilSelectService?.SelectAlignment();

            if (alignment == null)
                return;

            if (Alignments.Contains(alignment))
            {
                var index = Alignments.IndexOf(alignment);
                SelectedAlignment = Alignments[index];
            }
        }


    }
}