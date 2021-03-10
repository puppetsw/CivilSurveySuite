using _3DS_CivilSurveySuite.Helpers;
using _3DS_CivilSurveySuite.Models;
using _3DS_CivilSurveySuite.Traverse;
using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.GraphicsInterface;
using Autodesk.AutoCAD.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[assembly: CommandClass(typeof(_3DS_CivilSurveySuite.Tests.TestTransientGraphics))]
namespace _3DS_CivilSurveySuite.Tests
{
    public class TestTransientGraphics : CivilBase
    {
        DBObjectCollection _markers = null;

        [CommandMethod("3DSTestTransientGraphics")]
        public void TestTransient()
        {
            PromptPointOptions ppo = new PromptPointOptions("\n3DS> Select a base point: ");
            PromptPointResult ppr = Editor.GetPoint(ppo);

            if (ppr.Status != PromptStatus.OK) return; //if we have a valid point

            ClearTransientGraphics(); //clear graphics
            _markers = new DBObjectCollection();

            //generate points
            List<TraverseItem> dmsList = new List<TraverseItem>
            {
                new TraverseItem(0, 30),
                new TraverseItem(90, 10),
                new TraverseItem(180, 30),
                new TraverseItem(270, 10)
            };

            using (Transaction tr = startTransaction())
            {
                var coords = MathHelpers.BearingAndDistanceToCoordinates(dmsList, new Point2d(ppr.Value.X, ppr.Value.Y));
                int i = 1;
                foreach (Point2d point in coords)
                {
                    //draw the coordlines
                    Line ln;
                    if (coords.Count == i)
                        break;
                    else
                        ln = new Line(new Point3d(point.X, point.Y, 0), new Point3d(coords[i].X, coords[i].Y, 0));

                    _markers.Add(ln);

                    TransientManager tm = TransientManager.CurrentTransientManager;
                    IntegerCollection intCol = new IntegerCollection();
                    tm.AddTransient(ln, TransientDrawingMode.Highlight, 128, intCol);

                    i++;
                }
                tr.Commit();
            }
        }

        private void ClearTransientGraphics()
        {
            TransientManager tm = TransientManager.CurrentTransientManager;
            IntegerCollection intCol = new IntegerCollection();

            if (_markers != null)
            {
                foreach (DBObject marker in _markers)
                {
                    tm.EraseTransient(marker, intCol);
                    marker.Dispose();
                }
            }
        }
    }
}