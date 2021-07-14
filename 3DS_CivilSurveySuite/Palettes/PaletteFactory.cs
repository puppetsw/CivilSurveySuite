// Copyright Scott Whitney. All Rights Reserved.
// Reproduction or transmission in whole or in part, any form or by any
// means, electronic, mechanical or otherwise, is prohibited without the
// prior written consent of the copyright owner.

using System;
using System.Collections.Generic;
using System.Windows;
using _3DS_CivilSurveySuite.Model;
using _3DS_CivilSurveySuite.Services;
using _3DS_CivilSurveySuite.UI.UserControls;
using _3DS_CivilSurveySuite.UI.Views;
using _3DS_CivilSurveySuite.ViewModels;
using _3DS_CivilSurveySuite_ACADBase21;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.Windows;
using Application = Autodesk.AutoCAD.ApplicationServices.Application;

[assembly: CommandClass(typeof(_3DS_CivilSurveySuite.Palettes.PaletteFactory))]
namespace _3DS_CivilSurveySuite.Palettes
{
    /// <summary>
    /// PaletteFactory class for hooking up Views and ViewModels to be
    /// displayed as Palettes in AutoCAD Civil3D.
    /// </summary>
    public class PaletteFactory : IDisposable
    {
        private bool _paletteVisible;
        private readonly List<Type> _palettes = new List<Type>();
        private PaletteSet _civilSurveySuitePalSet;
        private static IViewerService s_viewerService;

        public PaletteFactory()
        {
            s_viewerService = new ViewerService();
        }
        ~PaletteFactory()
        {
            ReleaseUnmanagedResources();
        }

        [CommandMethod("3DSShowConnectLinePalette")]
        public void ShowConnectLinePalette()
        {
            var view = new ConnectLineworkView();
            var vm = new ConnectLineworkViewModel(Properties.Settings.Default.ConnectLineworkFileName);
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
            var vm = new TraverseViewModel(s_viewerService);
            GeneratePalette(view, vm, "Traverse");
        }

        [CommandMethod("3DSShowAngleTraversePalette")]
        public void ShowTraverseAnglePalette()
        {
            var view = new TraverseAngleView();
            var vm = new TraverseAngleViewModel(s_viewerService);
            GeneratePalette(view, vm, "Angle Traverse");
        }

        /// <summary>
        /// Generates the palette.
        /// </summary>
        /// <param name="view">The view.</param>
        /// <param name="viewModel">The ViewModel.</param>
        /// <param name="viewName">The View associated with the ViewModel.</param>
        /// <param name="hideMethod">The action to run when the palette is hidden/closed.</param>
        private void GeneratePalette(FrameworkElement view, ViewModelBase viewModel, string viewName, Action hideMethod = null)
        {
            view.DataContext = viewModel;

            if (_civilSurveySuitePalSet == null)
            {
                CreatePaletteSet();
            }

            // ReSharper disable PossibleNullReferenceException
            if (!_palettes.Contains(view.GetType()))
            {
                _civilSurveySuitePalSet.AddVisual(viewName, view);
                _palettes.Add(view.GetType());
                _civilSurveySuitePalSet.Activate(_palettes.IndexOf(view.GetType()));

                if (hideMethod != null)
                {
                    _civilSurveySuitePalSet.StateChanged += (s, e) =>
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

            if (!_civilSurveySuitePalSet.Visible)
            {
                _civilSurveySuitePalSet.Visible = true;
            }
            // ReSharper restore PossibleNullReferenceException
        }

        private void CreatePaletteSet()
        {
            _civilSurveySuitePalSet = new PaletteSet("3DS Civil Survey Suite", new Guid("C55243DF-EEBB-4FA6-8651-645E018F86DE"));
            _civilSurveySuitePalSet.Style = PaletteSetStyles.ShowCloseButton |
                                             PaletteSetStyles.ShowPropertiesMenu |
                                             PaletteSetStyles.ShowAutoHideButton;
            _civilSurveySuitePalSet.EnableTransparency(true);
            _civilSurveySuitePalSet.KeepFocus = false;
        }

        private void DocumentManager_DocumentActivated(object sender, DocumentCollectionEventArgs e)
        {
            if (_civilSurveySuitePalSet == null)
            {
                return;
            }

            _civilSurveySuitePalSet.Visible = e.Document != null && _paletteVisible;
        }

        private void DocumentManager_DocumentCreated(object sender, DocumentCollectionEventArgs e)
        {
            if (_civilSurveySuitePalSet == null)
            {
                return;
            }

            _civilSurveySuitePalSet.Visible = _paletteVisible;
        }

        private void DocumentManager_DocumentToBeDeactivated(object sender, DocumentCollectionEventArgs e)
        {
            if (_civilSurveySuitePalSet == null)
            {
                return;
            }

            _paletteVisible = _civilSurveySuitePalSet.Visible;
        }

        private void DocumentManager_DocumentToBeDestroyed(object sender, DocumentCollectionEventArgs e)
        {
            if (_civilSurveySuitePalSet == null)
            {
                return;
            }

            _paletteVisible = _civilSurveySuitePalSet.Visible;

            if (AutoCADApplicationManager.DocumentManager.Count == 1)
            {
                _civilSurveySuitePalSet.Visible = false;
            }
        }

        public void HookupEvents()
        {
            AutoCADApplicationManager.DocumentManager.DocumentActivated += DocumentManager_DocumentActivated;
            AutoCADApplicationManager.DocumentManager.DocumentCreated += DocumentManager_DocumentCreated;
            AutoCADApplicationManager.DocumentManager.DocumentToBeDeactivated += DocumentManager_DocumentToBeDeactivated;
            AutoCADApplicationManager.DocumentManager.DocumentToBeDestroyed += DocumentManager_DocumentToBeDestroyed;
        }

        public void UnhookEvents()
        {
            AutoCADApplicationManager.DocumentManager.DocumentActivated -= DocumentManager_DocumentActivated;
            AutoCADApplicationManager.DocumentManager.DocumentCreated -= DocumentManager_DocumentCreated;
            AutoCADApplicationManager.DocumentManager.DocumentToBeDeactivated -= DocumentManager_DocumentToBeDeactivated;
            AutoCADApplicationManager.DocumentManager.DocumentToBeDestroyed -= DocumentManager_DocumentToBeDestroyed;
        }

        private void ReleaseUnmanagedResources()
        {
            if (_civilSurveySuitePalSet != null)
                _civilSurveySuitePalSet.Dispose();
        }

        /// <inheritdoc />
        public void Dispose()
        {
            ReleaseUnmanagedResources();
            GC.SuppressFinalize(this);
        }
    }
}