// Copyright Scott Whitney. All Rights Reserved.
// Reproduction or transmission in whole or in part, any form or by any
// means, electronic, mechanical or otherwise, is prohibited without the
// prior written consent of the copyright owner.

using System;
using System.Collections.Generic;
using _3DS_CivilSurveySuite.Core;
using _3DS_CivilSurveySuite.Model;
using Autodesk.AutoCAD.ApplicationServices.Core;
using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.GraphicsInterface;
using Polyline = Autodesk.AutoCAD.DatabaseServices.Polyline;

namespace _3DS_CivilSurveySuite_ACADBase21
{
    /// <summary>
    /// A helper class to display and manage TransientGraphics
    /// within AutoCAD.
    /// </summary>
    public class TransientGraphics : IDisposable
    {
        private readonly DBObjectCollection _graphics;

        public Color Color { get; set; } = Color.FromColorIndex(ColorMethod.ByPen, 2);

        public TransientDrawingMode DrawingMode { get; set; }

        public TransientGraphics()
        {
            _graphics = new DBObjectCollection();
            DrawingMode = TransientDrawingMode.Main;
        }

        public TransientGraphics(TransientDrawingMode mode)
        {
            _graphics = new DBObjectCollection();
            DrawingMode = mode;
        }

        private static double ScreenSize(int numPix)
        {
            object systemVariable = Application.GetSystemVariable(SystemVariables.SCREENSIZE);
            object viewSize = Application.GetSystemVariable(SystemVariables.VIEWSIZE);

            Point2d screenSize = (Point2d)systemVariable;
            
            return Convert.ToDouble(viewSize) / screenSize.Y * numPix;
        }

        private void DrawAdd(DBObject entity)
        {
            _graphics.Add(entity);
            TransientManager.CurrentTransientManager.AddTransient(entity, DrawingMode, 0, new IntegerCollection());
        }

        public void DrawLines(IReadOnlyList<Point2d> coordinates) => DrawLines(coordinates.ToListOfPoint3d());

        public void DrawLines(IReadOnlyList<Point3d> coordinates)
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
                DrawLine(startPoint, endPoint);
                nextCoord++;
            }
        }

        public void DrawLine(Point2d point1, Point2d point2) => DrawLine(point1.ToPoint3d(), point2.ToPoint3d());

        public void DrawLine(Point3d point1, Point3d point2)
        {
            var line = new Line(point1, point2) { Color = Color };
            DrawAdd(line);
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

            var midPoint = MathHelpers.AngleAndDistanceToPoint(angle + 180, screenSize * 0.5, position.ToPoint());
            var endPoint1 = MathHelpers.AngleAndDistanceToPoint(angle + 90, screenSize * 0.5, midPoint);
            var endPoint2 = MathHelpers.AngleAndDistanceToPoint(angle - 90, screenSize * 0.5, midPoint);
            var topPoint = MathHelpers.AngleAndDistanceToPoint(angle, screenSize * 0.5, position.ToPoint());

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

            var endPoint1 = MathHelpers.AngleAndDistanceToPoint(angle, screenSize * 1.5, position.ToPoint());
            var endPoint2 = MathHelpers.AngleAndDistanceToPoint(angle + 90, screenSize * 0.5, endPoint1);
            var endPoint3 = MathHelpers.AngleAndDistanceToPoint(angle - 90, screenSize * 0.5, endPoint1);

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

        public void DrawText(Point3d position, string text, double textSize, Angle angle)
        {
            var mText = new MText();
            mText.SetDatabaseDefaults();
            mText.Rotation = angle.ToRadians();
            mText.Location = position;
            mText.Attachment = AttachmentPoint.MiddleLeft;
            mText.TextHeight = textSize;
            mText.SetContentsRtf(text);
            DrawAdd(mText);
        }

        public void ClearGraphics()
        {
            if (_graphics == null) 
                return;

            var tm = TransientManager.CurrentTransientManager;
            var intCol = new IntegerCollection();

            foreach (DBObject graphic in _graphics)
            {
                tm.EraseTransient(graphic, intCol);
                graphic.Dispose();
            }
        }

        public void Dispose()
        {
            ClearGraphics();
            _graphics?.Dispose();
        }
    }
}