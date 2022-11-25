using System;
using _3DS_CivilSurveySuite.Shared.Models;
using _3DS_CivilSurveySuite.Shared.Services.Interfaces;
using Autodesk.AutoCAD.Geometry;

namespace _3DS_CivilSurveySuite.ACAD
{
    public class TransientGraphicsUndoTestCommand : IAcadCommand, IDisposable
    {
        private readonly TransientGraphics _graphics = new TransientGraphics();

        public void Execute()
        {
            _graphics.DrawLine(new Point3d(0, 0, 0), new Point3d(45, 45, 0));
            _graphics.DrawSquare(new Point3d(0, 50, 0), 10);
            _graphics.DrawRectangle(new Point3d(0, 100, 0), 10, 30);
            EditorUtils.TryGetString("Pausing", out _);
            _graphics.Undo();
            EditorUtils.TryGetString("Pausing", out _);

            _graphics.DrawCircle(new Point3d(0, 150, 0), 1);
            EditorUtils.TryGetString("Pausing", out _);
            _graphics.Undo();
            EditorUtils.TryGetString("Pausing", out _);

            _graphics.DrawBox(new Point3d(0, 200, 0), 10, true);
            EditorUtils.TryGetString("Pausing", out _);
            _graphics.Undo();
            EditorUtils.TryGetString("Pausing", out _);

            _graphics.DrawTriangle(new Point3d(0, 250, 0), 10);
            EditorUtils.TryGetString("Pausing", out _);
            _graphics.Undo();
            EditorUtils.TryGetString("Pausing", out _);

            _graphics.DrawX(new Point3d(0, 300, 0), 10);
            EditorUtils.TryGetString("Pausing", out _);
            _graphics.Undo();
            EditorUtils.TryGetString("Pausing", out _);

            _graphics.DrawDot(new Point3d(0, 350, 0), 10);
            EditorUtils.TryGetString("Pausing", out _);
            _graphics.Undo();
            EditorUtils.TryGetString("Pausing", out _);

            _graphics.DrawPlus(new Point3d(0, 400, 0), 10);
            EditorUtils.TryGetString("Pausing", out _);
            _graphics.Undo();
            EditorUtils.TryGetString("Pausing", out _);

            _graphics.DrawArrow(new Point3d(0, 450, 0), 0, 10, true);
            EditorUtils.TryGetString("Pausing", out _);
            _graphics.Undo();
            EditorUtils.TryGetString("Pausing", out _);

            _graphics.DrawText(new Point3d(0, 500, 0), "Test", 5, new Angle());
            EditorUtils.TryGetString("Pausing", out _);
            _graphics.Undo();
            EditorUtils.TryGetString("Pausing", out _);
        }

        public void Dispose()
        {
            _graphics?.Dispose();
        }
    }
}
