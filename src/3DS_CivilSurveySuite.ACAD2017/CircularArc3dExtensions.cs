using System;
using System.Collections.Generic;
using Autodesk.AutoCAD.Geometry;

namespace _3DS_CivilSurveySuite.ACAD2017
{
    public static class CircularArc3dExtensions
    {
        private const double TOLERANCE = 1E-08;

        public static double GetArcBulge(this CircularArc2d circularArc)
        {
            double bulge = circularArc.EndAngle - circularArc.StartAngle;
            if (bulge < 0.0)
            {
                bulge += 2.0 * Math.PI;
            }

            if (circularArc.IsClockWise)
            {
                bulge *= -1.0;
            }

            return Math.Tan(bulge * 0.25);
        }

        public static double GetLength3D(this Curve3d circularArc)
        {
            return circularArc.GetLength(0, 1, TOLERANCE);
        }

        /// <summary>
        /// Gets the points that make up an arc at a specified mid-ordinate distance.
        /// </summary>
        /// <param name="circularArc">The <see cref="CircularArc3d"/> object.</param>
        /// <param name="midOrdinate">The mid-ordinate distance for point precision/amount.</param>
        /// <returns>A <see cref="IEnumerable{T}"/> of <see cref="Point3d"/> containing points along the arc.</returns>
        /// <remarks>Does not include the start of end points of the arc.</remarks>
        public static IEnumerable<Point3d> CurvePoints(this CircularArc3d circularArc, double midOrdinate = 0.01)
        {
            var point3dList = new List<Point3d>();
            double midOrdCalc = circularArc.Radius * 2.0 * Math.Acos((circularArc.Radius - midOrdinate) / circularArc.Radius);
            double stepDistance = circularArc.EndAngle / Math.Truncate(circularArc.GetLength3D() / midOrdCalc);
            double endAngle = circularArc.EndAngle;
            double distOnArc = stepDistance;

            while (distOnArc <= endAngle)
            {
                var point3d = circularArc.EvaluatePoint(distOnArc);
                point3dList.Add(point3d);
                distOnArc += stepDistance;
            }

            return point3dList;
        }

