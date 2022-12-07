using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using _3DS_CivilSurveySuite.Shared.Helpers;
using _3DS_CivilSurveySuite.Shared.Models;
using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.GraphicsInterface;
using Polyline = Autodesk.AutoCAD.DatabaseServices.Polyline;

namespace _3DS_CivilSurveySuite.ACAD
{
    public sealed class Drawables : List<Drawable> { }

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
    ///   graphics.Undo(); // If you need to undo last drawn.
    /// }
    /// finally
    /// {
    ///   graphics.Dispose();
    /// }
    /// </code>
    /// </example>
    public sealed class TransientGraphics : IDisposable
    {
        private readonly List<TransientDrawable> _drawables;

        private const string DASHED_LINE_TYPE = "DASHED";

        public Color Color { get; } = Color.FromColorIndex(ColorMethod.ByPen, Settings.TransientColorIndex);

        public TransientGraphics()
        {
            _drawables = new List<TransientDrawable>();
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

            if (text <= 0) // don't allow value to be less than or equal to 0.
                return 1;

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

        private void AddTransientDrawable(TransientDrawable drawable, TransientDrawingMode mode = TransientDrawingMode.Main)
        {
            _drawables.Add(drawable);

            foreach (Drawable entity in drawable)
                TransientManager.CurrentTransientManager.AddTransient(entity, mode, 0, new IntegerCollection());
        }

        public void DrawLines(IReadOnlyList<Point2d> coordinates, TransientDrawingMode mode = TransientDrawingMode.Main) => DrawLines(coordinates.ToListOfPoint3d(), mode);

        public void DrawLines(IReadOnlyList<Point3d> coordinates, TransientDrawingMode mode = TransientDrawingMode.Main)
        {
            var drawables = new TransientDrawable();

            // Start a count for the next coordinate in the collection.
            var nextCoord = 1;
            // Draw the coord lines
            foreach (Point3d point in coordinates)
            {
                if (nextCoord == coordinates.Count)
                    break;

                var startPoint = new Point3d(point.X, point.Y, 0);
                var endPoint = new Point3d(coordinates[nextCoord].X, coordinates[nextCoord].Y, 0);
                drawables.Add(DrawLine(startPoint, endPoint, true));
                nextCoord++;
            }

            AddTransientDrawable(drawables, mode);
        }

        public void DrawLine(Point2d point1, Point2d point2, TransientDrawingMode mode = TransientDrawingMode.Main) => DrawLine(point1.ToPoint3d(), point2.ToPoint3d(), mode);

        public void DrawLine(Line line, TransientDrawingMode mode = TransientDrawingMode.Main) => DrawLine(line.StartPoint, line.EndPoint, mode);

        public void DrawLine(Point3d point1, Point3d point2, TransientDrawingMode mode = TransientDrawingMode.Main, bool useDashedLine = true)
        {
            var line = new Line(point1, point2) { Color = Color };

            if (useDashedLine)
                SetLineType(line);

            var drawable = new TransientDrawable { line };
            AddTransientDrawable(drawable, mode);
        }

        private Drawable DrawLine(Point3d point1, Point3d point2, bool useDashedLine = true)
        {
            var line = new Line(point1, point2) { Color = Color };

            if (useDashedLine)
                SetLineType(line);

            return line;
        }

        public void DrawLine(Curve curve, TransientDrawingMode mode = TransientDrawingMode.Main, bool useDashedLine = false)
        {
            if (useDashedLine)
                SetLineType(curve);

            var drawable = new TransientDrawable { curve };
            AddTransientDrawable(drawable, mode);
        }

        public void DrawSquare(Point3d position, double squareSize)
        {
            var topLeft = new Point3d(position.X - squareSize / 2, position.Y - squareSize / 2, 0);
            var topRight = new Point3d(topLeft.X + squareSize, topLeft.Y, 0);
            var bottomRight = new Point3d(topRight.X, topRight.Y + squareSize, 0);
            var bottomLeft = new Point3d(bottomRight.X - squareSize, bottomRight.Y, 0);

            var l1 = DrawLine(topLeft, topRight, true);
            var l2 = DrawLine(topRight, bottomRight, true);
            var l3 = DrawLine(bottomRight, bottomLeft, true);
            var l4 = DrawLine(bottomLeft, topLeft, true);

            var drawable = new TransientDrawable { l1, l2, l3, l4 };
            AddTransientDrawable(drawable);
        }

        public void DrawRectangle(Point3d position, double width, double height)
        {
            var topLeft = new Point3d(position.X - width / 2, position.Y - height / 2, 0);
            var topRight = new Point3d(topLeft.X + width, topLeft.Y, 0);
            var bottomRight = new Point3d(topRight.X, topRight.Y + height, 0);
            var bottomLeft = new Point3d(bottomRight.X - width, bottomRight.Y, 0);

            var l1 = DrawLine(topLeft, topRight, true);
            var l2 = DrawLine(topRight, bottomRight, true);
            var l3 = DrawLine(bottomRight, bottomLeft, true);
            var l4 = DrawLine(bottomLeft, topLeft, true);

            var drawable = new TransientDrawable { l1, l2, l3, l4 };
            AddTransientDrawable(drawable);
        }

        public void DrawCircle(Point3d position, double circleSize = 0.5)
        {
            var circle = new Circle(position, Vector3d.ZAxis, circleSize) { Color = Color };

            SetLineType(circle);

            var drawable = new TransientDrawable { circle };
            AddTransientDrawable(drawable);
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

            var drawable = new TransientDrawable();

            if (fill)
            {
                var solid = new Solid(point1, point2, point4, point3) { Color = Color };
                drawable.Add(solid);
            }

            drawable.Add(polyline);

            AddTransientDrawable(drawable);
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

            var drawable = new TransientDrawable { polyline };

            if (fill)
            {
                var solid = new Solid(topPoint.ToPoint3d(), endPoint2.ToPoint3d(), endPoint1.ToPoint3d()) { Color = Color };
                drawable.Add(solid);
            }

            AddTransientDrawable(drawable);
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

            var drawable = new TransientDrawable { line1 };

            var startPoint2 = new Point3d(position.X + screenSize * 0.5, position.Y - screenSize * 0.5, 0);
            var endPoint2 = new Point3d(position.X - screenSize * 0.5, position.Y + screenSize * 0.5, 0);

            var line2 = new Line
            {
                StartPoint = startPoint2,
                EndPoint = endPoint2,
                Color = Color
            };

            drawable.Add(line2);
            AddTransientDrawable(drawable);
        }

        public void DrawDot(Point3d point, int size)
        {
            double screenSize = ScreenSize(size);
            double circleSize = screenSize * 0.5;

            var polyline = new Polyline { Color = Color, Elevation = point.Z, Closed = true };

            polyline.AddVertexAt(0, new Point2d(point.X - screenSize * 0.25, point.Y), 1.0, circleSize, circleSize);
            polyline.AddVertexAt(1, new Point2d(point.X + screenSize * 0.25, point.Y), 1.0, circleSize, circleSize);

            var drawable = new TransientDrawable { polyline };

            var vector3d = new Vector3d(0, 0, 1);
            var circle = new Circle(point, vector3d, circleSize) { Color = Color };

            drawable.Add(circle);
            AddTransientDrawable(drawable);
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

            var drawable = new TransientDrawable { line1 };

            var startPoint2 = new Point3d(position.X, position.Y - screenSize * 0.5, 0);
            var endPoint2 = new Point3d(position.X, position.Y + screenSize * 0.5, 0);

            var line2 = new Line
            {
                StartPoint = startPoint2,
                EndPoint = endPoint2,
                Color = Color
            };

            drawable.Add(line2);
            AddTransientDrawable(drawable);
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

            var drawable = new TransientDrawable { polyline };

            if (fill)
            {
                var solid = new Solid(position, endPoint2.ToPoint3d(), endPoint3.ToPoint3d()) { Color = Color };
                drawable.Add(solid);
            }

            AddTransientDrawable(drawable);
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

            var textStyleId = GetTextStyleId();

            if (textStyleId != null)
                mText.TextStyleId = textStyleId.Value;
            else
                AcadApp.Logger.Warn("Unable to get or create transient graphics text style. Using default.");

            var drawable = new TransientDrawable { mText };
            AddTransientDrawable(drawable);
        }

        private const string TEXT_STYLE_NAME = "CSS_Transient_Text";

        private static ObjectId? GetTextStyleId()
        {
            using (var tr = AcadApp.StartLockedTransaction())
            {
                var tsTable = (TextStyleTable)tr.GetObject(AcadApp.ActiveDatabase.TextStyleTableId, OpenMode.ForRead);

                if (tsTable == null)
                {
                    tr.Commit();
                    return null;
                }

                ObjectId textStyleId;

                if (!tsTable.Has(TEXT_STYLE_NAME))
                {
                    tsTable.UpgradeOpen();
                    var textStyle = new TextStyleTableRecord
                    {
                        FileName = "iso.shx",
                        Name = TEXT_STYLE_NAME,
                    };
                    textStyleId = tsTable.Add(textStyle);
                    tr.AddNewlyCreatedDBObject(textStyle, true);
                }
                else
                {
                    textStyleId = tsTable[TEXT_STYLE_NAME];
                }

                tr.Commit();
                return textStyleId;
            }
        }

        public void ClearGraphics()
        {
            if (_drawables == null)
                return;

            if (_drawables.Count < 0)
                return;

            var tm = TransientManager.CurrentTransientManager;
            var intCol = new IntegerCollection();

            foreach (TransientDrawable transientDrawable in _drawables)
            {
                foreach (Drawable drawable in transientDrawable)
                {
                    tm.EraseTransient(drawable, intCol);
                    drawable.Dispose();
                }
            }

            _drawables.Clear();

            AcadApp.Editor.UpdateScreen();
        }

        public void Undo()
        {
            if (_drawables == null)
                return;

            if (_drawables.Count <= 0)
                return;

            var lastIndex = _drawables.Count - 1;
            var lastDrawable = _drawables[lastIndex];

            var intCol = new IntegerCollection();

            foreach (Drawable drawable in lastDrawable)
                TransientManager.CurrentTransientManager.EraseTransient(drawable, intCol);

            _drawables.RemoveAt(lastIndex);
        }

        private void Dispose(bool disposing)
        {
            if (!disposing)
            {
                return;
            }

            ClearGraphics();
        }

        public void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        /// Draws a chainage between two <see cref="Point3d"/> entities.
        /// </summary>
        /// <param name="graphics">The <see cref="TransientGraphics"/> object to draw the chainage.</param>
        /// <param name="firstPoint"></param>
        /// <param name="secondPoint"></param>
        /// <param name="interval"></param>
        /// <param name="textSize"></param>
        public void DrawChainage(Point3d firstPoint, Point3d secondPoint, int interval = 10, double textSize = 1.0)
        {
            var distance = PointHelpers.GetDistanceBetweenPoints(firstPoint.ToPoint(), secondPoint.ToPoint());
            var angle = AngleHelpers.GetAngleBetweenPoints(firstPoint.ToPoint(), secondPoint.ToPoint());

            var drawables = new TransientDrawable();

            var intervalStep = 0;
            while (intervalStep < distance)
            {
                var point = PointHelpers.AngleAndDistanceToPoint(angle, intervalStep, firstPoint.ToPoint());

                drawables.Add(DrawTick(point.ToPoint3d(), angle));
                drawables.Add(DrawChainageText(point.ToPoint3d(), intervalStep.ToString(), textSize, angle, planReadability: false));
                intervalStep += interval;
            }
            //draw last
            drawables.Add(DrawTick(secondPoint, angle));
            drawables.Add(DrawChainageText(secondPoint, Math.Round(distance, 3).ToString(CultureInfo.InvariantCulture), textSize, angle, planReadability: false));
            AddTransientDrawable(drawables);
        }

        private Drawable DrawChainageText(Point3d position, string text, double textSize, Angle angle, double offsetAmount = 0.5, bool planReadability = true)
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

            return mText;
        }

        private Drawable DrawTick(Point3d tickPoint, Angle drawAngle, double tickLength = 1.0)
        {
            var point1 = PointHelpers.AngleAndDistanceToPoint(drawAngle + 90, tickLength, tickPoint.ToPoint());
            var point2 = PointHelpers.AngleAndDistanceToPoint(drawAngle - 90, tickLength, tickPoint.ToPoint());
            return DrawLine(point1.ToPoint3d(), point2.ToPoint3d(), useDashedLine: false);
        }
    }
}
