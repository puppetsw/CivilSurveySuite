// Copyright Scott Whitney. All Rights Reserved.
// Reproduction or transmission in whole or in part, any form or by any
// means, electronic, mechanical or otherwise, is prohibited without the
// prior written consent of the copyright owner.

using _3DS_CivilSurveySuite.ACAD2017.Services;
using _3DS_CivilSurveySuite.Model;
using _3DS_CivilSurveySuite.UI.Views;
using _3DS_CivilSurveySuite.ViewModels;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Runtime;
using Application = Autodesk.AutoCAD.ApplicationServices.Application;

[assembly: CommandClass(typeof(_3DS_CivilSurveySuite.ACAD2017.AcadPalettes))]
namespace _3DS_CivilSurveySuite.ACAD2017
{
    /// <summary>
    /// PaletteFactory class for hooking up Views and ViewModels to be
    /// displayed as Palettes in AutoCAD Civil3D.
    /// </summary>
    public class AcadPalettes
    {
        private bool _paletteVisible;
        private readonly IViewerService _viewerService;
        private readonly IPaletteService _paletteService;

        public AcadPalettes()
        {
            _viewerService = ServiceLocator.Container.GetInstance<IViewerService>();
            _paletteService = ServiceLocator.Container.GetInstance<IPaletteService>();
        }

        [CommandMethod("3DSShowAngleCalculator")]
        public void ShowAngleCalculatorPalette()
        {
            var view = new AngleCalculatorView();
            var vm = new AngleCalculatorViewModel();
            view.DataContext = vm;
            Application.ShowModelessWindow(view);
        }

        [CommandMethod("3DSShowTraversePalette")]
        public void ShowTraversePalette()
        {
            var view = new TraverseView();
            var vm = new TraverseViewModel(_viewerService, new TraverseService());
            _paletteService.GeneratePalette(view, vm, "Traverse");
        }

        [CommandMethod("3DSShowAngleTraversePalette")]
        public void ShowTraverseAnglePalette()
        {
            var view = new TraverseAngleView();
            var vm = new TraverseAngleViewModel(_viewerService, new TraverseService());
            _paletteService.GeneratePalette(view, vm, "Angle Traverse");
        }

        private void DocumentManager_DocumentActivated(object sender, DocumentCollectionEventArgs e)
        {
            if (_paletteService.PaletteSet == null)
            {
                return;
            }

            _paletteService.PaletteSet.Visible = e.Document != null && _paletteVisible;
        }

        private void DocumentManager_DocumentCreated(object sender, DocumentCollectionEventArgs e)
        {
            if (_paletteService.PaletteSet == null)
            {
                return;
            }

            _paletteService.PaletteSet.Visible = _paletteVisible;
        }

        private void DocumentManager_DocumentToBeDeactivated(object sender, DocumentCollectionEventArgs e)
        {
            if (_paletteService.PaletteSet == null)
            {
                return;
            }

            _paletteVisible = _paletteService.PaletteSet.Visible;
        }

        private void DocumentManager_DocumentToBeDestroyed(object sender, DocumentCollectionEventArgs e)
        {
            if (_paletteService.PaletteSet == null)
            {
                return;
            }

            _paletteVisible = _paletteService.PaletteSet.Visible;

            if (AcadApp.DocumentManager.Count == 1)
            {
                _paletteService.PaletteSet.Visible = false;
            }
        }

        public void HookupEvents()
        {
            AcadApp.DocumentManager.DocumentActivated += DocumentManager_DocumentActivated;
            AcadApp.DocumentManager.DocumentCreated += DocumentManager_DocumentCreated;
            AcadApp.DocumentManager.DocumentToBeDeactivated += DocumentManager_DocumentToBeDeactivated;
            AcadApp.DocumentManager.DocumentToBeDestroyed += DocumentManager_DocumentToBeDestroyed;
        }

        public void UnhookEvents()
        {
            AcadApp.DocumentManager.DocumentActivated -= DocumentManager_DocumentActivated;
            AcadApp.DocumentManager.DocumentCreated -= DocumentManager_DocumentCreated;
            AcadApp.DocumentManager.DocumentToBeDeactivated -= DocumentManager_DocumentToBeDeactivated;
            AcadApp.DocumentManager.DocumentToBeDestroyed -= DocumentManager_DocumentToBeDestroyed;
        }
    }
}