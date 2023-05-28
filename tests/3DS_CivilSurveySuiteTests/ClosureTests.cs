using System;
using System.Collections.Generic;
using CivilSurveySuite.Common.Helpers;
using CivilSurveySuite.Common.Models;
using NUnit.Framework;

namespace CivilSurveySuiteTests
{
    [TestFixture]
    public class ClosureTests
    {
        [Test]
        public void Test2dArea1()
        {
            const double expectedArea = 300;
            var coords = new List<Point>
            {
                new Point(0, 0),
                new Point(0, 30),
                new Point(10, 30),
                new Point(10, 0)
            };

            var area = MathHelpers.Area(coords);
            Assert.AreEqual(expectedArea, area);
        }

        [Test]
        public void Test2dArea2()
        {
            const double expectedArea = 839.8114;
            var coords = new List<Point>
            {
                new Point(0,0),
                new Point(-4.2429, 23.4555),
                new Point(10.1496, 37.6785),
                new Point(27.9531, 22.7901),
                new Point(16.1396, -6.4877),
                new Point(7.9866, 2.3289)
            };

            var area = MathHelpers.Area(coords);
            Assert.AreEqual(expectedArea, Math.Round(area, 4));
        }
    }
}