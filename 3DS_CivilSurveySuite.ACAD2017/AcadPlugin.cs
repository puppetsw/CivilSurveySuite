// Copyright Scott Whitney. All Rights Reserved.
// Reproduction or transmission in whole or in part, any form or by any
// means, electronic, mechanical or otherwise, is prohibited without the
// prior written consent of the copyright owner.

using System;
using Autodesk.AutoCAD.Runtime;

[assembly: ExtensionApplication(typeof(_3DS_CivilSurveySuite.ACAD2017.AcadPlugin))]
namespace _3DS_CivilSurveySuite.ACAD2017
{
    public class AcadPlugin : IExtensionApplication
    {
        private const string _3DS_CUI_FILE = "3DS_CSS_ACAD.cuix";

        public void Initialize()
        {
            AcadApp.Editor.WriteMessage($"\n3DS> Loading Civil Survey Suite for AutoCAD... {System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}");

            try
            {
                AcadService.Register();
            }
            catch (InvalidOperationException e)
            {
                AcadApp.Editor.WriteMessage("\n3DS> Error Loading Civil Survey Suite for AutoCAD: " + e.Message);
            }

            if (!AcadApp.IsCivil3DRunning())
                AcadApp.LoadCuiFile(_3DS_CUI_FILE);
        }

        public void Terminate()
        {
            Properties.Settings.Default.Save();
        }
    }
}