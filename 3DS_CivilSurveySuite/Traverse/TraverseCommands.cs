using Autodesk.AutoCAD.Runtime;

[assembly: CommandClass(typeof(_3DS_CivilSurveySuite.Traverse.TraverseCommands))]
namespace _3DS_CivilSurveySuite.Traverse
{
    public class TraverseCommands : CivilBase
    {
        [CommandMethod("3DSShowTraverseWindow")]
        public void Initialize()
        {
            TraverseWindow tw = new TraverseWindow();
            Autodesk.AutoCAD.ApplicationServices.Core.Application.ShowModalWindow(tw);
        }
    }
}
