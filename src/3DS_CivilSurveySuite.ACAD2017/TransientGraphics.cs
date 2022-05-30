// Copyright Scott Whitney. All Rights Reserved.
// Reproduction or transmission in whole or in part, any form or by any
// means, electronic, mechanical or otherwise, is prohibited without the
// prior written consent of the copyright owner.

using System;
using System.Collections.Generic;
using _3DS_CivilSurveySuite.Shared.Helpers;
using _3DS_CivilSurveySuite.Shared.Models;
using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.GraphicsInterface;
using Polyline = Autodesk.AutoCAD.DatabaseServices.Polyline;

namespace _3DS_CivilSurveySuite.ACAD2017
{
    /// <summary>
    /// A helper class to display and manage TransientGraphics
    /// within AutoCAD.
    /// </summary>
    /// <remarks>
    /// Always wrap in a try/catch/finally block or use
    /// a using statement to dispose of the graphics correctly.
    /// </remarks>
    /// <example>
    /// <code>
    /// var graphics = new TransientGraphics();
    /// try
    /// {
    ///   graphics.DrawLine(point1, point2);
    /// }
    /// finally
    /// {
    ///   graphics.Dispose();
    /// }
    /// </code>
    /// </example>
    public class TransientGraphics : IDisposable
    {
        private readonly DBObjectCollection _graphics;

        private const string DASHED_LINE_TYPE = "DASHED";

        public Color Color { get; } = Color.FromColorIndex(ColorMethod.ByPen, Settings.TransientColorIndex);

        public TransientGraphics()
        {
            _graphics = new DBObjectCollection();
        }

        private static double ScreenSize(int numPix)
        {
            var viewSize = SystemVariables.VIEWSIZE;
            var screenSize = SystemVariables.SCREENSIZE;
            return viewSize / screenSize.Y * numPix;
        }

        private static double TextSize(double textSize)
        {
            var viewSize = SystemVariables.VIEWSIZE;
            var text = viewSize / 100 * textSize;
            return Math.Round(text, 2);
        }

        private static double TextOffset(double offsetDist)
        {
            var viewSize = SystemVariables.VIEWSIZE;
            return viewSize / 100 * offsetDist;
        }

        private static void SetLineType(Entity entity)
        {
            if (LineTypeUtils.LoadLineType(DASHED_LINE_TYPE))
                entity.Linetype = DASHED_LINE_TYPE;
        }

        private void DrawAdd(DBObject entity, TransientDrawingMode mode = TransientDrawingMode.Main)
        {
            _graphics.Add(entity);
            TransientManager.CurrentTransientManager.AddTransient(entity, mode, 0, new IntegerCollection());
        }

        public void DrawLines(IReadOnlyList<Point2d> coordinates, TransientDrawingMode mode = TransientDrawingMode.Main) => DrawLines(coordinates.ToListOfPoint3d(), mode);

        public void DrawLines(IReadOnlyList<Point3d> coordinates, TransientDrawingMode mode = TransientDrawingMode.Main)
        {
            // Start a count for the next coordinate in the collection.
            var nextCoord = 1;
            // Draw the coord lines
            foreach (Point3d point in coordinates)
            {
                if (nextCoord == coordinates.Count)
                    break;

                var startPoint = new Point3d(point.X, point.Y, 0);
                var endPoint = new Point3d(coordinates[nextCoord].X, coordinates[nextCoord].Y, 0);
                DrawLine(startPoint, endPoint, mode);
                nextCoord++;
            }
        }

        public void DrawLine(Point2d point1, Point2d point2, TransientDrawingMode mode = TransientDrawingMode.Main) => DrawLine(point1.ToPoint3d(), point2.ToPoint3d(), mode);

        public void DrawLine(Line line, TransientDrawingMode mode = TransientDrawingMode.Main) => DrawLine(line.StartPoint, line.EndPoint, mode);

        public void DrawLine(Point3d point1, Point3d point2, TransientDrawingMode mode = TransientDrawingMode.Main, bool useDashedLine = true)
        {
            var line = new Line(point1, point2) { Color = Color };

            if (useDashedLine)
                SetLineType(line);

            DrawAdd(line, mode);
        }

