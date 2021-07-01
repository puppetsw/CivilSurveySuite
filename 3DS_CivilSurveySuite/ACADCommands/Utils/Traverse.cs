using _3DS_CivilSurveySuite.Helpers.AutoCAD;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;

[assembly: CommandClass(typeof(_3DS_CivilSurveySuite.ACADCommands.Utils.Traverse))]
namespace _3DS_CivilSurveySuite.ACADCommands.Utils
{
    /// <summary>
    /// Traverse utility with command line input
    /// </summary>
    public class Traverse : CivilBase
    {
        [CommandMethod("3DSTraverse")]
        public void TraverseCommand()
        {
            PickBasePoint();

            PromptKeywordOptions pko = new PromptKeywordOptions("\n3DS> Accept traverse and draw linework? ");
            pko.AppendKeywordsToMessage = true;
            pko.Keywords.Add("Accept");
            pko.Keywords.Add("Cancel");
            pko.Keywords.Add("Redraw");
        }

        private void PickBasePoint()
        {

        }
    }
}
