using System;
using _3DS_CivilSurveySuite.Shared.Helpers;
using _3DS_CivilSurveySuite.Shared.Models;
using NUnit.Framework;

namespace _3DS_CivilSurveySuiteTests
{
    [TestFixture]
    public class IntersectionTests
    {
        [Test]
        public void AngleAngle_Intersect_SouthTriangle()
        {
            var angle1 = new Angle(135);
            var point1 = new Point(184, 158);

            var angle2 = new Angle(225);
            var point2 = new Point(265, 153);

            PointHelpers.AngleAngleIntersection(point1, angle1, point2, angle2, out Point result);

            var expectedIntersection = new Point(227, 115);

            Assert.AreEqual(expectedIntersection.X, Math.Round(result.X, 3));
            Assert.AreEqual(expectedIntersection.Y, Math.Round(result.Y, 3));
        }

        [Test]
        public void AngleAngle_Intersect_NorthTriangle()
        {
            var angle1 = new Angle(45);
            var point1 = new Point(184, 158);

            var angle2 = new Angle(315);
            var point2 = new Point(265, 153);

            PointHelpers.AngleAngleIntersection(point1, angle1, point2, angle2, out Point result);

            var expectedIntersection = new Point(222, 196);

            Assert.AreEqual(expectedIntersection.X, Math.Round(result.X, 3));
            Assert.AreEqual(expectedIntersection.Y, Math.Round(result.Y, 3));
        }

        [Test]
        public void AngleAngle_Intersect_NoIntersection_ShouldReturnOrigin()
        {
            var point1 = new Point(0, 0);
            var angle1 = new Angle(0);

            var point2 = new Point(0, 30);
            var angle2 = new Angle(180);

            PointHelpers.AngleAngleIntersection(point1, angle1, point2, angle2, out Point result);

            Assert.AreEqual(Point.Origin, result);
        }

        [Test]
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

        [Test]
        public void DistanceDistance_NoIntersect_ShouldReturnFalse()
        {
            var point1 = new Point(360, 150);
            var dist1 = 10;

            var point2 = new Point(440, 150);
            var dist2 = 10;

            var resultBool = PointHelpers.DistanceDistanceIntersection(point1, dist1, point2, dist2, out _, out _);

            Assert.IsFalse(resultBool);
        }

        [Test]
        public void BearingDistance_IntersectTwo_ShouldReturnTrue()
        {
            var point1 = new Point(0, 0);
            var angle = new Angle(0);

            var point2 = new Point(10, 50);
            var distance = 20;

            var expectedPoint1 = new Point(0, 67.32051);
            var expectedPoint2 = new Point(0, 32.67949);

            var resultBool = PointHelpers.AngleDistanceIntersection(point1, angle, point2, distance, out Point result1, out Point result2);
            Assert.IsTrue(resultBool);
            Assert.AreEqual(expectedPoint1, result1);
            Assert.AreEqual(expectedPoint2, result2);
        }

        [Test]
        public void BearingDistance_IntersectNone_ShouldReturnFalse()
        {
            var point1 = new Point(0, 0);
            var angle = new Angle(45);

            var point2 = new Point(100, 0);
            var distance = 20;

            var expectedPoint1 = Point.Origin;
            var expectedPoint2 = Point.Origin;

            var resultBool = PointHelpers.AngleDistanceIntersection(point1, angle, point2, distance, out Point result1, out Point result2);
            Assert.IsFalse(resultBool);
            Assert.AreEqual(expectedPoint1, result1);
            Assert.AreEqual(expectedPoint2, result2);
        }


        [Test]
        public void Perpendicular_Intersection_ShouldBeTrue()
        {
            var pointA = new Point(0, 0);
            var pointB = new Point(0, 100);
            var pointC = new Point(10, 50);

            var expectedPoint = new Point(0, 50);

            PointHelpers.PerpendicularIntersection(pointA, pointB, pointC, out Point intersectionPoint);

            Assert.AreEqual(expectedPoint, intersectionPoint);

        }


        [Test]
        public void FourPoint_Intersection_ShouldReturnTrue()
        {
            var pointA = new Point(218.26792775, 127.56814117);
            var pointB = new Point(272.08656829, 220.3042242);
            var pointC = new Point(327.23079203, 125.71341979);
            var pointD = new Point(168.95626798, 236.99671873);

            var expectedIntersection = new Point(249.08015611, 180.66136945);

            PointHelpers.FourPointIntersection(pointA, pointB, pointC, pointD, out Point resultIntersection);

            Assert.AreEqual(expectedIntersection, resultIntersection);
        }

        [Test]
        public void FourPoint_Intersection_ParallelLines_ShouldReturnFalse()
        {
            var pointA = new Point(0, 0);
            var pointB = new Point(0, 100);
            var pointC = new Point(10, 100);
            var pointD = new Point(10, 100);

            var result = PointHelpers.FourPointIntersection(pointA, pointB, pointC, pointD, out _);

            Assert.IsFalse(result);
        }




    }
}