using System;
using System.Collections.Generic;
using CivilSurveySuite.Common.Models;
using NUnit.Framework;

namespace CivilSurveySuiteTests
{
    [TestFixture]
    public class CoordinateTests
    {
        public struct Coordinate
        {
            public double X;
            public double Y;
        }

        [Test]
        public void TestCoordinate1()
        {
            var startEasting = 2923151.9610;
            var startNorthing = 1103222.9080;

            var distance = 206.306;
            var decimalDegree = 305.9922;
            var radians = DecimalDegreesToRadians(decimalDegree);

            double depature = distance * Math.Sin(radians);
            double latitude = distance * Math.Cos(radians);

            Assert.AreEqual(-166.9216, Math.Round(depature, 4));
            Assert.AreEqual(121.2409, Math.Round(latitude, 4));

            double newEast = Math.Round(startEasting + depature, 4);
            double newNorth = Math.Round(startNorthing + latitude, 4);

            double expectedEast = 2922985.0394;
            double expectedNorth = 1103344.1489;

            Assert.AreEqual(expectedEast, newEast);
            Assert.AreEqual(expectedNorth, newNorth);
        }

        [Test]
        public void TestCoordinate2()
        {
            const double startCoordx = 0;
            const double startCoordy = 0;

            const double distance = 50;

            const double expectedX = -4.4978;
            const double expectedY = 49.7973;

            double decimalDegree = DMSToDecimalDegrees(new Angle() { Degrees = 354, Minutes = 50, Seconds = 20 });
            double radians = DecimalDegreesToRadians(decimalDegree);

            double departure = distance * Math.Sin(radians);
            double latitude = distance * Math.Cos(radians);

            double newEast = Math.Round(startCoordx + departure, 4);
            double newNorth = Math.Round(startCoordy + latitude, 4);

            Assert.AreEqual(expectedX, newEast);
            Assert.AreEqual(expectedY, newNorth);
        }

        [Test]
        public void TestClosure()
        {
            var dmsList = new List<Angle>();
            var coordinates = new List<Coordinate>
            {
                new Coordinate() { X = 0, Y = 0 }
            };

            const double distance = 50;

            dmsList.Add(new Angle { Degrees = 354, Minutes = 0, Seconds = 0 });
            dmsList.Add(new Angle { Degrees = 84, Minutes = 0, Seconds = 0 });
            dmsList.Add(new Angle { Degrees = 174, Minutes = 0, Seconds = 0 });

            int i = 0;

            //calculate coordinates from bearing and distance
            foreach (Angle dms in dmsList)
            {
                double dec = DMSToDecimalDegrees(dms);
                double rad = DecimalDegreesToRadians(dec);

                double departure = distance * Math.Sin(rad);
                double latitude = distance * Math.Cos(rad);

                double startingX = coordinates[i].X;
                double startingY = coordinates[i].Y;

                double newEast = Math.Round(startingX + departure, 4);
                double newNorth = Math.Round(startingY + latitude, 4);

                coordinates.Add(new Coordinate() { X = newEast, Y = newNorth });

                i++;
            }

            //work out last bearing and distance
            int lastIndex = coordinates.Count - 1;
            int firstIndex = 0;

            double x = Math.Abs(coordinates[lastIndex].X - coordinates[firstIndex].X);
            double y = Math.Abs(coordinates[lastIndex].Y - coordinates[firstIndex].Y);

            double distanceBetween = Math.Round(Math.Sqrt((x * x) + (y * y)), 4);

            Assert.AreEqual(distance, distanceBetween);

            double angleRad = Math.Atan2(coordinates[firstIndex].X - coordinates[lastIndex].X, coordinates[firstIndex].Y - coordinates[lastIndex].Y);

            if (angleRad < 0)
            {
                angleRad += 2 * Math.PI; // if radians is less than 0 add 2PI
            }

            double decDeg = Math.Abs(angleRad) * 180 / Math.PI;
            Angle resultDMS = DecimalDegreesToDMS(decDeg);

            Assert.AreEqual(264, resultDMS.Degrees);
        }

