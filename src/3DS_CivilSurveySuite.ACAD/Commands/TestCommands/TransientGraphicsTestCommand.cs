using System;
using _3DS_CivilSurveySuite.Shared.Models;
using _3DS_CivilSurveySuite.Shared.Services.Interfaces;
using Autodesk.AutoCAD.Geometry;

namespace _3DS_CivilSurveySuite.ACAD
{
    public class TransientGraphicsTestCommand : IAcadCommand, IDisposable
    {
        private readonly TransientGraphics _graphics = new TransientGraphics();

        public void Execute()
        {
            _graphics.DrawLine(new Point3d(0, 0, 0), new Point3d(45, 45, 0));

            _graphics.DrawSquare(new Point3d(0, 50, 0), 10);

            _graphics.DrawRectangle(new Point3d(0, 100, 0), 10, 30);

            _graphics.DrawCircle(new Point3d(0, 150, 0), 1);

            _graphics.DrawBox(new Point3d(0, 200, 0), 10, true);

            _graphics.DrawTriangle(new Point3d(0, 250, 0), 10);

            _graphics.DrawX(new Point3d(0, 300, 0), 10);

            _graphics.DrawDot(new Point3d(0, 350, 0), 10);

            _graphics.DrawPlus(new Point3d(0, 400, 0), 10);

            _graphics.DrawArrow(new Point3d(0, 450, 0), 0, 10, true);

            _graphics.DrawText(new Point3d(0, 500, 0), "Test", 5, new Angle());
        }

        public void Dispose()
        {
            _graphics?.Dispose();
        }
    }
}