        public void DrawLine(Curve curve, TransientDrawingMode mode = TransientDrawingMode.Main, bool useDashedLine = false)
        {
            if (useDashedLine)
                SetLineType(curve);

            DrawAdd(curve, mode);
        }

        public void DrawSquare(Point3d position, double squareSize)
        {
            throw new NotImplementedException(); //TODO: Write method
        }

        public void DrawRectangle(Point3d position, double width, double height)
        {
            throw new NotImplementedException(); //TODO: Write method
        }

        public void DrawCircle(Point3d position, double circleSize = 0.5)
        {
            var circle = new Circle(position, Vector3d.ZAxis, circleSize) { Color = Color };

            SetLineType(circle);
            DrawAdd(circle);
        }

        public void DrawBox(Point3d position, int size, bool fill = false)
        {
            double screenSize = ScreenSize(size);
            var point1 = new Point3d(position.X - screenSize * 0.5, position.Y - screenSize * 0.5, 0);
            var point2 = new Point3d(position.X - screenSize * 0.5, position.Y + screenSize * 0.5, 0);
            var point3 = new Point3d(position.X + screenSize * 0.5, position.Y + screenSize * 0.5, 0);
            var point4 = new Point3d(position.X + screenSize * 0.5, position.Y - screenSize * 0.5, 0);
            var polyline = new Polyline { Color = Color, Closed = true };
            polyline.AddVertexAt(0, point1.ToPoint2d(), 0, 0, 0);
            polyline.AddVertexAt(1, point2.ToPoint2d(), 0, 0, 0);
            polyline.AddVertexAt(2, point3.ToPoint2d(), 0, 0, 0);
            polyline.AddVertexAt(3, point4.ToPoint2d(), 0, 0, 0);
            DrawAdd(polyline);

            if (!fill)
                return;

            var solid = new Solid(point1, point2, point4, point3) { Color = Color };
            DrawAdd(solid);
        }

        public void DrawTriangle(Point3d position, int size, bool fill = true)
        {
            var screenSize = ScreenSize(size);

            var angle = new Angle(0);

            var midPoint = PointHelpers.AngleAndDistanceToPoint(angle + 180, screenSize * 0.5, position.ToPoint());
            var endPoint1 = PointHelpers.AngleAndDistanceToPoint(angle + 90, screenSize * 0.5, midPoint);
            var endPoint2 = PointHelpers.AngleAndDistanceToPoint(angle - 90, screenSize * 0.5, midPoint);
            var topPoint = PointHelpers.AngleAndDistanceToPoint(angle, screenSize * 0.5, position.ToPoint());

            var polyline = new Polyline { Color = Color, Closed = true };
            polyline.AddVertexAt(0, endPoint2.ToPoint2d(), 0, 0, 0);
            polyline.AddVertexAt(1, endPoint1.ToPoint2d(), 0, 0, 0);
            polyline.AddVertexAt(2, topPoint.ToPoint2d(), 0, 0, 0);

            DrawAdd(polyline);

            if (!fill)
                return;

            var solid = new Solid(topPoint.ToPoint3d(), endPoint2.ToPoint3d(), endPoint1.ToPoint3d()) { Color = Color };
            DrawAdd(solid);
        }

        public void DrawX(Point3d position, int size)
        {
            double screenSize = ScreenSize(size);

            var startPoint1 = new Point3d(position.X - screenSize * 0.5, position.Y - screenSize * 0.5, 0);
            var endPoint1 = new Point3d(position.X + screenSize * 0.5, position.Y + screenSize * 0.5, 0);

            var line1 = new Line
            {
                StartPoint = startPoint1,
                EndPoint = endPoint1,
                Color = Color
            };

            DrawAdd(line1);

            var startPoint2 = new Point3d(position.X + screenSize * 0.5, position.Y - screenSize * 0.5, 0);
            var endPoint2 = new Point3d(position.X - screenSize * 0.5, position.Y + screenSize * 0.5, 0);

            var line2 = new Line
            {
                StartPoint = startPoint2,
                EndPoint = endPoint2,
                Color = Color
            };

            DrawAdd(line2);
        }