        [Test]
        public void TestClosure2()
        {
            var dmsList = new List<Angle>();
            var coordinates = new List<Coordinate>
            {
                new Coordinate { X = 0, Y = 0 }
            };

            const double distance = 50;

            dmsList.Add(new Angle { Degrees = 354, Minutes = 0, Seconds = 0 });
            dmsList.Add(new Angle { Degrees = 84, Minutes = 0, Seconds = 0 });
            dmsList.Add(new Angle { Degrees = 0, Minutes = 0, Seconds = 0 });
            dmsList.Add(new Angle { Degrees = 0, Minutes = 0, Seconds = 0 });
            dmsList.Add(new Angle { Degrees = 0, Minutes = 0, Seconds = 0 });
            dmsList.Add(new Angle { Degrees = 0, Minutes = 0, Seconds = 0 });

            int i = 0;

            //calculate coordinates from bearing and distance
            foreach (Angle dms in dmsList)
            {
                double dec = DMSToDecimalDegrees(dms);
                double rad = DecimalDegreesToRadians(dec);

                double departure = distance * Math.Sin(rad);
                double latitude = distance * Math.Cos(rad);

                double startingX = coordinates[i].X;
                double startingY = coordinates[i].Y;

                double newEast = Math.Round(startingX + departure, 4);
                double newNorth = Math.Round(startingY + latitude, 4);

                coordinates.Add(new Coordinate() { X = newEast, Y = newNorth });

                i++;
            }

            //work out last bearing and distance
            int lastIndex = coordinates.Count - 1;
            const int firstIndex = 0;

            double angleRad = Math.Atan2(coordinates[firstIndex].X - coordinates[lastIndex].X, coordinates[firstIndex].Y - coordinates[lastIndex].Y);

            if (angleRad < 0)
                angleRad += 2 * Math.PI; // if radians is less than 0 add 2PI

            double decDeg = Math.Abs(angleRad) * 180 / Math.PI;
            Angle resultDMS = DecimalDegreesToDMS(decDeg);

            Assert.AreEqual(189, resultDMS.Degrees);
        }

        [Test]
        public void TestDMSToDecimalDegrees()
        {
            var dms = new Angle { Degrees = 57, Minutes = 12, Seconds = 34 };

            const double expectedResult = 57.2094;
            double result = DMSToDecimalDegrees(dms);

            Assert.AreEqual(expectedResult, Math.Round(result, 4));
        }

        [Test]
        public void TestDecimalDegreeToDMS()
        {
            const double decimalDegree = 57.2094;
            var expectedDMS = new Angle { Degrees = 57, Minutes = 12, Seconds = 34 };

            Angle result = DecimalDegreesToDMS(decimalDegree);

            Assert.AreEqual(expectedDMS, result);
        }

        [Test]
        public void TestDecimalDegreesToRadians()
        {
            const double expectedResult = 0.9985;
            const double decimalBearing = 57.2094;

            double result = DecimalDegreesToRadians(decimalBearing);

            Assert.AreEqual(expectedResult, Math.Round(result, 4));
        }

        private static double DMSToDecimalDegrees(Angle dms)
        {
            double minutes = (double) dms.Minutes / 60;
            double seconds = (double) dms.Seconds / 3600;
            return dms.Degrees + minutes + seconds;
        }

        private static double DecimalDegreesToRadians(double decimalDegrees)
        {
            return decimalDegrees * (Math.PI / 180);
        }

        private static Angle DecimalDegreesToDMS(double decimalDegrees)
        {
            double degrees = Math.Floor(decimalDegrees);
            double minutes = Math.Floor((decimalDegrees - degrees) * 60);
            double seconds = Math.Round((((decimalDegrees - degrees) * 60) - minutes) * 60, 0);

            return new Angle { Degrees = Convert.ToInt32(degrees), Minutes = Convert.ToInt32(minutes), Seconds = Convert.ToInt32(seconds) };
        }

        [Test]
        public void TestDecimalRounding()
        {
            const double dec = 156.742;
            const double expectedDeg = 156;
            const double expectedMin = 44;
            const double expectedSec = 31;

            /*The whole number is degrees.So 156.742 gives you 156 degrees.
            Multiply the remaining decimal by 60.
            0.742 * 60 = 44.52, so the whole number 44 equals minutes.
            Multiply the remaining decimal by 60.
            0.52 * 60 = 31.2, so the whole number 31 equals seconds.
            Decimal degrees 156.742 converts to 156 degrees, 44 minutes and 31 seconds, or 156° 44' 31".
            Be sure to follow math rules of rounding when calculating seconds by hand.
            If your resulting seconds is something like 31.9 you may round up to 32.*/

            double degrees = Math.Floor(dec);
            Assert.AreEqual(expectedDeg, degrees);

            double minutes = Math.Floor((dec - degrees) * 60);
            Assert.AreEqual(expectedMin, minutes);

            double seconds = Math.Round((((dec - degrees) * 60) - minutes) * 60, 0);
            Assert.AreEqual(expectedSec, seconds);
        }

        [Test]
        public void TestDecimalRounding2()
        {
            const double dec = 57.2094;
            const double expectedDeg = 57;
            const double expectedMin = 12;
            const double expectedSec = 34;

            var degrees = Math.Floor(dec);
            Assert.AreEqual(expectedDeg, degrees);

            var minutes = Math.Floor((dec - degrees) * 60);
            Assert.AreEqual(expectedMin, minutes);

            var seconds = Math.Round((((dec - degrees) * 60) - minutes) * 60, 0);
            Assert.AreEqual(expectedSec, seconds);
        }
    }
}