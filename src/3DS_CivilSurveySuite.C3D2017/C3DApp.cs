// Copyright Scott Whitney. All Rights Reserved.
// Reproduction or transmission in whole or in part, any form or by any
// means, electronic, mechanical or otherwise, is prohibited without the
// prior written consent of the copyright owner.

using System;
using System.Windows;
using _3DS_CivilSurveySuite.ACAD2017;
using _3DS_CivilSurveySuite.C3D2017.Services;
using _3DS_CivilSurveySuite.UI.Helpers;
using _3DS_CivilSurveySuite.UI.Models;
using _3DS_CivilSurveySuite.UI.Services.Implementation;
using _3DS_CivilSurveySuite.UI.Services.Interfaces;
using _3DS_CivilSurveySuite.UI.ViewModels;
using _3DS_CivilSurveySuite.UI.Views;
using Autodesk.AutoCAD.Runtime;
using Autodesk.Civil.ApplicationServices;
using Autodesk.Civil.DatabaseServices;
using SimpleInjector;
using Application = Autodesk.AutoCAD.ApplicationServices.Core.Application;

[assembly: ExtensionApplication(typeof(_3DS_CivilSurveySuite.C3D2017.C3DApp))]
namespace _3DS_CivilSurveySuite.C3D2017
{
    /// <summary>
    /// Provides access to several "active" objects and helper methods
    /// in the AutoCAD Civil 3D runtime environment.
    /// Registers services for dependency injection.
    /// </summary>
    public sealed class C3DApp : IExtensionApplication
    {
        /// <summary>
        /// Filename of the menu file to be loaded when the application loads.
        /// </summary>
        private const string _3DS_CUI_FILE = "3DS_CSS_ACAD.cuix";

        private static Container Container { get; } = new Container();

        public static CivilDocument ActiveDocument => CivilApplication.ActiveDocument;

        public static bool IsCivil3D() => SystemObjects.DynamicLinker.GetLoadedModules().Contains("AecBase.dbx".ToLower());

        public void Initialize()
        {
            AcadApp.Editor.WriteMessage($"\n{ResourceHelpers.GetLocalisedString("C3D_Loading")} {System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}");

            try
            {
                RegisterServices();
            }
            catch (InvalidOperationException e)
            {
                AcadApp.Editor.WriteMessage($"\n{ResourceHelpers.GetLocalisedString("C3D_LoadingError")} {e.Message}");
            }

            //HACK: Removed because of coreconsole tests
            // if (AcadApp.IsCivil3DRunning())
            //     AcadApp.LoadCuiFile(_3DS_CUI_FILE);
        }

        public void Terminate()
        {
            // Nothing to cleanup.
        }

        private static void RegisterServices()
        {
            // CIVIL SERVICES
            Container.Register<ICivilSelectService, CivilSelectService>();
            Container.Register<ICogoPointService, CogoPointService>();

            Container.Register<IConnectLineworkService, ConnectLineworkService>();
            Container.Register<ICogoPointSurfaceReportService, CogoPointSurfaceReportService>();
            Container.Register<ICogoPointReplaceDuplicateService, CogoPointReplaceDuplicateService>();

            // DIALOGS
            Container.Register<IOpenFileDialogService, OpenFileDialogService>();
            Container.Register<ISaveFileDialogService, SaveFileDialogService>();
            Container.Register<IFolderBrowserDialogService, FolderBrowserDialogService>();

            // VIEWS AND VIEWMODELS
            Container.Register<SelectSurfaceView>();
            Container.Register<SelectSurfaceViewModel>();
            Container.Register<SelectPointGroupView>();
            Container.Register<SelectPointGroupViewModel>();
            Container.Register<SelectAlignmentView>();
            Container.Register<SelectAlignmentViewModel>();
            Container.Register<CogoPointMoveLabelView>();
            Container.Register<CogoPointMoveLabelViewModel>();
            Container.Register<ConnectLineworkView>();
            Container.Register<ConnectLineworkViewModel>();
            Container.Register<CogoPointEditorView>();
            Container.Register<CogoPointEditorViewModel>();
            Container.Register<CogoPointSurfaceReportView>();
            Container.Register<CogoPointSurfaceReportViewModel>();
            Container.Register<CogoPointReplaceDuplicateView>();
            Container.Register<CogoPointReplaceDuplicateViewModel>();

            Container.Verify(VerificationOption.VerifyAndDiagnose);
        }

        public static bool? ShowDialog<TView>() where TView : Window
        {
            var view = CreateWindow<TView>();
            return Application.ShowModalWindow(view);
        }

        private static Window CreateWindow<TView>() where TView : Window
        {
            return Container.GetInstance<TView>();
        }

        /// <summary>
        /// Selects the surface.
        /// </summary>
        /// <returns>TinSurface.</returns>
        public static TinSurface SelectSurface()
        {
            var window = CreateWindow<SelectSurfaceView>();
            var dialog = window as IDialogService<CivilSurface>;
            var showDialog = Application.ShowModalWindow(window);

            if (showDialog != true)
                return null;

            if (dialog == null)
                return null;

            var civilSurface = dialog.ResultObject;
            TinSurface surface;

            using (var tr = AcadApp.StartTransaction())
            {
                surface = SurfaceUtils.GetSurfaceByName(tr, civilSurface.Name);
                tr.Commit();
            }

            return surface;
        }

        /// <summary>
        /// Selects the point group.
        /// </summary>
        /// <returns>PointGroup.</returns>
        public static PointGroup SelectPointGroup()
        {
            var window = CreateWindow<SelectPointGroupView>();
            var dialog = window as IDialogService<CivilPointGroup>;
            var showDialog = Application.ShowModalWindow(window);

            if (showDialog != true)
                return null;

            if (dialog == null)
                return null;

            var civilPointGroup = dialog.ResultObject;
            PointGroup pointGroup;

            using (var tr = AcadApp.StartTransaction())
            {
                pointGroup = PointGroupUtils.GetPointGroupByName(tr, civilPointGroup.Name);
                tr.Commit();
            }

            return pointGroup;
        }


        /// <summary>
        /// Selects the alignment.
        /// </summary>
        /// <returns>Alignment.</returns>
        public static Alignment SelectAlignment()
        {
            var window = CreateWindow<SelectAlignmentView>();
            var dialog = window as IDialogService<CivilAlignment>;
            var showDialog = Application.ShowModalWindow(window);

            if (showDialog != true)
                return null;

            if (dialog == null)
                return null;

            var civilAlignment = dialog.ResultObject;
            Alignment alignment;

            using (var tr = AcadApp.StartTransaction())
            {
                alignment = AlignmentUtils.GetAlignmentByName(tr, civilAlignment.Name);
                tr.Commit();
            }

            return alignment;
        }

    }
}
