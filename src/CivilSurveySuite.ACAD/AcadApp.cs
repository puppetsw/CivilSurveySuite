﻿using System;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Customization;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using CivilSurveySuite.Common.Models;
using CivilSurveySuite.Common.Services.Interfaces;
using CivilSurveySuite.UI.Helpers;
using CivilSurveySuite.UI.Views;
using Application = Autodesk.AutoCAD.ApplicationServices.Core.Application;
using Exception = Autodesk.AutoCAD.Runtime.Exception;

namespace CivilSurveySuite.ACAD
{
    /// <summary>
    /// Provides access to several "active" objects and helper methods
    /// in the AutoCAD runtime environment.
    /// </summary>
    public sealed class AcadApp : IExtensionApplication
    {
        public const string ACAD_TOOLBAR_FILE = "3DS_CSS_ACAD.cuix";

        /// <summary>
        /// Gets the <see cref="DocumentManager"/>.
        /// </summary>
        public static DocumentCollection DocumentManager => Application.DocumentManager;

        /// <summary>
        /// Gets the active <see cref="Document"/> object.
        /// </summary>
        public static Document ActiveDocument => DocumentManager.MdiActiveDocument;

        /// <summary>
        /// Gets the active <see cref="Database"/> object.
        /// </summary>
        public static Database ActiveDatabase => ActiveDocument.Database;

        /// <summary>
        /// Gets the active <see cref="Editor"/> object.
        /// </summary>
        public static Editor Editor => ActiveDocument.Editor;

        public static ILogger Logger { get; private set; }

        public void Initialize()
        {
            try
            {
                Ioc.RegisterServices();
                Logger = Ioc.Default.GetInstance<ILogger>();
                Editor.WriteMessage($"\n{ResourceHelpers.GetLocalisedString("ACAD_Loading")}");
                Logger.Info($"{ResourceHelpers.GetLocalisedString("ACAD_Loading")}");
                Logger.Info("ACAD Services registered successfully.");
            }
            catch (InvalidOperationException e)
            {
                Editor.WriteMessage($"\n{ResourceHelpers.GetLocalisedString("ACAD_LoadingError")} {e.Message}");
                Logger.Error(e, ResourceHelpers.GetLocalisedString("ACAD_LoadingError"));
            }
        }

        public void Terminate()
        {
            Properties.Settings.Default.Save();
        }

        public static void ShowDialog<TView>() where TView : Window
        {
            var view = Ioc.GetRequiredView<TView>();
            Logger.Info($"New instance of {typeof(TView)} requested");

            try
            {
                Application.ShowModalWindow(view);
            }
            catch (Exception e)
            {
                Logger.Error(e, e.Message);
                throw;
            }
        }

        public static void ShowModelessDialog<TView>() where TView : Window
        {
            var view = Ioc.GetRequiredView<TView>();
            Logger.Info($"New instance of {typeof(TView)} requested");

            try
            {
                Application.ShowModelessWindow(view);
            }
            catch (Exception e)
            {
                Logger.Error(e, e.Message);
                throw;
            }
        }

        public static string ShowInputDialog(InputServiceOptions inputServiceOptions)
        {
            var window = Ioc.GetRequiredView<InputDialogView>();
            var dialog = (IInputDialogService)window;
            dialog.AssignOptions(inputServiceOptions);
            var showDialog = Application.ShowModalWindow(window);

            return showDialog != true ? string.Empty : dialog.ResultString;
        }

        /// <summary>
        /// Starts a transaction.
        /// </summary>
        /// <returns>Transaction.</returns>
        public static Transaction StartTransaction([CallerMemberName] string caller = "")
        {
            Logger.Info($"Transaction started from: {caller}");
            return ActiveDocument.TransactionManager.StartTransaction();
        }

        /// <summary>
        /// Starts a locked transaction.
        /// </summary>
        /// <returns>Transaction.</returns>
        public static Transaction StartLockedTransaction([CallerMemberName] string caller = "")
        {
            Logger.Info($"Locked transaction started from: {caller}");
            return ActiveDocument.TransactionManager.StartLockedTransaction();
        }

        /// <summary>
        /// Check if the user is running inside Civil 3D.
        /// </summary>
        /// <returns><c>True</c> if Civil 3D is running. Otherwise <c>false</c>.</returns>
        public static bool IsCivil3DRunning()
        {
            return SystemObjects.DynamicLinker.GetLoadedModules().Contains("AecBase.dbx".ToLower());
        }

        /// <summary>
        /// Loads a partial cui file.
        /// </summary>
        /// <param name="fileName">Path to cui file.</param>
        public static void LoadCuiFile(string fileName)
        {
            // Is Cui file already loaded?
            // If it is, we will unload so it can be reloaded.
            if (IsCuiFileLoaded(fileName))
            {
                Logger.Info($"CUI file {fileName} already loaded. Unloading...");
                UnloadCuiFile(fileName);
            }

            // Load the CUI file.
            var filePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + Path.DirectorySeparatorChar + fileName;

            if (!File.Exists(filePath))
            {
                Editor.WriteMessage($"\nCould not find CUI file: {filePath}");
                Logger.Info($"Could not find CUI file: {filePath}");
                return;
            }

            Editor.WriteMessage("\n");
            Logger.Info($"Loading CUI file: {filePath}");
            Autodesk.AutoCAD.ApplicationServices.Application.LoadPartialMenu(filePath);
        }

        /// <summary>
        /// Unloads a partial cui file.
        /// </summary>
        /// <param name="fileName">Path to cui file.</param>
        public static void UnloadCuiFile(string fileName)
        {
            // Unload the CUI file.
            var filePath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + Path.DirectorySeparatorChar + fileName;
            Autodesk.AutoCAD.ApplicationServices.Application.UnloadPartialMenu(filePath);
        }

        private static bool IsCuiFileLoaded(string fileName)
        {
            var mainCuiFile = SystemVariables.MENUNAME;
            mainCuiFile += ".cuix";

            var cs = new CustomizationSection(mainCuiFile);
            foreach (var file in cs.PartialCuiFiles)
            {
                if (string.Equals(Path.GetFileName(file), fileName, StringComparison.InvariantCultureIgnoreCase))
                {
                    return true;
                }
            }
            return false;
        }

        public static void WriteMessage(string message)
        {
            Editor.WriteMessage($"\n{message}");
        }

        public static void WriteErrorMessage(Exception e)
        {
            Editor.WriteMessage($"\nError: {e.ErrorStatus}");
            Editor.WriteMessage($"\nException: {e.Message}");
        }
    }
}