// Copyright Scott Whitney. All Rights Reserved.
// Reproduction or transmission in whole or in part, any form or by any
// means, electronic, mechanical or otherwise, is prohibited without the
// prior written consent of the copyright owner.

using _3DS_CivilSurveySuite.Shared.Services.Interfaces;
using Autodesk.AutoCAD.Geometry;

namespace _3DS_CivilSurveySuite.ACAD2017
{
    public class TestDebugCommand : IAcadCommand
    {
        private void TestCircleArc3d()
        {
            if (!EditorUtils.TryGetPoint("\nFirst point", out Point3d point1))
                return;

            if (!EditorUtils.TryGetPoint("\nSecond point", out Point3d point2))
                return;

            if (!EditorUtils.TryGetPoint("\nThird point", out Point3d point3))
                return;


            var circleArc = new CircularArc3d(point1, point2, point3);

            var points = circleArc.CurvePoints();

            using (TransientGraphics graphics = new TransientGraphics())
            {
                foreach (Point3d point3d in points)
                {
                    graphics.DrawPlus(point3d, 8);
                }

                EditorUtils.TryGetString("\nPause", out var _);
            }
        }


        public void Execute()
        {
            TestCircleArc3d();
        }
    }
}