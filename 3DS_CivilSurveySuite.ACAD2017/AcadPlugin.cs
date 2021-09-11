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
        public void Initialize()
        {
            AcadService.Register();
        }

        public void Terminate()
        {
        }
    }
}
