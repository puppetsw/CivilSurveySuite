using System;
using System.Collections.Generic;
using _3DS_CivilSurveySuite.Shared.Helpers;
using _3DS_CivilSurveySuite.Shared.Models;
using NUnit.Framework;

namespace _3DS_CivilSurveySuiteTests
{
    [TestFixture]
    public class MathHelpersTests
    {
        [Test]
        public void ConvertLinkToMetersTest()
        {
            var linkValue = 100;
            var expectedValue = 20.1168;

            double result = MathHelpers.ConvertLinkToMeters(linkValue);
            Assert.AreEqual(expectedValue, result);
        }

        [Test]
        public void ConvertFeetToMetersTest_FeetOnly()
        {
            var feetValue = 100;
            var expectedValue = 30.48;

            double result = MathHelpers.ConvertFeetToMeters(feetValue);
            Assert.AreEqual(expectedValue, result);
        }

        [Test]
        public void ConvertFeetToMetersTest_FeetAndInches()
        {
            var feetAndInchesValue = 100.10;
            var expectedValue = 30.734;

            double result = MathHelpers.ConvertFeetToMeters(feetAndInchesValue);
            Assert.AreEqual(expectedValue, result);
        }

        [Test]
        public void ConvertFeetToMetersTest_FeetAndInches_InchesLessThan10()
        {
            var feetAndInchesValue = 100.09;
            var expectedValue = 30.7086;

            double result = MathHelpers.ConvertFeetToMeters(feetAndInchesValue);
            Assert.AreEqual(expectedValue, result);
        }

        [Test]
        public void AngleToDecimalDegreesTest()
        {
            var angle = new Angle { Degrees = 57, Minutes = 12, Seconds = 34 };

            const double expectedResult = 57.2094;
            double result = angle.ToDecimalDegrees();

            Assert.AreEqual(expectedResult, result, 4);
        }

        [Test]
        public void AngleToDecimalDegreesTest_ShouldReturnZero()
        {
            Angle angle = null;

            const double expectedResult = 0;
            // ReSharper disable once ExpressionIsAlwaysNull
            double result = angle.ToDecimalDegrees();

            Assert.AreEqual(expectedResult, result);
        }

        [Test]
        public void DecimalDegreesToRadiansTest()
        {
            const double expectedResult = 0.9985;
            const double decimalDegrees = 57.2094;

            double result = Math.Round(MathHelpers.DecimalDegreesToRadians(decimalDegrees), 4);

            Assert.AreEqual(expectedResult, result);
        }

        [Test]
        public void DecimalDegreesToAngleTest()
        {
            var decimalDegree = 57.2094;
            var expectedAngle = new Angle { Degrees = 57, Minutes = 12, Seconds = 34 };

            Angle result = AngleHelpers.DecimalDegreesToAngle(decimalDegree);

            Assert.AreEqual(expectedAngle, result);
        }

        [Test]
        public void DistanceBetweenPointsTest()
        {
            var x1 = 0;
            var y1 = 0;
            var x2 = 100;
            var y2 = 100;

            var expectedDistance = 141.4214;

            var result = MathHelpers.GetDistanceBetweenPoints(x1, x2, y1, y2);

            Assert.AreEqual(expectedDistance, Math.Round(result, 4));
        }

        [Test]
        public void DistanceBetweenPointsTest_Point()
        {
            var point1 = new Point(0, 0);
            var point2 = new Point(100, 100);

            var expectedDistance = 141.4214;

            var result = PointHelpers.GetDistanceBetweenPoints(point1, point2);

            Assert.AreEqual(expectedDistance, Math.Round(result, 4));
        }

        [Test]
        public void AngleBetweenPointsTest()
        {
            var x1 = 0;
            var y1 = 0;
            var x2 = 100;
            var y2 = 100;

            var expectedDistance = new Angle(45);

            var result = AngleHelpers.GetAngleBetweenPoints(x1, x2, y1, y2);

            Assert.AreEqual(expectedDistance, result);
        }

        [Test]
        public void AngleBetweenPointsTest_Point()
        {
            var point1 = new Point(0, 0);
            var point2 = new Point(100, 100);

            var expectedDistance = new Angle(45);

            var result = AngleHelpers.GetAngleBetweenPoints(point1, point2);

            Assert.AreEqual(expectedDistance, result);
        }

