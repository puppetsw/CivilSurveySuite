using System;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Runtime;

namespace _3DS_CivilSurveySuite.ACAD2017
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

        protected override void Dispose(bool a1)
        {
            base.Dispose(a1);
            if (a1)
            {
                _docLock.Dispose();
            }
        }
    }
}