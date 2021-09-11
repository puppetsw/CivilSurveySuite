// Copyright Scott Whitney. All Rights Reserved.
// Reproduction or transmission in whole or in part, any form or by any
// means, electronic, mechanical or otherwise, is prohibited without the
// prior written consent of the copyright owner.

using System;
using System.Collections.Generic;
using _3DS_CivilSurveySuite.Core;
using _3DS_CivilSurveySuite.Model;
using _3DS_CivilSurveySuite.UI.Services;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.GraphicsInterface;
using Autodesk.AutoCAD.Internal;
using Point = _3DS_CivilSurveySuite.Model.Point;

namespace _3DS_CivilSurveySuite.ACAD2017.Services
{
    /// <summary>
    /// Traverse service implementation.
    /// </summary>
    public class TraverseService : ITraverseService, IDisposable
    {
        private readonly TransientGraphics _graphics;

        private Point _basePoint;
        
        public TraverseService()
        {
            _graphics = new TransientGraphics();
        }

        public void DrawLines(IEnumerable<TraverseAngleObject> traverse)
        {
            _graphics?.ClearGraphics();

            if (!_basePoint.IsValid())
                return;

            using (var tr = AcadApp.StartLockedTransaction())
            {
                var coordinates = PointHelpers.TraverseAngleObjectsToCoordinates(traverse, _basePoint);
                LineUtils.DrawLines(tr, coordinates.ToListOfPoint3d());
                tr.Commit();
                AcadApp.Editor.Regen();
            }
        }

        public void DrawLines(IEnumerable<TraverseObject> traverse)
        { 
            _graphics?.ClearGraphics();

            if (!_basePoint.IsValid())
                return;

            using (var tr = AcadApp.StartLockedTransaction())
            {
                var coordinates = PointHelpers.TraverseObjectsToCoordinates(traverse, _basePoint);
                LineUtils.DrawLines(tr, coordinates.ToListOfPoint3d());
                tr.Commit();
                AcadApp.Editor.Regen();
            }
        }

        public void DrawTransientLines(IEnumerable<TraverseObject> traverse)
        {
            if (_graphics == null)
                return;

            _graphics.ClearGraphics();
            var coordinates = PointHelpers.TraverseObjectsToCoordinates(traverse, _basePoint);
            DrawTraverseGraphics(_graphics, coordinates);
            AcadApp.Editor.UpdateScreen();
        }

        public void DrawTransientLines(IEnumerable<TraverseAngleObject> traverse)
        {
            if (_graphics == null)
                return;

            _graphics.ClearGraphics();
            var coordinates = PointHelpers.TraverseAngleObjectsToCoordinates(traverse, _basePoint);
            DrawTraverseGraphics(_graphics, coordinates);
            AcadApp.Editor.UpdateScreen();
        }

        public void SetBasePoint()
        {
            Utils.SetFocusToDwgView();

            if (!EditorUtils.GetPoint(out Point3d basePoint, "\n3DS> Select base point: "))
                return;

            AcadApp.Editor.WriteMessage($"\n3DS> Base point: X:{Math.Round(_basePoint.X, 4)}, Y:{Math.Round(_basePoint.Y, 4)}");

            _basePoint = basePoint.ToPoint();
        }
        
        public void ClearGraphics()
        {
            _graphics?.ClearGraphics();
        }
        
        private static void DrawTraverseGraphics(TransientGraphics graphics, IReadOnlyList<Point> coordinates)
        {
            // Clear existing graphics
            graphics.ClearGraphics();

            var points = coordinates.ToListOfPoint3d();

            graphics.DrawLines(points, TransientDrawingMode.Highlight);

            // If the list count is greater than 2, we show the boxes
            if (coordinates.Count >= 2)
            {
                var endPoint = coordinates[coordinates.Count - 1].ToPoint3d();
                var startPoint = coordinates[0].ToPoint3d();

                graphics.DrawBox(endPoint, 4);
                graphics.DrawX(endPoint, 4);

                graphics.DrawBox(startPoint, 4);
                graphics.DrawX(startPoint, 4);
            }
        }

        public void Dispose()
        {
            _graphics?.Dispose();
        }
    }
}
