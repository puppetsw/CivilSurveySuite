using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.Windows;

[assembly: CommandClass(typeof(_3DS_CivilSurveySuite.Traverse.TraverseCommands))]
namespace _3DS_CivilSurveySuite.Traverse
{
    public class TraverseCommands : CivilBase
    {
        [CommandMethod("3DSShowTraverseWindow")]
        public void Initialize()
        {
            TraversePalette tw = new TraversePalette();
            //Autodesk.AutoCAD.ApplicationServices.Core.Application.ShowModalWindow(tw);

            var paletteSet = new PaletteSet("Traverse Palette");
            paletteSet.AddVisual("TraverseWindow", tw, true);
            paletteSet.Size = new System.Drawing.Size(600, 300);
            paletteSet.Visible = true;
        }
    }
}
