// Copyright Scott Whitney. All Rights Reserved.
// Reproduction or transmission in whole or in part, any form or by any
// means, electronic, mechanical or otherwise, is prohibited without the
// prior written consent of the copyright owner.
// 
// Filename: PaletteFactory.cs
// Date:     01/07/2021
// Author:   scott

using System;
using System.Collections.Generic;
using System.Windows.Controls;
using _3DS_CivilSurveySuite.Helpers.AutoCAD;
using _3DS_CivilSurveySuite.ViewModels;
using _3DS_CivilSurveySuite.Views;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.Windows;

[assembly: CommandClass(typeof(_3DS_CivilSurveySuite.Palettes.PaletteFactory))]
namespace _3DS_CivilSurveySuite.Palettes
{
    /// <summary>
    /// PaletteFactory class for hooking up Views and ViewModels to be
    /// displayed as Palettes in AutoCAD Civil3D.
    /// </summary>
    public class PaletteFactory : CivilBase, IExtensionApplication
    {
        private bool _paletteVisible;
        private static readonly List<Type> s_palettes = new List<Type>();
        private static PaletteSet s_civilSurveySuitePalSet;

        public void Initialize()
        {
            //hookup events
            AcaddocManager.DocumentActivated += AcaddocManager_DocumentActivated;
            AcaddocManager.DocumentCreated += AcaddocManager_DocumentCreated;
            AcaddocManager.DocumentToBeDeactivated += AcaddocManager_DocumentToBeDeactivated;
            AcaddocManager.DocumentToBeDestroyed += AcaddocManager_DocumentToBeDestroyed;
        }

        public void Terminate()
        {
            //unhook events
            AcaddocManager.DocumentActivated -= AcaddocManager_DocumentActivated;
            AcaddocManager.DocumentCreated -= AcaddocManager_DocumentCreated;
            AcaddocManager.DocumentToBeDeactivated -= AcaddocManager_DocumentToBeDeactivated;
            AcaddocManager.DocumentToBeDestroyed -= AcaddocManager_DocumentToBeDestroyed;
        }

        [CommandMethod("3DSShowConnectLinePalette")]
        public void ShowConnectLinePalette()
        {
            ConnectLineworkView view = new ConnectLineworkView();
            ConnectLineworkViewModel vm = new ConnectLineworkViewModel();
            GeneratePalette(view, vm, "Linework");
        }

        [CommandMethod("3DSShowDMSCalculatorPalette")]
        public void ShowDMSCalculatorPalette()
        {
            DMSCalculatorView view = new DMSCalculatorView();
            DMSCalculatorViewModel vm = new DMSCalculatorViewModel();
            GeneratePalette(view, vm, "Calculator");
        }

        [CommandMethod("3DSShowTraversePalette")]
        public void ShowTraversePalette()
        {
            TraverseView view = new TraverseView();
            TraverseViewModel vm = new TraverseViewModel();
            GeneratePalette(view, vm, "Traverse", true, vm.ClearTransientGraphics);
        }

        /// <summary>
        /// Generates the palette.
        /// </summary>
        /// <param name="view">The view.</param>
        /// <param name="viewModel">The view model.</param>
        /// <param name="viewName">Name of the view.</param>
        /// <param name="hideEvent">if set to <c>true</c> [hide event].</param>
        /// <param name="hideMethod">The hide method.</param>
        /// <exception cref="ArgumentNullException">viewName</exception>
        private void GeneratePalette(UserControl view, ViewModelBase viewModel, string viewName, bool hideEvent = false, Action hideMethod = null)
        {
            view.DataContext = viewModel;

            if (string.IsNullOrEmpty(viewName))
            {
                throw new ArgumentNullException(nameof(viewName));
            }

            if (s_civilSurveySuitePalSet == null)
            {
                CreatePaletteSet();
            }

            // ReSharper disable PossibleNullReferenceException
            if (!s_palettes.Contains(view.GetType()))
            {
                s_civilSurveySuitePalSet.AddVisual(viewName, view);
                s_palettes.Add(view.GetType());
                s_civilSurveySuitePalSet.Activate(s_palettes.IndexOf(view.GetType()));

                if (hideEvent && hideMethod != null)
                {
                    s_civilSurveySuitePalSet.StateChanged += (s, e) =>
                    {
                        if (e.NewState == StateEventIndex.Hide)
                        {
                            hideMethod.Invoke();
                        }
                    };
                }
            }

            if (!s_civilSurveySuitePalSet.Visible)
            {
                s_civilSurveySuitePalSet.Visible = true;
            }
            // ReSharper restore PossibleNullReferenceException
        }

        private void AcaddocManager_DocumentActivated(object sender, DocumentCollectionEventArgs e)
        {
            if (s_civilSurveySuitePalSet == null)
            {
                return;
            }

            s_civilSurveySuitePalSet.Visible = e.Document != null && _paletteVisible;
        }

        private void AcaddocManager_DocumentCreated(object sender, DocumentCollectionEventArgs e)
        {
            if (s_civilSurveySuitePalSet == null)
            {
                return;
            }

            s_civilSurveySuitePalSet.Visible = _paletteVisible;
        }

        private void AcaddocManager_DocumentToBeDeactivated(object sender, DocumentCollectionEventArgs e)
        {
            if (s_civilSurveySuitePalSet == null)
            {
                return;
            }

            _paletteVisible = s_civilSurveySuitePalSet.Visible;
        }

        private void AcaddocManager_DocumentToBeDestroyed(object sender, DocumentCollectionEventArgs e)
        {
            if (s_civilSurveySuitePalSet == null)
            {
                return;
            }

            _paletteVisible = s_civilSurveySuitePalSet.Visible;

            if (AcaddocManager.Count == 1)
            {
                s_civilSurveySuitePalSet.Visible = false;
            }
        }

        private static void CreatePaletteSet()
        {
            s_civilSurveySuitePalSet = new PaletteSet("3DS Civil Survey Suite", new Guid("C55243DF-EEBB-4FA6-8651-645E018F86DE"));
            s_civilSurveySuitePalSet.Style = PaletteSetStyles.ShowCloseButton |
                                             PaletteSetStyles.ShowPropertiesMenu |
                                             PaletteSetStyles.ShowAutoHideButton;
            s_civilSurveySuitePalSet.EnableTransparency(true);
            s_civilSurveySuitePalSet.KeepFocus = false;
        }
    }
}