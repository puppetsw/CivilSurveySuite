// Copyright Scott Whitney. All Rights Reserved.
// Reproduction or transmission in whole or in part, any form or by any
// means, electronic, mechanical or otherwise, is prohibited without the
// prior written consent of the copyright owner.

using Autodesk.AutoCAD.Runtime;

[assembly: ExtensionApplication(typeof(_3DS_CivilSurveySuite.ACAD2017.AcadPlugin))]
namespace _3DS_CivilSurveySuite.ACAD2017
{
    public class AcadPlugin : IExtensionApplication
    {
        private const string _3DS_CUI_FILE = "3DS_CSS_ACAD.cuix";

        public void Initialize()
        {
            AcadService.Register();
            AcadApp.LoadCuiFile(_3DS_CUI_FILE);
        }

        public void Terminate()
        {
            Properties.Settings.Default.Save();
            AcadApp.UnloadCuiFile(_3DS_CUI_FILE);
        }
    }
}