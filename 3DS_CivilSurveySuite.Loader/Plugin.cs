// Copyright Scott Whitney. All Rights Reserved.
// Reproduction or transmission in whole or in part, any form or by any
// means, electronic, mechanical or otherwise, is prohibited without the
// prior written consent of the copyright owner.

using System.Reflection;
using Autodesk.AutoCAD.Runtime;

[assembly: ExtensionApplication(typeof(_3DS_CivilSurveySuite.Loader.Loader))]
namespace _3DS_CivilSurveySuite.Loader
{
    public class Loader : IExtensionApplication
    {
        public void Initialize()
        {
            //double num = AcadUtils.AutoCADVersion();
            //if (num < 21.0 || 23.1 < num)
            //{
            //    Application.DocumentManager.CurrentDocument.Editor.WriteMessage("\n3DS> Warning: Cannot load 3DS_CivilSurveySuite.dll It was written for AutoCAD 2017 to 2020. ");
            //    throw new Exception();
            //}

            //AcadUtils.Editor.WriteMessage("\n3DS> Initializing - 3DS_CivilSurveySuite.dll");

            Assembly.Load("Microsoft.Xaml.Behaviors");
            //Assembly.Load("SimpleInjector");
            Assembly.Load("3DS_CivilSurveySuite.ACAD2017");
            Assembly.Load("3DS_CivilSurveySuite.C3D2017");
        }

        public void Terminate()
        {
        }
    }
}