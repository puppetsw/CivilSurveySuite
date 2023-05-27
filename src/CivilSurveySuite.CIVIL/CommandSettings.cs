// Copyright Scott Whitney. All Rights Reserved.
// Reproduction or transmission in whole or in part, any form or by any
// means, electronic, mechanical or otherwise, is prohibited without the
// prior written consent of the copyright owner.

using Autodesk.Civil.Settings;

namespace CivilSurveySuite.CIVIL
{
    public static class CommandSettings
    {
        #region SettingsCmdAddSurfaceBreaklines

        public static double GetDefaultBreaklineMidOrdinateDistance => C3DApp.ActiveDocument.Settings.GetSettings<SettingsCmdAddSurfaceBreaklines>().DataOptions.MidOrdinateDistance.Value;

        public static double GetDefaultBreaklineSupplementingDistance => C3DApp.ActiveDocument.Settings.GetSettings<SettingsCmdAddSurfaceBreaklines>().DataOptions.SupplementingDistance.Value;

        public static double GetDefaultBreaklineWeedingDistance => C3DApp.ActiveDocument.Settings.GetSettings<SettingsCmdAddSurfaceBreaklines>().DataOptions.WeedingDistance.Value;

        public static double GetDefaultBreaklineWeedingAngle => C3DApp.ActiveDocument.Settings.GetSettings<SettingsCmdAddSurfaceBreaklines>().DataOptions.WeedingAngle.Value;
        
        #endregion SettingsCmdAddSurfaceBreaklines

    }
}