using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace _3DS_CivilSurveySuiteTests
{
    [TestClass]
    public class ClosureTests
    {
        private struct Coordinate
        {
            public double x;
            public double y;
        }

        [TestMethod]
        public void Test2dArea1()
        {
            double expectedArea = 300;
            List<Coordinate> coords = new List<Coordinate>
            {
                new Coordinate() { x = 0, y = 0 },
                new Coordinate() { x = 0, y = 30 },
                new Coordinate() { x = 10, y = 30 },
                new Coordinate() { x = 10, y = 0 }
            };

            var area = Math.Abs(coords.Take(coords.Count - 1).Select((p, i) => (coords[i + 1].x - p.x) * (coords[i + 1].y + p.y)).Sum() / 2);
            Assert.AreEqual(expectedArea, area);
        }

        [TestMethod]
        public void Test2dArea2()
        {
            double expectedArea = 300;
            List<Coordinate> coords = new List<Coordinate>
            {
                new Coordinate() { x = 0, y = 0 },
                new Coordinate() { x = 0, y = 10 },
                new Coordinate() { x = 30, y = 10 },
                new Coordinate() { x = 30, y = 0 }
            };

            var area = CalculateArea(coords);
            Assert.AreEqual(expectedArea, area);
        }

        [TestMethod]
        public void Test2dArea3()
        {
            double expectedArea = 839.8114;
            List<Coordinate> coords = new List<Coordinate>
            {
                new Coordinate() { x = 0, y = 0 },
                new Coordinate() { x = -4.2429, y = 23.4555 },
                new Coordinate() { x = 10.1496, y = 37.6785 },
                new Coordinate() { x = 27.9531, y = 22.7901 },
                new Coordinate() { x = 16.1396, y = -6.4877 },
                new Coordinate() { x = 7.9866, y = 2.3289 }
            };

            var area = PolygonArea(coords);
            Assert.AreEqual(expectedArea, Math.Round(area, 4));
        }

        double PolygonArea(List<Coordinate> polygon)
        {
            var array = polygon.ToArray();

            double area = 0;
            var j = array.Length - 1;

            for (int i = 0; i < array.Length; i++)
            {
                area += (polygon[j].x + polygon[i].x) * (polygon[j].y - polygon[i].y);
                j = i;
            }

            return area / 2;
        }

        private static double CalculateArea(List<Coordinate> coords)
        {
            if (coords.Count < 3)
                return -1;

            return Math.Abs(coords.Take(coords.Count - 1).Select((p, i) => (coords[i + 1].x - p.x) * (coords[i + 1].y + p.y)).Sum() / 2);
        }
    }
}
