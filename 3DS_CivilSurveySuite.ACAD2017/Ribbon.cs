// Copyright Scott Whitney. All Rights Reserved.
// Reproduction or transmission in whole or in part, any form or by any
// means, electronic, mechanical or otherwise, is prohibited without the
// prior written consent of the copyright owner.

using Autodesk.AutoCAD.Runtime;
using Autodesk.Windows;

[assembly: CommandClass(typeof(_3DS_CivilSurveySuite.ACAD2017.Ribbon))]
namespace _3DS_CivilSurveySuite.ACAD2017
{
    public class Ribbon
    {
        private void Create()
        {
            RibbonControl ribbonControl = ComponentManager.Ribbon;
            var tab = new RibbonTab
            {
                Title = "Autodesk .NET forum Ribbon Sample",
                Id = "RibbonSample_TAB_ID"
            };
            ribbonControl.Tabs.Add(tab);
        }

        [CommandMethod("TestRibbon")]
        public void Load()
        {
            Create();
        }
    }
}