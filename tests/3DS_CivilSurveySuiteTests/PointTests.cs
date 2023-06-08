using CivilSurveySuite.Common.Helpers;
using CivilSurveySuite.Common.Models;
using NUnit.Framework;

namespace CivilSurveySuiteTests
{
    [TestFixture]
    public class PointTests
    {
        [Test]
        public void IsValid_ShouldBeTrue()
        {
            var point = new Point(100, 100, 50);

            var result = point.IsValid();

            Assert.IsTrue(result);
        }

        [Test]
        public void IsValid_ShouldBeFalse()
        {
            var point = new Point(double.MinValue, 0, 0);

            var result = point.IsValid();

            Assert.IsFalse(result);
        }

        [Test]
        public void ToStringTest()
        {
            var point = new Point(0, 0, 0);

            var expectedString = "X:0,Y:0,Z:0";

            var result = point.ToString();

            Assert.AreEqual(expectedString, result);
        }

        [Test]
        public void TestLeftLegPoint()
        {
            var point1 = new Point(0, 0);
            var point2 = new Point(0, 30);
            var expectedPoint = new Point(-2, 0);

            var result = PointHelpers.CalculateRightAngleTurn(point1, point2);

            Assert.AreEqual(expectedPoint, result);
        }

        [Test]
        public void TestRightLegPoint()
        {
            var point1 = new Point(0, 0);
            var point2 = new Point(0, 30);
            var expectedPoint = new Point(2, 0);

            var result = PointHelpers.CalculateRightAngleTurn(point1, point2, false);

            Assert.AreEqual(expectedPoint, result);
        }

        [Test]
        public void TestRectangleCalculate()
        {
            var point1 = new Point(0, 0);
            var point2 = new Point(0, 30);
            var point3 = new Point(15, 30);
            var expectedPoint = new Point(15, 0);

            var result = PointHelpers.CalculateRectanglePoint(point1, point2, point3);

            Assert.AreEqual(expectedPoint, result);
        }
    }
}
