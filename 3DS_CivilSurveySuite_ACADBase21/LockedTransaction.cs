using System;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Runtime;

namespace _3DS_CivilSurveySuite_ACADBase21
{
    public class LockedTransaction : Transaction
    {
        private readonly DocumentLock _docLock;
        public LockedTransaction(Transaction tr, DocumentLock docLock) : base(tr.UnmanagedObject, tr.AutoDelete)
        {
            Interop.DetachUnmanagedObject(tr);
            GC.SuppressFinalize(tr);
            _docLock = docLock;
        }

        protected override void Dispose(bool A_1)
        {
            base.Dispose(A_1);
            if (A_1)
            {
                _docLock.Dispose();
            }
        }
    }
}
