using System;
using System.Reflection;
using _3DS_CivilSurveySuite.ACAD;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.Civil;
using Autodesk.Civil.DatabaseServices;
using Autodesk.Civil.DatabaseServices.Styles;
using DBObject = Autodesk.AutoCAD.DatabaseServices.DBObject;

namespace _3DS_CivilSurveySuite.CIVIL
{
    public static class FeatureLineUtils
    {
        public static ObjectId CreateFeatureLineFromPoly(this Site site, Polyline poly, FeatureLineStyle style)
        {
            object acadObject = site.AcadObject;
            object[] args =
            {
                poly.AcadObject,
                style.AcadObject
            };

            object target = acadObject.GetType().InvokeMember("FeatureLines", BindingFlags.GetProperty, null, acadObject, null);
            return DBObject.FromAcadObject(target.GetType().InvokeMember("AddFromPolylineEx", BindingFlags.InvokeMethod, null, target, args));
        }

        public static void FlattenFeatureLine(FeatureLine featureLine)
        {
            if (featureLine == null)
            {
                throw new ArgumentNullException(nameof(featureLine));
            }

            var pointCount = featureLine.GetPoints(FeatureLinePointType.AllPoints);

            for (int i = 0; i < pointCount.Count; i++)
            {
                featureLine.SetPointElevation(i, 0);
            }
        }

        public static bool ConvertToPolyline3d(this FeatureLine featureLine, Transaction tr, out Polyline3d polyline3d, double midOrdinate = 0.01)
        {
            // If the mid-ordinate distance is 0 set it to the default.
            if (midOrdinate <= 0)
            {
                midOrdinate = 0.01;
            }

            Polyline polyline = featureLine.BaseCurve2d();
            if (polyline.HasBulges)
            {
                featureLine.UpgradeOpen();
                for (int i = 0; i < polyline.EndParam; i++)
                {
                    var radiusPoint = polyline.SegmentRadiusPoint(i);
                    if (!radiusPoint.IsArc())
                    {
                        continue;
                    }

                    double stepDistance = CircularArcExtensions.ArcLengthByMidOrdinate(Math.Abs(radiusPoint.Radius), midOrdinate);
                    double distanceAtParameter1 = polyline.GetDistanceAtParameter(i);
                    double distanceAtParameter2 = polyline.GetDistanceAtParameter(i + 1);
                    while ((distanceAtParameter1 += stepDistance) < distanceAtParameter2)
                    {
                        Point3d pointAtDist = featureLine.GetPointAtDist(distanceAtParameter1);
                        featureLine.InsertElevationPoint(pointAtDist);
                    }
                }
            }
            else
            {
                polyline3d = null;
                return false;
            }

            Point3dCollection points = featureLine.GetPoints(FeatureLinePointType.AllPoints);

            polyline3d = new Polyline3d(Poly3dType.SimplePoly, points, false);
            polyline3d.Layer = featureLine.Layer;
            return true;
        }

        private static Polyline BaseCurve2d(this FeatureLine featureLine)
        {
            Polyline baseCurve = featureLine.BaseCurve as Polyline;
            if (baseCurve != null)
            {
                baseCurve.Elevation = 0.0;
                return baseCurve;
            }

            object acadObject = featureLine.AcadObject;
            object[] args = { 1 };
            double[] numArray = (double[]) acadObject.GetType().InvokeMember("GetPoints", BindingFlags.InvokeMethod, null, acadObject, args);

            Polyline polyline = new Polyline();

            int vertexIndex = 0;
            int polyIndex = 0;

            while (vertexIndex < numArray.Length)
            {
                Point2d pt = new Point2d(numArray[vertexIndex], numArray[vertexIndex + 1]);
                Point3d point3d = new Point3d(numArray[vertexIndex], numArray[vertexIndex + 1], numArray[vertexIndex + 2]);
                args[0] = point3d.ToArray();
                double bulge = (double) acadObject.GetType().InvokeMember("GetBulgeAtPoint", BindingFlags.InvokeMethod, null, acadObject, args);
                polyline.AddVertexAt(polyIndex, pt, bulge, 0.0, 0.0);
                vertexIndex += 3;
                polyIndex++;
            }

            polyline.Elevation = 0.0;
            return polyline;
        }
    }
}
