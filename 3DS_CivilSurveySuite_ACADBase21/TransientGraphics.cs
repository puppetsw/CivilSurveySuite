using System;
using System.Collections.Generic;
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
        private DBObjectCollection _transientGraphics;

        public Color Color { get; set; } = Color.FromColorIndex(ColorMethod.ByPen, 4);

        public void DrawTransientTraverse(IReadOnlyList<Point2d> coordinates)
        {
            try
            {
                //TODO: add text and marker style
                if (_transientGraphics == null)
                {
                    _transientGraphics = new DBObjectCollection();
                }

                var i = 1; //Start an index for the next coordinate in the collection.
                //draw the coord lines
                foreach (Point2d point in coordinates)
                {
                    var tm = TransientManager.CurrentTransientManager;
                    var intCol = new IntegerCollection();

                    if (coordinates.Count == i) // if the next coordinate index is the same as the collection count, end.
                    {
                        //draw boxes on last and first points
                        Polyline box1 = Polylines.Square(point, 0.5);
                        Polyline box2 = Polylines.Square(coordinates[0], 0.5);
                        box1.Color = Color.FromColorIndex(ColorMethod.None, 2);
                        box2.Color = Color.FromColorIndex(ColorMethod.None, 2);
                        _transientGraphics.Add(box1);
                        _transientGraphics.Add(box2);
                        tm.AddTransient(box1, TransientDrawingMode.Main, 128, intCol);
                        tm.AddTransient(box2, TransientDrawingMode.Main, 128, intCol);
                    }
                    else
                    {
                        var ln = new Line(new Point3d(point.X, point.Y, 0),
                            new Point3d(coordinates[i].X, coordinates[i].Y, 0));

                        _transientGraphics.Add(ln);
                        tm.AddTransient(ln, TransientDrawingMode.Highlight, 128, intCol);
                    }

                    i++;
                }

                AutoCADActive.ActiveDocument.TransactionManager.QueueForGraphicsFlush();
            }
            catch (Exception e)
            {
                AutoCADActive.Editor.WriteMessage(e.Message);
                throw;
            }
        }

        public void DrawTransientPoint(Point3d point, double pointSize = 0.1)
        {
            try
            {
                if (_transientGraphics == null)
                {
                    _transientGraphics = new DBObjectCollection();
                }

                var tm = TransientManager.CurrentTransientManager;
                var intCol = new IntegerCollection();

                var marker = new Circle(point, Vector3d.ZAxis, pointSize)
                {
                    Color = Color
                };
                _transientGraphics.Add(marker);

                tm.AddTransient(marker, TransientDrawingMode.Highlight, 128, intCol);
            }
            catch (Exception e)
            {
                AutoCADActive.Editor.WriteMessage(e.Message);
            }
        }

        public void DrawTransientLine(Point3d point1, Point3d point2)
        {
            try
            {
                if (_transientGraphics == null)
                {
                    _transientGraphics = new DBObjectCollection();
                }

                var tm = TransientManager.CurrentTransientManager;
                var intCol = new IntegerCollection();

                var ln = new Line(point1, point2)
                {
                    Color = Color
                };

                _transientGraphics.Add(ln);
                tm.AddTransient(ln, TransientDrawingMode.Highlight, 128, intCol);
            }
            catch (Exception e)
            {
                AutoCADActive.Editor.WriteMessage(e.Message);
            }
        }

        public void ClearTransientGraphics()
        {
            if (_transientGraphics != null)
            {
                var tm = TransientManager.CurrentTransientManager;
                var intCol = new IntegerCollection();

                foreach (DBObject graphic in _transientGraphics)
                {
                    tm.EraseTransient(graphic, intCol);
                    graphic.Dispose();
                }
            }
        }

        /// <inheritdoc />
        public void Dispose()
        {
            ClearTransientGraphics();
            _transientGraphics?.Dispose();
        }
    }
}