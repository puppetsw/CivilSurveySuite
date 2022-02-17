using System;
using System.IO;
using System.Reflection;
using Autodesk.AutoCAD.ApplicationServices.Core;
using Autodesk.AutoCAD.DatabaseServices;
using NUnit.Framework;

namespace _3DS_CivilSurveySuiteAcadCoreTests
{
    /// <summary>
    /// Base class for ACAD tests.
    /// </summary>
    public abstract class TestBase
    {
        /// <summary>
        /// Executes a list of delegate actions
        /// </summary>
        /// <param name="drawingFile">Path to the test drawing file.</param>
        /// <param name="testActions">Test actions to execute.</param>
        protected static void ExecuteTestActions(string drawingFile = "", params Action<Database, Transaction>[] testActions)
        {
            bool defaultDrawing;
            if (string.IsNullOrEmpty(drawingFile))
            {
                defaultDrawing = true;

                string directoryPlugin = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                drawingFile = Path.Combine(directoryPlugin, "TestDrawing.dwg");
            }
            else
            {
                defaultDrawing = false;
                if (!File.Exists(drawingFile))
                {
                    Assert.Fail($"Drawing file {drawingFile} does not exist.");
                }
            }

            Exception exception = null;
            var document = Application.DocumentManager.MdiActiveDocument;

            // Lock the document and execute the test actions.
            using (document.LockDocument())
            using (var db = new Database(defaultDrawing, false))
            {
                if (!string.IsNullOrEmpty(drawingFile))
                    db.ReadDwgFile(drawingFile, FileOpenMode.OpenForReadAndWriteNoShare, true, null);

                var oldDb = HostApplicationServices.WorkingDatabase;
                HostApplicationServices.WorkingDatabase = db; // change to the current database.

                foreach (var testAction in testActions)
                {
                    using (var tr = db.TransactionManager.StartTransaction())
                    {
                        try
                        {
                            // Execute the test action.
                            testAction(db, tr);
                        }
                        catch (Exception e)
                        {
                            exception = e;
                            tr.Commit();
                            break;
                        }
                        tr.Commit();
                    }
                }
                // Change the database back to the original.
                HostApplicationServices.WorkingDatabase = oldDb;
            }

            // From CADBloke
            // Throw exception outside of transaction.
            // Sometimes Autocad crashes when exception throws.
            if (exception != null)
            {
                throw exception;
            }
        }
    }
}