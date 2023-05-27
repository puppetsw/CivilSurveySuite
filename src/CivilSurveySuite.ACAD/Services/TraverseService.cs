using System;
using System.Collections.Generic;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.GraphicsInterface;
using Autodesk.AutoCAD.Internal;
using CivilSurveySuite.Shared.Helpers;
using CivilSurveySuite.Shared.Models;
using CivilSurveySuite.Shared.Services.Interfaces;
using Point = CivilSurveySuite.Shared.Models.Point;

namespace CivilSurveySuite.ACAD.Services
{
    /// <summary>
    /// Traverse service implementation.
    /// </summary>
    /// <remarks>
    /// Things to do...
    /// * When user selects a line in traverse window it should clear the current traverse?
    /// * This should fix the issue with the basePoint not being set and zoom?
    /// * Add ability to select multiple lines.
    /// * Pick in order, so we can test if the next line is connected to the previous.
    /// * Add load/save to traverse palette
    /// * Add Curves to traverse?
    /// </remarks>
    public class TraverseService : ITraverseService, IDisposable
    {
        private readonly TransientGraphics _graphics;

        private Shared.Models.Point _basePoint;

        public TraverseService()
        {
            _graphics = new TransientGraphics();
        }

        public void DrawLines(IEnumerable<TraverseObject> traverse)
        {
            if (!_basePoint.IsValid())
            {
                AcadApp.Editor.WriteMessage("\n3DS> No base point set. ");
                return;
            }

            var coordinates = PointHelpers.TraverseObjectsToCoordinates(traverse, _basePoint);
            DrawTraverseLines(coordinates);
        }

        public void DrawTransientLines(IEnumerable<TraverseObject> traverse)
        {
            if (_graphics == null)
                return;

            var coordinates = PointHelpers.TraverseObjectsToCoordinates(traverse, _basePoint);
            DrawTraverseGraphics(_graphics, coordinates);
        }

        public void SetBasePoint()
        {
            Utils.SetFocusToDwgView();

            if (!EditorUtils.TryGetPoint("\n3DS> Select base point: ", out Point3d basePoint))
                return;

            AcadApp.Editor.WriteMessage($"\n3DS> Base point: X:{Math.Round(_basePoint.X, 4)}, Y:{Math.Round(_basePoint.Y, 4)}");
            AcadApp.Editor.WriteMessage("\n");

            _basePoint = basePoint.ToPoint();
        }

        public void ClearGraphics()
        {
            _graphics?.ClearGraphics();
        }

        public void ZoomTo(IEnumerable<TraverseObject> traverse)
        {
            var coordinates = PointHelpers.TraverseObjectsToCoordinates(traverse, _basePoint);
            var minMax = PointHelpers.GetBounds(coordinates);
            EditorUtils.ZoomToWindow(minMax.MinPoint.ToPoint3d(), minMax.MaxPoint.ToPoint3d());
        }

        public AngleDistance SelectLine()
        {
            Utils.SetFocusToDwgView();
            AngleDistance angDist = default;

            using (var tr = AcadApp.StartLockedTransaction())
            {
                var line = LineUtils.GetLineOrPolylineSegment(tr);

                if (line != null)
                {
                    angDist.Angle = AngleHelpers.GetAngleBetweenPoints(line.StartPoint.ToPoint(), line.EndPoint.ToPoint());
                    angDist.Distance = line.Length;

                    // Check if basePoint is set?
                    if (_basePoint.Equals(Point.Origin))
                    {
                        _basePoint = line.StartPoint.ToPoint();
                    }
                }
                tr.Commit();
            }
            return angDist;
        }

        public IEnumerable<TraverseObject> SelectLines()
        {
            Utils.SetFocusToDwgView();
            //BUG: There is a bug when the loop starts again, there is a flash of the TraverseWindow.

            var traverse = new List<TraverseObject>();
            var basePoint = Point.Origin;

            var lastEndPoint = Point.Origin;

            using (var tr = AcadApp.StartLockedTransaction())
            {
                while (true)
                {
                    var line = LineUtils.GetLineOrPolylineSegment(tr);

                    if (line == null)
                        break;

                    if (!lastEndPoint.Equals(Point.Origin) && !line.StartPoint.ToPoint().Equals(lastEndPoint))
                    {
                        AcadApp.Editor.WriteMessage("\n3DS> Line was not connected to previous.");
                        continue;
                    }

                    var angle = AngleHelpers.GetAngleBetweenPoints(line.StartPoint.ToPoint(), line.EndPoint.ToPoint());
                    var distance = line.Length;

                    // Set new base point.
                    if (basePoint.Equals(Point.Origin))
                        basePoint = line.StartPoint.ToPoint();

                    traverse.Add(new TraverseObject(angle.ToDouble(), distance));
                    _graphics.DrawLine(line.StartPoint, line.EndPoint);

                    lastEndPoint = line.EndPoint.ToPoint();
                }

                ClearGraphics();
                tr.Commit();
            }

            _basePoint = basePoint;
            return traverse;
        }

        public IEnumerable<TraverseObject> GetTraverseData(string fileName)
        {
            throw new NotImplementedException();
        }

        public void WriteTraverseData(string fileName, IEnumerable<TraverseObject> traverseData)
        {
            throw new NotImplementedException();
        }

        private void DrawTraverseLines(IEnumerable<Shared.Models.Point> coordinates)
        {
            _graphics?.ClearGraphics();

            using (var tr = AcadApp.StartLockedTransaction())
            {
                LineUtils.DrawLines(tr, coordinates.ToListOfPoint3d());
                tr.Commit();
                AcadApp.Editor.Regen();
            }
        }

        private static void DrawTraverseGraphics(TransientGraphics graphics, IReadOnlyList<Shared.Models.Point> coordinates)
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
            AcadApp.Editor.UpdateScreen();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing)
                return;

            _graphics?.ClearGraphics();
            _graphics?.Dispose();
        }
    }
}