using Autodesk.AutoCAD.DatabaseServices;
using Exception = Autodesk.AutoCAD.Runtime.Exception;

namespace CivilSurveySuite.ACAD
{
    public static class LineTypeUtils
    {
        private const string ACAD_LINE_TYPE_FILE_NAME = "acad.lin";

        /// <summary>
        /// If the current <see cref="Database"/> does not contain the <param>lineTypeName</param>
        /// it will try to load it.
        /// </summary>
        /// <param name="lineTypeName">Name of the line type.</param>
        /// <returns><c>true</c> if the line type loaded successfully, <c>false</c> otherwise.</returns>
        public static bool LoadLineType(string lineTypeName)
        {
            try
            {
                using (var tr = AcadApp.StartTransaction())
                {
                    var tbl = (LinetypeTable)tr.GetObject(AcadApp.ActiveDatabase.LinetypeTableId, OpenMode.ForRead);
                    if (!tbl.Has(lineTypeName))
                    {
                        AcadApp.ActiveDatabase.LoadLineTypeFile(lineTypeName, ACAD_LINE_TYPE_FILE_NAME);
                    }
                    tr.Commit();
                }
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Checks if the given line type is loaded in the current <see cref="Database"/>.
        /// </summary>
        /// <param name="lineTypeName">Name of the line type.</param>
        /// <returns><c>true</c> if the line type is loaded, otherwise <c>false</c>.</returns>
        public static bool IsLineTypeLoaded(string lineTypeName)
        {
            var isLoaded = false;

            try
            {
                using (var tr = AcadApp.StartTransaction())
                {
                    var tbl = (LinetypeTable)tr.GetObject(AcadApp.ActiveDatabase.LinetypeTableId, OpenMode.ForRead);
                    isLoaded = tbl.Has(lineTypeName);

                    tr.Commit();
                }
            }
            catch (Exception)
            {
                return isLoaded;
            }

            return isLoaded;
        }
    }
}