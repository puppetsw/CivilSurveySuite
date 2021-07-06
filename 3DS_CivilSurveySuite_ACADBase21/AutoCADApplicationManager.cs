// Copyright Scott Whitney. All Rights Reserved.
// Reproduction or transmission in whole or in part, any form or by any
// means, electronic, mechanical or otherwise, is prohibited without the
// prior written consent of the copyright owner.

using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
// ReSharper disable InconsistentNaming

namespace _3DS_CivilSurveySuite_ACADBase21
{
    public static class AutoCADApplicationManager
    {
        public static DocumentCollection DocumentManager => Autodesk.AutoCAD.ApplicationServices.Core.Application.DocumentManager;

        public static Document ActiveDocument => DocumentManager.MdiActiveDocument;

        public static Editor Editor => ActiveDocument.Editor;

        public static Transaction StartTransaction() => ActiveDocument.TransactionManager.StartTransaction();

        public static Transaction StartLockedTransaction() => ActiveDocument.TransactionManager.StartLockedTransaction();
    }
}