        public void DrawDot(Point3d point, int size)
        {
            double screenSize = ScreenSize(size);
            double circleSize = screenSize * 0.5;

            var polyline = new Polyline { Color = Color, Elevation = point.Z, Closed = true };

            polyline.AddVertexAt(0, new Point2d(point.X - screenSize * 0.25, point.Y), 1.0, circleSize, circleSize);
            polyline.AddVertexAt(1, new Point2d(point.X + screenSize * 0.25, point.Y), 1.0, circleSize, circleSize);

            DrawAdd(polyline);

            var vector3d = new Vector3d(0, 0, 1);
            var circle = new Circle(point, vector3d, circleSize) { Color = Color };

            DrawAdd(circle);
        }

        public void DrawPlus(Point3d position, int size)
        {
            double screenSize = ScreenSize(size);

            var startPoint1 = new Point3d(position.X - screenSize * 0.5, position.Y, 0);
            var endPoint1 = new Point3d(position.X + screenSize * 0.5, position.Y, 0);

            var line1 = new Line
            {
                StartPoint = startPoint1,
                EndPoint = endPoint1,
                Color = Color
            };

            DrawAdd(line1);

            var startPoint2 = new Point3d(position.X, position.Y - screenSize * 0.5, 0);
            var endPoint2 = new Point3d(position.X, position.Y + screenSize * 0.5, 0);

            var line2 = new Line
            {
                StartPoint = startPoint2,
                EndPoint = endPoint2,
                Color = Color
            };

            DrawAdd(line2);
        }

        public void DrawArrow(Point3d position, Angle arrowDirection, int size, bool fill = true)
        {
            var screenSize = ScreenSize(size);

            var angle = arrowDirection.Flip();

            var endPoint1 = PointHelpers.AngleAndDistanceToPoint(angle, screenSize * 1.5, position.ToPoint());
            var endPoint2 = PointHelpers.AngleAndDistanceToPoint(angle + 90, screenSize * 0.5, endPoint1);
            var endPoint3 = PointHelpers.AngleAndDistanceToPoint(angle - 90, screenSize * 0.5, endPoint1);

            var polyline = new Polyline { Color = Color, Closed = true };
            polyline.AddVertexAt(0, position.ToPoint2d(), 0, 0, 0);
            polyline.AddVertexAt(1, endPoint2.ToPoint2d(), 0, 0, 0);
            polyline.AddVertexAt(2, endPoint3.ToPoint2d(), 0, 0, 0);

            DrawAdd(polyline);

            if (!fill)
                return;

            var solid = new Solid(position, endPoint2.ToPoint3d(), endPoint3.ToPoint3d()) { Color = Color };
            DrawAdd(solid);
        }

        public void DrawArrow(Point3d position, double bearing, int size, bool fill = true) => DrawArrow(position, new Angle(bearing), size, fill);

        public void DrawText(Point3d position, string text, double textSize, Angle angle, double offsetAmount = 0.5, bool planReadability = true)
        {
            var mText = new MText();
            mText.SetDatabaseDefaults();

            mText.Rotation = planReadability ?
                    angle.GetOrdinaryAngle().ToCounterClockwise().ToRadians() :
                    angle.ToCounterClockwise().ToRadians();

            Point insPoint = PointHelpers.AngleAndDistanceToPoint(angle.GetOrdinaryAngle() - 90, TextOffset(offsetAmount), position.ToPoint());

            mText.Location = insPoint.ToPoint3d();
            mText.Color = Color;
            mText.Attachment = AttachmentPoint.BottomCenter;
            mText.TextHeight = TextSize(textSize);
            mText.Contents = text;
            DrawAdd(mText);
        }

        public void ClearGraphics()
        {
            if (_graphics == null)
                return;

            if (_graphics?.Count < 0)
                return;

            var tm = TransientManager.CurrentTransientManager;
            var intCol = new IntegerCollection();

            foreach (DBObject graphic in _graphics)
            {
                tm.EraseTransient(graphic, intCol);
                graphic.Dispose();
            }
            AcadApp.Editor.UpdateScreen();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing)
                return;

            ClearGraphics();
            _graphics?.Dispose();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}