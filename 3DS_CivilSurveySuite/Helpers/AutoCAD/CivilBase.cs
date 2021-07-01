using System;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.Civil.ApplicationServices;

namespace _3DS_CivilSurveySuite.Helpers.AutoCAD
{
    /// <summary>
    /// Base class for all sample command extensions.
    /// </summary>
    public class CivilBase : IDisposable
    {
        #region Documents and Database Access

        protected DocumentCollection AcaddocManager => Autodesk.AutoCAD.ApplicationServices.Core.Application.DocumentManager;

        /// <summary>
        /// Returns instance to active AutoCAD document.
        /// </summary>
        protected Document Acaddoc => AcaddocManager.MdiActiveDocument;


        /// <summary>
        /// Returns instance to active Civil 3D document.
        /// </summary>
        protected CivilDocument Civildoc => CivilApplication.ActiveDocument;

        /// <summary>
        /// Returns the document's editor instance.
        /// </summary>
        protected Editor Editor => Acaddoc.Editor;

        #endregion

        /// <summary>
        /// Creates a new database transaction that allows to open,
        /// read, and modified objects in the database.
        /// </summary>
        /// <returns>Returns a new database transaction.</returns>
        protected Transaction StartTransaction() => Acaddoc.TransactionManager.StartTransaction();

        /// <summary>
        /// Writes the specified message to the AutoCAD command-line on a new line with the 3DS> Prefix
        /// </summary>
        /// <param name="message"></param>
        protected void WriteMessage(string message) => Editor.WriteMessage("\n3DS> {0}", message);

        /// <summary>
        /// Check if the database contains the specified layer
        /// </summary>
        /// <param name="layerName">Name of the layer.</param>
        /// <param name="tr">The transaction.</param>
        /// <returns><c>true</c> if the specified layer name has layer; otherwise, <c>false</c>.</returns>
        protected bool HasLayer(string layerName, Transaction tr)
        {
            if (string.IsNullOrEmpty(layerName))
            {
                return false;
            }

            LayerTable layerTable = tr.GetObject(Acaddoc.Database.LayerTableId, OpenMode.ForRead) as LayerTable;
            return layerTable.Has(layerName);
        }

        /// <summary>
        /// Creates a layer in the database
        /// </summary>
        /// <param name="layerName"></param>
        /// <param name="tr"></param>
        /// <exception cref="ArgumentNullException"></exception>
        protected void CreateLayer(string layerName, Transaction tr)
        {
            if (string.IsNullOrEmpty(layerName))
            {
                throw new ArgumentNullException("layerName");
            }

            LayerTable layerTable = tr.GetObject(Acaddoc.Database.LayerTableId, OpenMode.ForRead) as LayerTable;

            if (layerTable.Has(layerName))
            {
                return;
            }

            LayerTableRecord ltr = new LayerTableRecord();
            ltr.Name = layerName;
            layerTable.UpgradeOpen();
            layerTable.Add(ltr);
            tr.AddNewlyCreatedDBObject(ltr, true);
        }

        public void Dispose()
        {
        }
    }
}


