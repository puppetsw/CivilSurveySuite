using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using System;
using System.Collections.Generic;
using System.Text;

[assembly: CommandClass(typeof(_3DS_CivilSurveySuite.Test))]
namespace _3DS_CivilSurveySuite
{
    public class Test : CivilBase
    {
        [CommandMethod("3DSTestWindow")]
        public void Initialize()
        {
            TraverseWindow tw = new TraverseWindow();
            //tw.Show();
            tw.ShowDialog();
        }

        [CommandMethod("3DSTestVectors")]
        public void TestVectors()
        {
            double startX = 2979.94088141;
            double startY = 4971.60099809;
            var pt2dStart = new Point2d(startX, startY);

            double endX = 2991.32887985;
            double endY = 4971.88357205;
            var pt2dEnd = new Point2d(endX, endY);

            double expectedAngle = 0.0248082219094586;

            var vector = new Vector2d(startX, startY);
            double result = vector.GetAngleTo(new Vector2d(endX, endY));

            WriteMessage("expected: " + expectedAngle);
            WriteMessage("got: " + result);
        }
    }
}
