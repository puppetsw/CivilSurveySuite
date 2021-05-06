using _3DS_CivilSurveySuite.Helpers.AutoCAD;
using Autodesk.AutoCAD.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


[assembly: CommandClass(typeof(_3DS_CivilSurveySuite.ACADCommands.Point.CadastralUtils))]
namespace _3DS_CivilSurveySuite.ACADCommands.Point
{
    public class CadastralUtils : CivilBase
    {
        public void CreatePointAtOffsetTwoLines()
        { }

        public void CreatePointOnProduction()
        { }

    }
}
