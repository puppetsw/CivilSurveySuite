// Copyright Scott Whitney. All Rights Reserved.
// Reproduction or transmission in whole or in part, any form or by any
// means, electronic, mechanical or otherwise, is prohibited without the
// prior written consent of the copyright owner.

using _3DS_CivilSurveySuite.ACAD2017;
using _3DS_CivilSurveySuite.ACAD2017.Services;
using _3DS_CivilSurveySuite.C3D2017.Services;
using _3DS_CivilSurveySuite.UI.Views;
using _3DS_CivilSurveySuite.ViewModels;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Runtime;

[assembly: CommandClass(typeof(_3DS_CivilSurveySuite.C3D2017.C3DPalettes))]
namespace _3DS_CivilSurveySuite.C3D2017
{
    /// <summary>
    /// PaletteFactory class for hooking up Views and ViewModels to be
    /// displayed as Palettes in AutoCAD Civil3D.
    /// </summary>
    public class C3DPalettes
    {
        private bool _paletteVisible;
        private readonly IPaletteService _paletteService;

        public C3DPalettes()
        {
            _paletteService = ServiceLocator.Container.GetInstance<IPaletteService>();
        }

        [CommandMethod("3DSShowConnectLinePalette")]
        public void ShowConnectLinePalette()
        {
            var view = new ConnectLineworkView();
            var vm = new ConnectLineworkViewModel("Properties.Settings.Default.ConnectLineworkFileName", new ConnectLineworkService());
            _paletteService.GeneratePalette(view, vm, "Linework");
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