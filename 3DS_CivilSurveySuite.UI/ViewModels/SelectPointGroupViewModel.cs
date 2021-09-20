﻿// Copyright Scott Whitney. All Rights Reserved.
// Reproduction or transmission in whole or in part, any form or by any
// means, electronic, mechanical or otherwise, is prohibited without the
// prior written consent of the copyright owner.

using System.Collections.ObjectModel;
using _3DS_CivilSurveySuite.Model;
using _3DS_CivilSurveySuite.UI.Services;

namespace _3DS_CivilSurveySuite.UI.ViewModels
{
    public class SelectPointGroupViewModel : ViewModelBase
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

        public SelectPointGroupViewModel(ISelectPointGroupService pointGroupSelectService)
        {
            PointGroups = new ObservableCollection<CivilPointGroup>(pointGroupSelectService.GetPointGroups());

            if (PointGroups.Count > 0)
            {
                SelectedPointGroup = PointGroups[0];
            }
        }
    }
}