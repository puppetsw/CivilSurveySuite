﻿using System;
using System.Windows;
using CivilSurveySuite.CIVIL;
using Autodesk.AutoCAD.Runtime;
using Autodesk.Civil.ApplicationServices;
using CivilSurveySuite.ACAD;
using CivilSurveySuite.UI.Helpers;
using Application = Autodesk.AutoCAD.ApplicationServices.Core.Application;

[assembly: ExtensionApplication(typeof(C3DApp))]
namespace CivilSurveySuite.CIVIL
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
        private const string CIVIL_TOOLBAR_CUI_FILE = "3DS_CSS_CIVIL.cuix";

        public static CivilDocument ActiveDocument => CivilApplication.ActiveDocument;

        public static bool IsCivil3D() => SystemObjects.DynamicLinker.GetLoadedModules().Contains("AecBase.dbx".ToLower());

        public void Initialize()
        {
            // Check if ACAD is loaded.

            AcadApp.Editor.WriteMessage($"\n{ResourceHelpers.GetLocalisedString("C3D_Loading")}");
            AcadApp.Logger?.Info($"{ResourceHelpers.GetLocalisedString("C3D_Loading")}");

            try
            {
                Ioc.RegisterServices();
            }
            catch (InvalidOperationException e)
            {
                AcadApp.Editor.WriteMessage($"\n{ResourceHelpers.GetLocalisedString("C3D_LoadingError")} {e.Message}");
                AcadApp.Logger?.Error(e, ResourceHelpers.GetLocalisedString("C3D_LoadingError"));
            }
        }

        public void Terminate()
        {
            // Nothing to cleanup.
        }

        public static void LoadMenu()
        {
            if (AcadApp.IsCivil3DRunning())
            {
                AcadApp.LoadCuiFile(CIVIL_TOOLBAR_CUI_FILE);
            }
        }

        public static void ShowDialog<TView>() where TView : Window
        {
            var view = Ioc.GetRequiredView<TView>();
            AcadApp.Logger?.Info($"Creating instance of {typeof(TView)}");
            Application.ShowModalWindow(view);
        }
    }
}
