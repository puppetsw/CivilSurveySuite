﻿// Copyright Scott Whitney. All Rights Reserved.
// Reproduction or transmission in whole or in part, any form or by any
// means, electronic, mechanical or otherwise, is prohibited without the
// prior written consent of the copyright owner.

using System.Collections.ObjectModel;
using _3DS_CivilSurveySuite.Model;

namespace _3DS_CivilSurveySuite.UI.ViewModels
{
    public class PointGroupSelectViewModel : ViewModelBase
    {
        private readonly IPointGroupSelectService _pointGroupSelectService;
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
            set
            {
                SetProperty(ref _selectedPointGroup, value);
                _pointGroupSelectService.PointGroup = value;
            }
        }


        public PointGroupSelectViewModel(IPointGroupSelectService pointGroupSelectService)
        {
            _pointGroupSelectService = pointGroupSelectService;
            PointGroups = new ObservableCollection<CivilPointGroup>(_pointGroupSelectService.GetPointGroups());

            if (PointGroups.Count > 0)
            {
                SelectedPointGroup = PointGroups[0];
            }
        }





    }
}