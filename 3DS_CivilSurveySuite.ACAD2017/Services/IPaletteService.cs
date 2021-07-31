// Copyright Scott Whitney. All Rights Reserved.
// Reproduction or transmission in whole or in part, any form or by any
// means, electronic, mechanical or otherwise, is prohibited without the
// prior written consent of the copyright owner.

using System;
using System.Windows;
using _3DS_CivilSurveySuite.ViewModels;
using Autodesk.AutoCAD.Windows;

namespace _3DS_CivilSurveySuite.ACAD2017.Services
{
    public interface IPaletteService
    {
        /// <summary>
        /// Generates the palette.
        /// </summary>
        /// <param name="view">The view.</param>
        /// <param name="viewModel">The ViewModel.</param>
        /// <param name="viewName">The View associated with the ViewModel.</param>
        /// <param name="hideMethod">The action to run when the palette is hidden/closed.</param>
        void GeneratePalette(FrameworkElement view, ViewModelBase viewModel, string viewName, Action hideMethod = null);

        PaletteSet PaletteSet { get; }
    }
}