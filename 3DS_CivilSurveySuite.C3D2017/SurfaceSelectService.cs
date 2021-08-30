// Copyright Scott Whitney. All Rights Reserved.
// Reproduction or transmission in whole or in part, any form or by any
// means, electronic, mechanical or otherwise, is prohibited without the
// prior written consent of the copyright owner.

using System;
using _3DS_CivilSurveySuite.Model;
using _3DS_CivilSurveySuite.UI.Views;
using _3DS_CivilSurveySuite.ViewModels;

namespace _3DS_CivilSurveySuite.C3D2017
{
    public class SurfaceSelectService : ISurfaceSelectService
    {
        public string SurfaceName { get; private set; }

        public bool ShowDialog()
        {
            var vm = new SurfaceSelectViewModel(SurfaceUtils.GetSurfaceNames());
            var view = new SurfaceSelectView
            {
                DataContext = vm
            };

            bool? dialogResult = view.ShowDialog();

            if (dialogResult != null && dialogResult.Value)
            {
                SurfaceName = vm.SelectedSurfaceName;
                return true;
            }

            return false;
        }
    }
}