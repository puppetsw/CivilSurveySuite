// Copyright Scott Whitney. All Rights Reserved.
// Reproduction or transmission in whole or in part, any form or by any
// means, electronic, mechanical or otherwise, is prohibited without the
// prior written consent of the copyright owner.

using System.Reflection;
using _3DS_CivilSurveySuite_ACADBase21;
using Autodesk.AutoCAD.Runtime;

[assembly: ExtensionApplication(typeof(_3DS_CivilSurveySuite.MyPlugin))]
namespace _3DS_CivilSurveySuite
{
    public class MyPlugin : IExtensionApplication
    {
        private Palettes _palettes;

        public void Initialize()
        {
            double num = AutoCADActive.AutoCADVersion();
            if (num < 21.0 || 23.1 < num)
            {
                AutoCADActive.Editor.WriteMessage("\n3DS> Warning: Cannot load 3DS_CivilSurveySuite.dll It was written for AutoCAD 2017 to 2020. ");
                throw new Exception();
            }

            AutoCADActive.Editor.WriteMessage("\n3DS> Initializing - 3DS_CivilSurveySuite.dll");

            //HACK: Force Behaviors assembly to load.
            Assembly.Load("Microsoft.Xaml.Behaviors");
            //Assembly.Load("3DS_CivilSurveySuite.Commands");

            _palettes = new Palettes();
            _palettes.HookupEvents();
        }

        public void Terminate()
        {
            _palettes.UnhookEvents();
            _palettes.Dispose();
        }
    }
}