﻿// Copyright Scott Whitney. All Rights Reserved.
// Reproduction or transmission in whole or in part, any form or by any
// means, electronic, mechanical or otherwise, is prohibited without the
// prior written consent of the copyright owner.


using Autodesk.AutoCAD.Runtime;
using Autodesk.Civil.ApplicationServices;

namespace _3DS_CivilSurveySuite_C3DBase21
{
    /// <summary>
    /// Provides access to several "active" objects and helper methods
    /// in the AutoCAD Civil 3D runtime environment.
    /// </summary>
    public static class CivilActive
    {
        public static CivilDocument ActiveCivilDocument => CivilApplication.ActiveDocument;

        public static bool IsCivil3D() => SystemObjects.DynamicLinker.GetLoadedModules().Contains("AecBase.dbx".ToLower());
    }
}