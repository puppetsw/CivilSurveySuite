using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.Windows;
using System;

[assembly: CommandClass(typeof(_3DS_CivilSurveySuite.Traverse.TraverseCommands))]
namespace _3DS_CivilSurveySuite.Traverse
{
    public class TraverseCommands : CivilBase
    {
        static PaletteSet m_PalSet = null;

        [CommandMethod("3DSShowTraversePalette")]
        public void ShowTraversePalette()
        {
            //WriteMessage("Showing Traverse Palette");
            TraversePalette tw = new TraversePalette();

            if (m_PalSet == null)
            {
                m_PalSet = new PaletteSet("3DS Traverse", new Guid("39663E77-EAC7-409A-87E4-4E6E15A5D05A"));
                m_PalSet.AddVisual("TraverseWindow", tw);

                //m_PalSet.StateChanged += (s, e) =>
                //{
                //    if (e.NewState == StateEventIndex.Hide)
                //        WriteMessage("Closing Traverse Palette\n");
                //};

                m_PalSet.EnableTransparency(true);
                m_PalSet.KeepFocus = true;
            }

            m_PalSet.Visible = true;
        }
    }
}