        //
        // public static bool SetWeighted(this CircularArc3d circArc, List<Point3d> pointList, List<double> weightedList)
        // {
        //     bool flag = false; // set initial flag
        //     if (pointList.Count > 3) // if the point list is greater than 3.
        //     {
        //         Point3d point3d1 = pointList[0]; // first point
        //         Point3d point3d2 = pointList[pointList.Count - 1]; // last point
        //         var circularArc3dList = new List<CircularArc3d>(); // define arc object
        //         int num1 = checked(pointList.Count - 2); // second to last point.
        //         int index1 = 1; // index starts from second point.
        //         while (index1 <= num1) // while index is less than num1.
        //         {
        //             circularArc3dList.Add(new CircularArc3d(point3d1, pointList[index1], point3d2)); // create a list of arc objects.
        //             index1++; // increment index.
        //         }
        //
        //         var checkList = new List<double>(); // define checklist
        //         int num2 = circularArc3dList.Count - 1; // last arc object
        //         int index2 = 0; // index 0
        //         double num3 = 0;
        //         double num4 = 0;
        //         double num5 = 0;
        //         while (index2 <= num2) // while index is less than number of arc objects.
        //         {
        //             CircularArc3d circularArc3d = circularArc3dList[index2];
        //             num3 += circularArc3d.Center.X * weightedList[index2]; // + add each arc objects center point * weight.
        //             num4 += circularArc3d.Center.Y * weightedList[index2];
        //             num5 += circularArc3d.Center.Z * weightedList[index2];
        //             checkList.Add(circularArc3d.Radius); // add the radius to the check list.
        //
        //             index2++;
        //
        //         }
        //
        //         double num6 = circularArc3dList.Count; // number of arc objects
        //         double y = num4 / num6;
        //         double x = num3 / num6;
        //         double z = num5 / num6;
        //         Point3d point3d3 = new Point3d(x, y, z); // point at the extreme center?
        //         double num7 = checkList.Average(weightedList); // calculate weighted average of the arc radiuses.
        //         Point3d SolPt1 = default;
        //         Point3d SolPt2 = default;
        //
        //         // distance intersection between first point and last point at weighted average distance of the radius.
        //         if (PointHelpers.DistanceDistanceIntersection(point3d1.ToPoint(), num7, point3d2.ToPoint(), num7, out var solpt1, out var solpt2))
        //         {
        //             // if the distance of extreme center and solution 1 is less than distance of extreme cetner and solution 2.
        //             // use respective solution point.
        //             object obj = Interaction.IIf(SupGenMath.Distance(point3d3, SolPt1) < SupGenMath.Distance(point3d3, SolPt2), (object)SolPt1, (object)SolPt2);
        //             point3d3 = obj != null ? (Point3d)obj : new Point3d(); // if solution is null, use blank point
        //             Point3d Point2 = point3d1.HalfWayTo(point3d2); //mid point between first and last.
        //             double UseAzi = SupGenMath.Azimuth(point3d3, Point2); // between extreme center and midpoint
        //             Point3d WrkPnt = SupGenMath.PntPolar(point3d3, UseAzi, num7); // calculate point
        //             pointList.SortByDist(WrkPnt); // sort the list by the new point
        //             WrkPnt.SetElevOnGrade(pointList[0], pointList[1]);
        //             circArc = new CircularArc3d(point3d1, WrkPnt, point3d2);
        //             flag = true;
        //         }
        //     }
        //
        //     return flag;
        // }
        //
        // public static void SetElevOnGrade(ref this Point3d WrkPnt, Point3d PriPnt, Point3d SecPnt)
        // {
        //     double num1 = SecPnt.Z - PriPnt.Z;
        //     double num2 = PriPnt.DistanceTo(SecPnt, false);
        //     double num3 = PriPnt.DistanceTo(WrkPnt, false);
        //     double z = PriPnt.Z + num1 * (num3 / num2);
        //     WrkPnt = new Point3d(WrkPnt.X, WrkPnt.Y, z);
        // }
        //
        // public static double BulgeFactor(this CircularArc3d CirArc)
        // {
        //     double ChkVal = Math.Tan(CirArc.EndAngle * 0.25);
        //     if (CirArc.Clockwise())
        //         ChkVal = ChkVal.Inverted();
        //     return ChkVal;
        // }
        //
        // public static bool Clockwise(this CircularArc3d CirArc) => new List<Point3d>((IEnumerable<Point3d>)new Point3d[3]
        // {
        //      CirArc.StartPoint,
        //      CirArc.ArcMidPoint(),
        //      CirArc.EndPoint
        // }).Clockwise();
        //
        // public static double Inverted(this double ChkVal) => ChkVal >= 0.0 ? (ChkVal <= 0.0 ? ChkVal : 0.0 - ChkVal) : Math.Abs(ChkVal);
        //
        // public static Point3d ArcMidPoint(this CircularArc3d CirArc) => CirArc.EvaluatePoint(CirArc.EndAngle * 0.5);
        //
        // public static bool Clockwise(this List<Point3d> PntLst)
        // {
        //     double num1 = 0.0;
        //     if (PntLst.Count > 2)
        //     {
        //         Point3d[] point3dArray = PntLst.ToArray(); // create the array
        //
        //         point3dArray[Information.UBound((Array)point3dArray)] = point3dArray[0]; // set last value to the first value?
        //
        //         uint num2 = checked((uint)(Information.UBound((Array)point3dArray) - 1)); // length of array.
        //
        //
        //
        //         uint index = 0;
        //         while (index <= num2)
        //         {
        //             num1 += (point3dArray[checked((int)((long)index + 1L))].X - point3dArray[checked((int)index)].X) * (point3dArray[checked((int)((long)index + 1L))].Y + point3dArray[checked((int)index)].Y) / 2.0;
        //             checked { ++index; }
        //         }
        //     }
        //     return num1 > 0.0;
        // }
        //
        // public static void SortByDist(this List<Point3d> OrgPts, Point3d NeaOrg)
        // {
        //     List<Point3d> point3dList = new List<Point3d>();
        //     while (OrgPts.Count > 0) // while input list count is greater than 0
        //     {
        //         Point3d point3d = new Point3d(); // new point
        //         double num1 = double.MaxValue;  // set value to maximum
        //         foreach (Point3d SecPnt in OrgPts) // loop each point in input list
        //         {
        //             double num2 = NeaOrg.DistanceTo(SecPnt, false); // distance between input point and point in input list
        //             if (num2 < num1) // if distance between input point and point in list is less than maxvalue
        //             {
        //                 point3d = SecPnt; // point is the point
        //                 num1 = num2; // maxvalue equals new distance
        //             }
        //         }
        //
        //         point3dList.Add(point3d); // add point to list
        //         OrgPts.Remove(point3d); // remove point from input list.
        //     }
        //     OrgPts = point3dList;
        // }
        //
        // public static double DistanceTo(this Point3d PriPnt, Point3d SecPnt, bool Use3D)
        // {
        //     double num1 = SecPnt.X - PriPnt.X;
        //     double num2 = SecPnt.Y - PriPnt.Y;
        //     double num3;
        //     if (!Use3D)
        //     {
        //         num3 = Math.Sqrt(num2 * num2 + num1 * num1);
        //     }
        //     else
        //     {
        //         double num4 = SecPnt.Z - PriPnt.Z;
        //         num3 = Math.Sqrt(num2 * num2 + num1 * num1 + num4 * num4);
        //     }
        //     return num3;
        // }
        //
        // /// <summary>
        // /// This is a weighted average list thing.
        // /// </summary>
        // /// <param name="ChkLst"></param>
        // /// <param name="ChkWgt"></param>
        // /// <returns></returns>
        // public static double Average(this List<double> ChkLst, List<double> ChkWgt)
        // {
        //     double num1 = 0.0;
        //     long num2 = (long)checked(ChkLst.Count - 1);
        //     long index = 0;
        //     double num3 = 0;
        //     double num4 = 0;
        //     while (index <= num2)
        //     {
        //         if (!ChkLst[(int)index].Equals(double.NaN))
        //         {
        //             num3 += ChkLst[checked((int)index)] * ChkWgt[checked((int)index)];
        //             num4 += ChkWgt[checked((int)index)];
        //         }
        //         checked { ++index; }
        //     }
        //     if (num4 > 0.0)
        //     {
        //         num1 = num3 / num4;
        //     }
        //
        //     return num1;
        // }
    }
}