        [Test]
        public void AngleBetweenPointsTest_Point_Negative()
        {
            var point1 = new Point(-100, -100);
            var point2 = new Point(-200, -200);

            var expectedDistance = new Angle(225);

            var result = AngleHelpers.GetAngleBetweenPoints(point1, point2);

            Assert.AreEqual(expectedDistance, result);
        }

        [Test]
        public void BearingAndDistanceToCoordinatesTest()
        {
            var traverseObjects = new List<TraverseObject>
            {
                new TraverseObject(0, 30),
                new TraverseObject(90, 10),
                new TraverseObject(180, 30)
            };

            var basePoint = new Point(0, 0, 0);

            var results = PointHelpers.TraverseObjectsToCoordinates(traverseObjects, basePoint);

            var expected = new List<Point>
            {
                new Point(0,0),
                new Point(0, 30),
                new Point(10, 30),
                new Point(10, 0)
            };

            CollectionAssert.AreEqual(expected, results);
        }

        [Test]
        public void AngleAndDistanceToCoordinatesTest()
        {
            var traverseAngleObject = new List<TraverseAngleObject>
            {
                new TraverseAngleObject(0, 30),
                new TraverseAngleObject(90, 10),
                new TraverseAngleObject(90, 30),
            };

            var basePoint = new Point(0, 0);

            var results = PointHelpers.TraverseObjectsToCoordinates(traverseAngleObject, basePoint);

            var expected = new List<Point>
            {
                new Point(0,0),
                new Point(0, 30),
                new Point(10, 30),
                new Point(10, 0)
            };

            CollectionAssert.AreEqual(expected, results);
        }

        [Test]
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

        [Test]
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

        [Test]
        public void Point_To_Vector()
        {
            var point = new Point(100, 100);

            var expectedVector = new Vector(100, 100);

            Assert.AreEqual(expectedVector, point.ToVector());
        }

        [Test]
        public void LineSegmentsDoNotIntersect()
        {
            var actual = MathHelpers.LineSegementsIntersect(
                new Vector(3, 0),
                new Vector(3, 4),
                new Vector(0, 5),
                new Vector(5, 5),
                out Point _);

            Assert.IsFalse(actual);
        }

        [Test]
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

        [Test]
        public void Vector_Multiply_By_Double()
        {
            var vector = new Vector(5, 5);
            var result = vector * 2;

            Assert.AreEqual(new Vector(10, 10), result);
        }

        [Test]
        public void Vector_Empty_Constructor_()
        {
            var vector = new Vector();
            Assert.AreEqual("X:NaN,Y:NaN", vector.ToString());
        }

        [Test]
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

        [Test]
        public void Convert_DecimalDegrees_To_Radians_And_Back_To_DecimalDegrees()
        {
            var angle = new Angle(90);
            var dec = angle.ToDecimalDegrees();
            var expectedDecimalDegrees = 90;

            Assert.AreEqual(expectedDecimalDegrees, dec);

            var expectedRadians = 1.570796326794897;
            var radians = MathHelpers.DecimalDegreesToRadians(dec);

            var checkEqualsRadians = MathHelpers.NearlyEqual(expectedRadians, radians);
            Assert.AreEqual(true, checkEqualsRadians);

            var convertedRadians = MathHelpers.RadiansToDecimalDegrees(radians);

            var checkEquals = MathHelpers.NearlyEqual(expectedDecimalDegrees, convertedRadians);

            Assert.AreEqual(true, checkEquals);


            var finalAngle = AngleHelpers.DecimalDegreesToAngle(Math.Round(convertedRadians, 4));

            Assert.AreEqual(angle, finalAngle);
        }

        [Test]
        public void Convert_Radians_To_Angle()
        {
            var radians = 1.570796326794897;
            var expectedAngle = new Angle(90);

            var angle = AngleHelpers.RadiansToAngle(radians);

            Assert.AreEqual(expectedAngle, angle);
        }

        [Test]
        public void IsLeft_RightSide_ShouldBe1()
        {
            var startPoint = new Point(0, 0);
            var endPoint = new Point(0, 30);

            var pickedPoint = new Point(15, 15);

            var side = MathHelpers.IsLeft(startPoint, endPoint, pickedPoint);

            Assert.AreEqual(-1, side);
        }

