using System;
using System.Reflection;
using _3DS_CivilSurveySuite.ACAD2017;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.Civil;
using Autodesk.Civil.DatabaseServices;
using Autodesk.Civil.DatabaseServices.Styles;

namespace _3DS_CivilSurveySuite.C3D2017
{
    public static class FeatureLineUtils
    {
        public static ObjectId AddFeatureFromPoly(this Site site, Polyline poly, FeatureLineStyle style)
        {
            object acadObject = site.AcadObject;
            object[] args =
            {
                poly.AcadObject,
                style.AcadObject
            };

            object target = acadObject.GetType().InvokeMember("FeatureLines", BindingFlags.GetProperty, null, acadObject, null);
            return Autodesk.AutoCAD.DatabaseServices.DBObject.FromAcadObject(target.GetType().InvokeMember("AddFromPolylineEx", BindingFlags.InvokeMethod, null, target, args));
        }

        public static void FlattenFeatureLine(FeatureLine featureLine)
        {
            if (featureLine == null)
            {
                throw new ArgumentNullException(nameof(featureLine));
            }

            var pointCount = featureLine.GetPoints(FeatureLinePointType.AllPoints);

            for (int i = 0; i < pointCount.Count - 1; i++)
            {
                featureLine.SetPointElevation(i, 0);
            }
        }

        public static bool TryConvertTo(this FeatureLine featureLine, Transaction tr, out Polyline3d polyline3d, double midOrdinate = 0.01)
        {
            Point3dCollection points;
            Polyline polyline = featureLine.BaseCurve2d();
            if (polyline.HasBulges)
            {
                featureLine.UpgradeOpen();
                for (int i = 0; i < polyline.EndParam; i++)
                {
                    var radiusPoint = polyline.SegmentRadiusPoint(i);
                    if (radiusPoint.IsArc())
                    {
                        double num = CircularArcExtensions.ArcLengthByMidOrdinate(Math.Abs(radiusPoint.Radius), midOrdinate);
                        double distanceAtParameter1 = polyline.GetDistanceAtParameter(i);
                        double distanceAtParameter2 = polyline.GetDistanceAtParameter(i + 1);
                        while ((distanceAtParameter1 += num) < distanceAtParameter2)
                        {
                            polyline.GetPointAtDist(distanceAtParameter1);
                            Point3d pointAtDist = featureLine.GetPointAtDist(distanceAtParameter1);
                            featureLine.InsertElevationPoint(pointAtDist);
                        }
                    }
                }
            }
            else
            {
                polyline3d = null;
                return false;
            }
            points = featureLine.GetPoints(FeatureLinePointType.AllPoints);

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
            Polyline polyline = new Polyline();
            object[] args = { 1 };
            double[] numArray = (double[]) acadObject.GetType().InvokeMember("GetPoints", BindingFlags.InvokeMethod, null, acadObject, args);
            int index1 = 0;
            int index2 = 0;
            while (index1 < numArray.Length)
            {
                Point2d pt = new Point2d(numArray[index1], numArray[index1 + 1]);
                Point3d point3d = new Point3d(numArray[index1], numArray[index1 + 1], numArray[index1 + 2]);
                args[0] = point3d.ToArray();
                double bulge = (double) acadObject.GetType().InvokeMember("GetBulgeAtPoint", BindingFlags.InvokeMethod, null, acadObject, args);
                polyline.AddVertexAt(index2, pt, bulge, 0.0, 0.0);
                index1 += 3;
                index2++;
            }
            polyline.Elevation = 0.0;
            return polyline;
        }
    }
}
