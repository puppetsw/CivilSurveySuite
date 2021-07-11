// Copyright Scott Whitney. All Rights Reserved.
// Reproduction or transmission in whole or in part, any form or by any
// means, electronic, mechanical or otherwise, is prohibited without the
// prior written consent of the copyright owner.

using System;
using System.Collections.Generic;
using System.Windows;
using _3DS_CivilSurveySuite.ViewModels;
using _3DS_CivilSurveySuite.Views;
using _3DS_CivilSurveySuite_ACADBase21;
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
    public class PaletteFactory : IExtensionApplication
    {
        private bool _paletteVisible;
        private static readonly List<Type> s_palettes = new List<Type>();
        private static PaletteSet s_civilSurveySuitePalSet;

        public void Initialize()
        {
            //hookup events
            AutoCADApplicationManager.DocumentManager.DocumentActivated += DocumentManager_DocumentActivated;
            AutoCADApplicationManager.DocumentManager.DocumentCreated += DocumentManager_DocumentCreated;
            AutoCADApplicationManager.DocumentManager.DocumentToBeDeactivated += DocumentManager_DocumentToBeDeactivated;
            AutoCADApplicationManager.DocumentManager.DocumentToBeDestroyed += DocumentManager_DocumentToBeDestroyed;
        }

        public void Terminate()
        {
            AutoCADApplicationManager.DocumentManager.DocumentActivated -= DocumentManager_DocumentActivated;
            AutoCADApplicationManager.DocumentManager.DocumentCreated -= DocumentManager_DocumentCreated;
            AutoCADApplicationManager.DocumentManager.DocumentToBeDeactivated -= DocumentManager_DocumentToBeDeactivated;
            AutoCADApplicationManager.DocumentManager.DocumentToBeDestroyed -= DocumentManager_DocumentToBeDestroyed;
        }

        [CommandMethod("3DSTEST")]
        public void RunViewer()
        {
        }


        [CommandMethod("3DSShowConnectLinePalette")]
        public void ShowConnectLinePalette()
        {
            var view = new ConnectLineworkView();
            var vm = new ConnectLineworkViewModel();
            GeneratePalette(view, vm, "Linework");
        }

        [CommandMethod("3DSShowAngleCalculatorPalette")]
        public void ShowAngleCalculatorPalette()
        {
            var view = new AngleCalculatorView();
            var vm = new AngleCalculatorViewModel();
            GeneratePalette(view, vm, "Angle Calculator");
        }

        [CommandMethod("3DSShowTraversePalette")]
        public void ShowTraversePalette()
        {
            var view = new TraverseView();
            var vm = new TraverseViewModel();
            GeneratePalette(view, vm, "Traverse", TransientGraphics.ClearTransientGraphics);
        }

        [CommandMethod("3DSShowAngleTraversePalette")]
        public void ShowTraverseAnglePalette()
        {
            var view = new TraverseAngleView();
            var vm = new TraverseAngleViewModel();
            GeneratePalette(view, vm, "Angle Traverse", TransientGraphics.ClearTransientGraphics);
        }

        /// <summary>
        /// Generates the palette.
        /// </summary>
        /// <param name="view">The view.</param>
        /// <param name="viewModel">The ViewModel.</param>
        /// <param name="viewName">The View associated with the ViewModel.</param>
        /// <param name="hideMethod">The action to run when the palette is hidden/closed.</param>
        private static void GeneratePalette(FrameworkElement view, ViewModelBase viewModel, string viewName, Action hideMethod = null)
        {
            view.DataContext = viewModel;

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

                if (hideMethod != null)
                {
                    s_civilSurveySuitePalSet.StateChanged += (s, e) =>
                    {
                        if (e.NewState == StateEventIndex.Hide)
                        {
                            hideMethod.Invoke();
                        }
                    };

                    //BUG: When closing AutoCAD this event tries to run. But Editor is null.
                    //view.IsVisibleChanged += (s, e) => AutoCADApplicationManager.Editor.WriteMessage("IsVisibleChanged");
                }
            }

            if (!s_civilSurveySuitePalSet.Visible)
            {
                s_civilSurveySuitePalSet.Visible = true;
            }
            // ReSharper restore PossibleNullReferenceException
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

        private void DocumentManager_DocumentActivated(object sender, DocumentCollectionEventArgs e)
        {
            if (s_civilSurveySuitePalSet == null)
            {
                return;
            }

            s_civilSurveySuitePalSet.Visible = e.Document != null && _paletteVisible;
        }

        private void DocumentManager_DocumentCreated(object sender, DocumentCollectionEventArgs e)
        {
            if (s_civilSurveySuitePalSet == null)
            {
                return;
            }

            s_civilSurveySuitePalSet.Visible = _paletteVisible;
        }

        private void DocumentManager_DocumentToBeDeactivated(object sender, DocumentCollectionEventArgs e)
        {
            if (s_civilSurveySuitePalSet == null)
            {
                return;
            }

            _paletteVisible = s_civilSurveySuitePalSet.Visible;
        }

        private void DocumentManager_DocumentToBeDestroyed(object sender, DocumentCollectionEventArgs e)
        {
            if (s_civilSurveySuitePalSet == null)
            {
                return;
            }

            _paletteVisible = s_civilSurveySuitePalSet.Visible;

            if (AutoCADApplicationManager.DocumentManager.Count == 1)
            {
                s_civilSurveySuitePalSet.Visible = false;
            }
        }

    }
}