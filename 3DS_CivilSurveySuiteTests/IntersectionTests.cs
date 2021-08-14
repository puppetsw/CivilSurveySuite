using System;
using System.Security.Permissions;
using _3DS_CivilSurveySuite.Core;
using _3DS_CivilSurveySuite.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace _3DS_CivilSurveySuiteTests
{
    [TestClass]
    public class IntersectionTests
    {
        [TestMethod]
        public void AngleAngle_Intersect_SouthTriangle()
        {
            var angle1 = new Angle(135);
            var point1 = new Point(184, 158);

            var angle2 = new Angle(225);
            var point2 = new Point(265, 153);

            var result = PointHelpers.AngleAngleIntersection(point1, angle1, point2, angle2);

            var expectedIntersection = new Point(227, 115);

            Assert.AreEqual(expectedIntersection.X, Math.Round(result.X, 3));
            Assert.AreEqual(expectedIntersection.Y, Math.Round(result.Y, 3));
        }

        [TestMethod]
        public void AngleAngle_Intersect_NorthTriangle()
        {
            var angle1 = new Angle(45);
            var point1 = new Point(184, 158);

            var angle2 = new Angle(315);
            var point2 = new Point(265, 153);

            var result = PointHelpers.AngleAngleIntersection(point1, angle1, point2, angle2);

            var expectedIntersection = new Point(222, 196);

            Assert.AreEqual(expectedIntersection.X, Math.Round(result.X, 3));
            Assert.AreEqual(expectedIntersection.Y, Math.Round(result.Y, 3));
        }

        [TestMethod]
        public void AngleAngle_Intersect_North_NegativeTriangle()
        {

        }

        [TestMethod]
        public void AngleAngle_Intersect_South_NegativeTriangle()
        {

        }

        [TestMethod]
        public void AngleAngle_Intersect_NoIntersection_ShouldReturnOrigin()
        {
            var point1 = new Point(0, 0);
            var angle1 = new Angle(0);

            var point2 = new Point(0, 30);
            var angle2 = new Angle(180);

            var result = PointHelpers.AngleAngleIntersection(point1, angle1, point2, angle2);

            Assert.AreEqual(Point.Origin, result);
        }

        [TestMethod]
        public void DistanceDistance_Intersect_ShouldReturnTrue()
        {
            var point1 = new Point(360, 150);
            var dist1 = 50;

            var point2 = new Point(440, 150);
            var dist2 = 50;

            var expectedPoint1 = new Point(400, 180);
            var expectedPoint2 = new Point(400, 120);

            var resultBool = PointHelpers.DistanceDistanceIntersection(point1, dist1, point2, dist2, out Point result1, out Point result2);

            Assert.IsTrue(resultBool);

            Assert.AreEqual(expectedPoint1, result1);
            Assert.AreEqual(expectedPoint2, result2);
        }

        [TestMethod]
        public void DistanceDistance_NoIntersect_ShouldReturnFalse()
        {
            var point1 = new Point(360, 150);
            var dist1 = 10;

            var point2 = new Point(440, 150);
            var dist2 = 10;

            var expectedPoint1 = new Point(400, 180);
            var expectedPoint2 = new Point(400, 120);

            var resultBool = PointHelpers.DistanceDistanceIntersection(point1, dist1, point2, dist2, out Point result1, out Point result2);

            Assert.IsFalse(resultBool);

            //Assert.AreEqual(expectedPoint1, result1);
            //Assert.AreEqual(expectedPoint2, result2);
        }
    }
}
