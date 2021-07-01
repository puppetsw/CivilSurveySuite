// Copyright Scott Whitney. All Rights Reserved.
// Reproduction or transmission in whole or in part, any form or by any
// means, electronic, mechanical or otherwise, is prohibited without the
// prior written consent of the copyright owner.
// 
// Filename: ClosureTests.cs
// Date:     01/07/2021
// Author:   scott

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace _3DS_CivilSurveySuiteTests
{
    [TestClass]
    public class ClosureTests
    {
        private struct Coordinate
        {
            public double X;
            public double Y;
        }

        [TestMethod]
        public void Test2dArea1()
        {
            const double expectedArea = 300;
            var coords = new List<Coordinate>
            {
                new Coordinate() { X = 0, Y = 0 },
                new Coordinate() { X = 0, Y = 30 },
                new Coordinate() { X = 10, Y = 30 },
                new Coordinate() { X = 10, Y = 0 }
            };

            var area = Math.Abs(coords.Take(coords.Count - 1).Select((p, i) => (coords[i + 1].X - p.X) * (coords[i + 1].Y + p.Y)).Sum() / 2);
            Assert.AreEqual(expectedArea, area);
        }

        [TestMethod]
        public void Test2dArea2()
        {
            const double expectedArea = 300;
            var coords = new List<Coordinate>
            {
                new Coordinate() { X = 0, Y = 0 },
                new Coordinate() { X = 0, Y = 10 },
                new Coordinate() { X = 30, Y = 10 },
                new Coordinate() { X = 30, Y = 0 }
            };

            var area = CalculateArea(coords);
            Assert.AreEqual(expectedArea, area);
        }

        [TestMethod]
        public void Test2dArea3()
        {
            const double expectedArea = 839.8114;
            var coords = new List<Coordinate>
            {
                new Coordinate() { X = 0, Y = 0 },
                new Coordinate() { X = -4.2429, Y = 23.4555 },
                new Coordinate() { X = 10.1496, Y = 37.6785 },
                new Coordinate() { X = 27.9531, Y = 22.7901 },
                new Coordinate() { X = 16.1396, Y = -6.4877 },
                new Coordinate() { X = 7.9866, Y = 2.3289 }
            };

            var area = PolygonArea(coords);
            Assert.AreEqual(expectedArea, Math.Round(area, 4));
        }

        private static double PolygonArea(List<Coordinate> polygon)
        {
            var array = polygon.ToArray();

            double area = 0;
            var j = array.Length - 1;

            for (int i = 0; i < array.Length; i++)
            {
                area += (polygon[j].X + polygon[i].X) * (polygon[j].Y - polygon[i].Y);
                j = i;
            }

            return area / 2;
        }

        private static double CalculateArea(IReadOnlyList<Coordinate> coords)
        {
            if (coords.Count < 3)
                return -1;

            return Math.Abs(coords.Take(coords.Count - 1).Select((p, i) => (coords[i + 1].X - p.X) * (coords[i + 1].Y + p.Y)).Sum() / 2);
        }
    }
}