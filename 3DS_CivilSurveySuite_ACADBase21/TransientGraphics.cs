using System.Collections.Generic;
using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.GraphicsInterface;
using Autodesk.AutoCAD.Runtime;
using Polyline = Autodesk.AutoCAD.DatabaseServices.Polyline;

namespace _3DS_CivilSurveySuite_ACADBase21
{
    /// <summary>
    /// A helper class to display and manage TransientGraphics
    /// within AutoCAD.
    /// </summary>
    public static class TransientGraphics
    {
        private static DBObjectCollection s_transientGraphics;

        public static void DrawTransientTraverse(IReadOnlyList<Point2d> coordinates)
        {
            try
            {
                //TODO: add text and marker style
                if (s_transientGraphics == null)
                {
                    s_transientGraphics = new DBObjectCollection();
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
                        s_transientGraphics.Add(box1);
                        s_transientGraphics.Add(box2);
                        tm.AddTransient(box1, TransientDrawingMode.Main, 128, intCol);
                        tm.AddTransient(box2, TransientDrawingMode.Main, 128, intCol);
                    }
                    else
                    {
                        var ln = new Line(new Point3d(point.X, point.Y, 0),
                            new Point3d(coordinates[i].X, coordinates[i].Y, 0));

                        s_transientGraphics.Add(ln);
                        tm.AddTransient(ln, TransientDrawingMode.Highlight, 128, intCol);
                    }

                    i++;
                }

                AutoCADActive.ActiveDocument.TransactionManager.QueueForGraphicsFlush();
            }
            catch (Exception ex)
            {
                AutoCADActive.Editor.WriteMessage(ex.Message);
                throw;
            }
            finally
            {
                ClearTransientGraphics();
            }
        }

        public static void DrawTransientPreview(IReadOnlyList<Point2d> coordinates)
        {
            using (Transaction tr = AutoCADActive.ActiveDocument.TransactionManager.StartLockedTransaction())
            {
                ClearTransientGraphics();
                DrawTransientTraverse(coordinates);
                tr.Commit();
                //Refresh ACAD screen to display changes
                Autodesk.AutoCAD.ApplicationServices.Core.Application.UpdateScreen();
            }
        }

        public static void ClearTransientGraphics()
        {
            var tm = TransientManager.CurrentTransientManager;
            var intCol = new IntegerCollection();

            if (s_transientGraphics != null)
            {
                foreach (DBObject graphic in s_transientGraphics)
                {
                    tm.EraseTransient(graphic, intCol);
                    graphic.Dispose();
                }
            }
        }
    }
}