using System;
using System.Collections.Generic;
using _3DS_CivilSurveySuite.Core;
using _3DS_CivilSurveySuite.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace _3DS_CivilSurveySuiteTests
{
    [TestClass]
    public class MathHelpersTests
    {
        [TestMethod]
        public void ConvertLinkToMetersTest()
        {
            var linkValue = 100;
            var expectedValue = 20.1168;

            double result = MathHelpers.ConvertLinkToMeters(linkValue);
            Assert.AreEqual(expectedValue, result);
        }

        [TestMethod]
        public void ConvertFeetToMetersTest_FeetOnly()
        {
            var feetValue = 100;
            var expectedValue = 30.48;

            double result = MathHelpers.ConvertFeetToMeters(feetValue);
            Assert.AreEqual(expectedValue, result);
        }

        [TestMethod]
        public void ConvertFeetToMetersTest_FeetAndInches()
        {
            var feetAndInchesValue = 100.10;
            var expectedValue = 30.734;

            double result = MathHelpers.ConvertFeetToMeters(feetAndInchesValue);
            Assert.AreEqual(expectedValue, result);
        }

        [TestMethod]
        public void ConvertFeetToMetersTest_FeetAndInches_InchesLessThan10()
        {
            var feetAndInchesValue = 100.09;
            var expectedValue = 30.7086;

            double result = MathHelpers.ConvertFeetToMeters(feetAndInchesValue);
            Assert.AreEqual(expectedValue, result);
        }

        [TestMethod]
        public void AngleToDecimalDegreesTest()
        {
            var angle = new Angle { Degrees = 57, Minutes = 12, Seconds = 34 };

            const double expectedResult = 57.2094;
            double result = MathHelpers.AngleToDecimalDegrees(angle);

            Assert.AreEqual(expectedResult, result, 4);
        }

        [TestMethod]
        public void AngleToDecimalDegreesTest_ShouldReturnZero()
        {
            Angle angle = null;

            const double expectedResult = 0;
            // ReSharper disable once ExpressionIsAlwaysNull
            double result = MathHelpers.AngleToDecimalDegrees(angle);

            Assert.AreEqual(expectedResult, result);
        }

        [TestMethod]
        public void DecimalDegreesToRadiansTest()
        {
            const double expectedResult = 0.9985;
            const double decimalDegrees = 57.2094;

            double result = Math.Round(MathHelpers.DecimalDegreesToRadians(decimalDegrees), 4);

            Assert.AreEqual(expectedResult, result);
        }

        [TestMethod]
        public void DecimalDegreesToAngleTest()
        {
            var decimalDegree = 57.2094;
            var expectedAngle = new Angle { Degrees = 57, Minutes = 12, Seconds = 34 };

            Angle result = MathHelpers.DecimalDegreesToAngle(decimalDegree);

            Assert.AreEqual(expectedAngle, result);
        }

        [TestMethod]
        public void DistanceBetweenPointsTest()
        {
            var x1 = 0;
            var y1 = 0;
            var x2 = 100;
            var y2 = 100;

            var expectedDistance = 141.4214;

            var result = MathHelpers.DistanceBetweenPoints(x1, x2, y1, y2);

            Assert.AreEqual(expectedDistance, result);
        }

        [TestMethod]
        public void DistanceBetweenPointsTest_Point()
        {
            var point1 = new Point(0, 0);
            var point2 = new Point(100, 100);

            var expectedDistance = 141.4214;

            var result = MathHelpers.DistanceBetweenPoints(point1, point2);

            Assert.AreEqual(expectedDistance, result);
        }

        [TestMethod]
        public void AngleBetweenPointsTest()
        {
            var x1 = 0;
            var y1 = 0;
            var x2 = 100;
            var y2 = 100;

            var expectedDistance = new Angle(45);

            var result = MathHelpers.AngleBetweenPoints(x1, x2, y1, y2);

            Assert.AreEqual(expectedDistance, result);
        }

        [TestMethod]
        public void AngleBetweenPointsTest_Point()
        {
            var point1 = new Point(0, 0);
            var point2 = new Point(100, 100);

            var expectedDistance = new Angle(45);

            var result = MathHelpers.AngleBetweenPoints(point1, point2);

            Assert.AreEqual(expectedDistance, result);
        }

        [TestMethod]
        public void AngleBetweenPointsTest_Point_Negative()
        {
            var point1 = new Point(-100, -100);
            var point2 = new Point(-200, -200);

            var expectedDistance = new Angle(225);

            var result = MathHelpers.AngleBetweenPoints(point1, point2);

            Assert.AreEqual(expectedDistance, result);
        }

        [TestMethod]
        public void BearingAndDistanceToCoordinatesTest()
        {
            var traverseObjects = new List<TraverseObject>
            {
                new TraverseObject(0, 30), 
                new TraverseObject(90, 10), 
                new TraverseObject(180, 30)
            };

            var basePoint = new Point(0, 0, 0);

            var results = MathHelpers.BearingAndDistanceToCoordinates(traverseObjects, basePoint);

            var expected = new List<Point>
            {
                new Point(0,0),
                new Point(0, 30),
                new Point(10, 30),
                new Point(10, 0)
            };

            CollectionAssert.AreEqual(expected, results);
        }

        [TestMethod]
        public void AngleAndDistanceToCoordinatesTest()
        {
            var traverseAngleObject = new List<TraverseAngleObject>
            {
                new TraverseAngleObject(0, 30),
                new TraverseAngleObject(90, 10),
                new TraverseAngleObject(90, 30),
            };

            var basePoint = new Point(0, 0);

            var results = MathHelpers.AngleAndDistanceToCoordinates(traverseAngleObject, basePoint);

            var expected = new List<Point>
            {
                new Point(0,0),
                new Point(0, 30),
                new Point(10, 30),
                new Point(10, 0)
            };

            CollectionAssert.AreEqual(expected, results);
        }

        [TestMethod]
        public void Intersection_Of_Two_Angles_As_Coordinates()
        {
            var pointA = new Vector(0, 0);
            var pointB = new Vector(0, 30);

            var pointC = new Vector(15, 15);
            var pointD = new Vector(-15, 15);

            var expected = new Point(0, 15);

            var worked = MathHelpers.LineSegementsIntersect(pointA, pointB, pointC, pointD, out Point result);

            Assert.AreEqual(expected, result);
            Assert.AreEqual(true, worked);
        }

        [TestMethod]
        public void Intersection_Of_Two_Angles_As_Coordinates_Parallel_Lines()
        {
            var pointA = new Vector(0, 0);
            var pointB = new Vector(0, 30);

            var pointC = new Vector(5, 0);
            var pointD = new Vector(5, 30);

            var expected = new Point(0, 0);

            var worked = MathHelpers.LineSegementsIntersect(pointA, pointB, pointC, pointD, out Point result);

            Assert.AreEqual(expected, result);
            Assert.AreEqual(false, worked);
        }

        [TestMethod]
        public void LineSegmentsDoNotIntersect()
        {
            Point intersection;
            var actual = MathHelpers.LineSegementsIntersect(
                new Vector(3, 0),
                new Vector(3, 4),
                new Vector(0, 5),
                new Vector(5, 5),
                out intersection);

            Assert.IsFalse(actual);
        }

        [TestMethod]
        public void LineSegmentsAreCollinearAndOverlapping()
        {
            Point intersection;
            var actual = MathHelpers.LineSegementsIntersect(
                new Vector(0, 0),
                new Vector(2, 0),
                new Vector(1, 0),
                new Vector(3, 0),
                out intersection, 
                considerCollinearOverlapAsIntersect: true);

            Assert.IsTrue(actual);
            Assert.AreEqual(0, intersection.X);
            Assert.AreEqual(0, intersection.Y);
        }

        [TestMethod]
        public void LineSegmentsAreCollinearAndOverlapping_ButFalse()
        {
            Point intersection;
            var actual = MathHelpers.LineSegementsIntersect(
                new Vector(0, 0),
                new Vector(2, 0),
                new Vector(1, 0),
                new Vector(3, 0),
                out intersection, 
                considerCollinearOverlapAsIntersect: false);

            Assert.IsFalse(actual);
            Assert.AreEqual(0, intersection.X);
            Assert.AreEqual(0, intersection.Y);
        }


    }
}