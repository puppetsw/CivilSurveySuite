// Copyright Scott Whitney. All Rights Reserved.
// Reproduction or transmission in whole or in part, any form or by any
// means, electronic, mechanical or otherwise, is prohibited without the
// prior written consent of the copyright owner.

using System.Collections.ObjectModel;
using CivilSurveySuite.Shared.Models;
using CivilSurveySuite.Shared.Services.Interfaces;

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