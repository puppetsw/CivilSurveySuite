using _3DS_CivilSurveySuite_ACADBase21;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;

[assembly: CommandClass(typeof(Traverse))]
namespace _3DS_CivilSurveySuite_ACADBase21
{
    /// <summary>
    /// Traverse utility with command line input
    /// </summary>
    public class Traverse
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