        [Test]
        public void IsLeft_LeftSide_ShouldBeTrue()
        {
            var startPoint = new Point(0, 0);
            var endPoint = new Point(0, 30);

            var pickedPoint = new Point(-15, -15);

            var side = MathHelpers.IsLeft(startPoint, endPoint, pickedPoint);

            Assert.AreEqual(1, side);
        }

        [Test]
        public void IsLeft_OnLine_ShouldBe0()
        {
            var startPoint = new Point(0, 0);
            var endPoint = new Point(0, 30);

            var pickedPoint = new Point(0, 15);

            var side = MathHelpers.IsLeft(startPoint, endPoint, pickedPoint);


            Assert.AreEqual(0, side);
        }

        [Test]
        public void IsConvexAngle_ShouldBeTrue()
        {
            var angle = new Angle(45);

            var result = AngleHelpers.IsOrdinaryAngle(angle);

            Assert.AreEqual(true, result);
        }

        [Test]
        public void IsConvexAngle_ShouldBeFalse()
        {
            var angle = new Angle(270);

            var result = AngleHelpers.IsOrdinaryAngle(angle);

            Assert.AreEqual(false, result);
        }

        [Test]
        public void IsOrdinaryAngle_ShouldBeTrue()
        {
            var startPoint = new Point(0, 0);
            var endPoint = new Point(50, 50);

            var result = MathHelpers.IsOrdinaryAngle(startPoint, endPoint);

            Assert.IsTrue(result);
        }

        [Test]
        public void IsOrdinaryAngle_ShouldBeFalse()
        {
            var startPoint = new Point(50, 50);
            var endPoint = new Point(0, 0);

            var result = MathHelpers.IsOrdinaryAngle(startPoint, endPoint);

            Assert.IsFalse(result);
        }

        [Test]
        public void GetOrdinaryAngle_ShouldBeOrdinary()
        {
            var angle = new Angle(90);
            var expected = new Angle(90);

            var result = angle.GetOrdinaryAngle();

            Assert.AreEqual(expected, result);
        }

        [Test]
        public void GetOrdinaryAngle_ShouldNotBeOrdinary()
        {
            var angle = new Angle(270);
            var expected = new Angle(90);

            var result = angle.GetOrdinaryAngle();

            Assert.AreEqual(expected, result);
        }

        [Test]
        public void ToRadians()
        {
            var angle = new Angle(180);
            var expectedRadians = 3.14159265358979;

            var result = angle.ToRadians();

            var assertion = (MathHelpers.NearlyEqual(expectedRadians, result));

            Assert.AreEqual(true, assertion);
        }

        [Test]
        public void MidpointBetweenPoints()
        {
            var expectedPoint = new Point(55, 55);
            var startPoint = new Point(10, 10);
            var endPoint = new Point(100, 100);

            var actual = PointHelpers.GetMidpointBetweenPoints(startPoint, endPoint);

            Assert.AreEqual(expectedPoint, actual);
        }

        [Test]
        public void CoordinateDelta_ReturnDifference()
        {
            var firstPoint = Point.Origin;
            var secondPoint = new Point(100, 100, 50);

            var expectedDelta = new Point(-100, -100, -50);

            var result = MathHelpers.DeltaPoint(firstPoint, secondPoint);

            Assert.AreEqual(expectedDelta, result);
        }

        [Test]
        public void Find_Min_And_Max_Coordinates()
        {
            var list = new List<Point>
            {
                new Point(0, 0),
                new Point(10, 10),
                new Point(0, 10),
                new Point(-10, 0)
            };

            var result = PointHelpers.GetBounds(list);

            Assert.AreEqual(new Point(-10, 0), result.MinPoint);
            Assert.AreEqual(new Point(10, 10), result.MaxPoint);
        }

        [Test]
        public void Find_Min_And_Max_Coordinates_One_Coordinate()
        {
            var list = new List<Point>
            {
                new Point(0, 0),
                new Point(10, 10)
            };

            var result = PointHelpers.GetBounds(list);

            Assert.AreEqual(new Point(0, 0), result.MinPoint);
            Assert.AreEqual(new Point(10, 10), result.MaxPoint);
        }
    }
}