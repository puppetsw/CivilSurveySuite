// AutoCAD References
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
// Civil 3D References
using Autodesk.Civil.ApplicationServices;
using System;

namespace _3DS_CivilSurveySuite.Helpers.AutoCAD
{
    /// <summary>
    /// Base class for all sample command extensions.
    /// </summary>
    public class CivilBase : IDisposable
    {
        #region Documents and Database Access

        /// <summary>
        /// Returns instance to active AutoCAD document.
        /// </summary>
        protected Document Acaddoc
        {
            get
            {
                if (null == m_AcadDocument)
                {
                    m_AcadDocument = Application.DocumentManager.MdiActiveDocument;
                    //TODO: Fix problem with document switching
                }
                return m_AcadDocument;
            }
        }

        /// <summary>
        /// Returns instance to active Civil 3D document.
        /// </summary>
        protected CivilDocument Civildoc
        {
            get
            {
                if (null == m_CivilDocument)
                {
                    m_CivilDocument = CivilApplication.ActiveDocument;
                }
                return m_CivilDocument;
            }
        }

        /// <summary>
        /// Returns the document's editor instance.
        /// </summary>
        protected Editor Editor
        {
            get
            {
                return Acaddoc.Editor;
            }
        }

        #endregion

        /// <summary>
        /// Creates a new database transaction that allows to open,
        /// read, and modified objects in the database.
        /// </summary>
        /// <returns>Returns a new database transaction.</returns>
        protected Transaction startTransaction()
        {
            return Acaddoc.TransactionManager.StartTransaction();
        }

        /// <summary>
        /// Writes the specified message to the AutoCAD command-line on a new line with the 3DS> Prefix
        /// </summary>
        /// <param name="message"></param>
        protected void WriteMessage(string message)
        {
            Editor.WriteMessage("\n3DS> {0}", message);
        }

        public void Dispose()
        {
        }

        private Document m_AcadDocument = null;
        private CivilDocument m_CivilDocument = null;
    }
}


