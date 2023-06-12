using System;
using Autodesk.AutoCAD.DatabaseServices;

namespace CivilSurveySuite.ACAD
{
    public static class LayerUtils
    {
        /// <summary>
        /// Check if the database contains the specified layer
        /// </summary>
        /// <param name="layerName">Name of the layer.</param>
        /// <param name="tr">The transaction.</param>
        /// <returns><c>true</c> if the specified layer name has layer; otherwise, <c>false</c>.</returns>
        public static bool HasLayer(string layerName, Transaction tr)
        {
            if (string.IsNullOrEmpty(layerName))
            {
                return false;
            }

            var layerTable = tr.GetObject(AcadApp.ActiveDocument.Database.LayerTableId, OpenMode.ForRead) as LayerTable;

            return layerTable != null && layerTable.Has(layerName);
        }

        /// <summary>
        /// Creates a layer in the database
        /// </summary>
        /// <param name="layerName"></param>
        /// <param name="tr"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public static void CreateLayer(string layerName, Transaction tr)
        {
            if (string.IsNullOrEmpty(layerName))
            {
                throw new ArgumentNullException(nameof(layerName));
            }

            var layerTable = (LayerTable)tr.GetObject(AcadApp.ActiveDocument.Database.LayerTableId, OpenMode.ForRead);

            if (layerTable.Has(layerName))
            {
                return;
            }

            var ltr = new LayerTableRecord { Name = layerName };
            layerTable.UpgradeOpen();
            layerTable.Add(ltr);
            tr.AddNewlyCreatedDBObject(ltr, true);
        }
    }
}