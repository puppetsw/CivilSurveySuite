// Copyright Scott Whitney. All Rights Reserved.
// Reproduction or transmission in whole or in part, any form or by any
// means, electronic, mechanical or otherwise, is prohibited without the
// prior written consent of the copyright owner.

using System;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;

namespace _3DS_CivilSurveySuite_ACADBase21
{
    public static class AutoCADApplicationManager
    {
        public static DocumentCollection DocumentManager => Autodesk.AutoCAD.ApplicationServices.Core.Application.DocumentManager;

        public static Document ActiveDocument => DocumentManager.MdiActiveDocument;

        public static Editor Editor => ActiveDocument.Editor;

        public static Transaction StartTransaction() => ActiveDocument.TransactionManager.StartTransaction();

        public static Transaction StartLockedTransaction() => ActiveDocument.TransactionManager.StartLockedTransaction();

        public static double AutoCADVersion()
        {
            Version version = Autodesk.AutoCAD.ApplicationServices.Core.Application.Version;
            return version.Major + version.Minor / 10.0;
        }
    }
}