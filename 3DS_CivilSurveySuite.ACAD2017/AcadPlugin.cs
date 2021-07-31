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
        private AcadPalettes _palettes;

        public void Initialize()
        {
            ServiceLocator.Register();

            _palettes = new AcadPalettes();
            _palettes.HookupEvents();
        }

        public void Terminate()
        {
            _palettes.UnhookEvents();
        }
    }
}
