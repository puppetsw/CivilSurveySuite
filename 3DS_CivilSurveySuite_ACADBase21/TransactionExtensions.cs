using Autodesk.AutoCAD.ApplicationServices;

namespace _3DS_CivilSurveySuite_ACADBase21
{
    public static class TransactionExtensions
    {
        /// <summary>
        /// Creates a new database transaction that allows to open,
        /// read, and modified objects in the database.
        /// </summary>
        /// <remarks>
        /// For use with Palettes as they require the document to be locked.
        /// </remarks>
        /// <returns>Returns a new database transaction and also locks the document.</returns>
        public static LockedTransaction StartLockedTransaction(this TransactionManager tm)
        {
            DocumentLock doclock = Autodesk.AutoCAD.ApplicationServices.Core.Application.DocumentManager.MdiActiveDocument.LockDocument();
            return new LockedTransaction(tm.StartTransaction(), doclock);
        }

        /// <summary>
        /// Creates a new database transaction that allows to open,
        /// read, and modified objects in the database.
        /// </summary>
        /// <remarks>
        /// For use with Palettes as they require the document to be locked.
        /// </remarks>
        /// <returns>Returns a new database transaction and also locks the document.</returns>
        public static LockedTransaction StartLockedTransaction(this TransactionManager tm, DocumentLockMode lockMode, string globalCommandName, string localCommandName, bool promptIfFails)
        {
            DocumentLock doclock = Autodesk.AutoCAD.ApplicationServices.Core.Application.DocumentManager.MdiActiveDocument.LockDocument(lockMode, globalCommandName, localCommandName, promptIfFails);
            return new LockedTransaction(tm.StartTransaction(), doclock);
        }
    }
}
