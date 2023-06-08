using System.Collections.ObjectModel;
using CivilSurveySuite.Common.Models;
using CivilSurveySuite.Common.Services.Interfaces;

namespace CivilSurveySuite.UI.ViewModels
{
    public class SelectPointGroupViewModel : ObservableObject
    {
        private ObservableCollection<CivilPointGroup> _pointGroups;
        private CivilPointGroup _selectedPointGroup;

        public ObservableCollection<CivilPointGroup> PointGroups
        {
            get => _pointGroups;
            set => SetProperty(ref _pointGroups, value);
        }

        public CivilPointGroup SelectedPointGroup
        {
            get => _selectedPointGroup;
            set => SetProperty(ref _selectedPointGroup, value);
        }

        public SelectPointGroupViewModel(ICivilSelectService civilSelectService)
        {
            PointGroups = new ObservableCollection<CivilPointGroup>(civilSelectService.GetPointGroups());

            if (PointGroups.Count > 0)
            {
                SelectedPointGroup = PointGroups[0];
            }
        }
    }
}