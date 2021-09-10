// Copyright Scott Whitney. All Rights Reserved.
// Reproduction or transmission in whole or in part, any form or by any
// means, electronic, mechanical or otherwise, is prohibited without the
// prior written consent of the copyright owner.

using _3DS_CivilSurveySuite.ACAD2017.Services;
using _3DS_CivilSurveySuite.UI.Services;
using _3DS_CivilSurveySuite.UI.ViewModels;
using _3DS_CivilSurveySuite.UI.Views;
using Autodesk.AutoCAD.ApplicationServices;
using Application = Autodesk.AutoCAD.ApplicationServices.Core.Application;

namespace _3DS_CivilSurveySuite.ACAD2017
{
    /// <summary>
    /// PaletteFactory class for hooking up Views and ViewModels to be
    /// displayed as Palettes in AutoCAD Civil3D.
    /// </summary>
    public static class AcadPalettes
    {
        private static bool s_paletteVisible;
        private static readonly IViewerService s_viewerService;
        private static readonly PaletteService s_paletteService;

        static AcadPalettes()
        {
            s_viewerService = AcadService.CreateViewerService();
            s_paletteService = AcadService.CreatePaletteService();
        }

        public static void ShowAngleCalculatorPalette()
        {
            var view = new AngleCalculatorView { DataContext = new AngleCalculatorViewModel() };
            Application.ShowModelessWindow(view);
        }

        public static void ShowTraversePalette()
        {
            var view = new TraverseView();
            var vm = new TraverseViewModel(s_viewerService, AcadService.CreateTraverseService());
            s_paletteService.GeneratePalette(view, vm, "Traverse");
        }

        public static void ShowTraverseAnglePalette()
        {
            var view = new TraverseAngleView();
            var vm = new TraverseAngleViewModel(s_viewerService, AcadService.CreateTraverseService());
            s_paletteService.GeneratePalette(view, vm, "Angle Traverse");
        }

        private static void DocumentManager_DocumentActivated(object sender, DocumentCollectionEventArgs e)
        {
            if (s_paletteService.PaletteSet == null)
            {
                return;
            }

            s_paletteService.PaletteSet.Visible = e.Document != null && s_paletteVisible;
        }

        private static void DocumentManager_DocumentCreated(object sender, DocumentCollectionEventArgs e)
        {
            if (s_paletteService.PaletteSet == null)
            {
                return;
            }

            s_paletteService.PaletteSet.Visible = s_paletteVisible;
        }

        private static void DocumentManager_DocumentToBeDeactivated(object sender, DocumentCollectionEventArgs e)
        {
            if (s_paletteService.PaletteSet == null)
            {
                return;
            }

            s_paletteVisible = s_paletteService.PaletteSet.Visible;
        }

        private static void DocumentManager_DocumentToBeDestroyed(object sender, DocumentCollectionEventArgs e)
        {
            if (s_paletteService.PaletteSet == null)
            {
                return;
            }

            s_paletteVisible = s_paletteService.PaletteSet.Visible;

            if (AcadApp.DocumentManager.Count == 1)
            {
                s_paletteService.PaletteSet.Visible = false;
            }
        }

        public static void HookupEvents()
        {
            AcadApp.DocumentManager.DocumentActivated += DocumentManager_DocumentActivated;
            AcadApp.DocumentManager.DocumentCreated += DocumentManager_DocumentCreated;
            AcadApp.DocumentManager.DocumentToBeDeactivated += DocumentManager_DocumentToBeDeactivated;
            AcadApp.DocumentManager.DocumentToBeDestroyed += DocumentManager_DocumentToBeDestroyed;
        }

        public static void UnhookEvents()
        {
            AcadApp.DocumentManager.DocumentActivated -= DocumentManager_DocumentActivated;
            AcadApp.DocumentManager.DocumentCreated -= DocumentManager_DocumentCreated;
            AcadApp.DocumentManager.DocumentToBeDeactivated -= DocumentManager_DocumentToBeDeactivated;
            AcadApp.DocumentManager.DocumentToBeDestroyed -= DocumentManager_DocumentToBeDestroyed;
        }
    }
}