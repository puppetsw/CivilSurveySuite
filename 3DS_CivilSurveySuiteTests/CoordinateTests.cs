using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using static _3DS_CivilSurveySuiteTests.TraverseTests;

namespace _3DS_CivilSurveySuiteTests
{
    [TestClass]
    public class CoordinateTests
    {
        [TestMethod]
        public void TestCoordinate1()
        {
            var startEasting = 2923151.9610;
            var startNorthing = 1103222.9080;

            var distance = 206.306;
            var decimalDegree = 305.9922;
            var radians = DecimalDegreesToRadians(decimalDegree);
            var dms = DecimalDegreesToDMS(decimalDegree);

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

        [TestMethod]
        public void TestCoordinate2()
        {
            double startCoordx = 0;
            double startCoordy = 0;

            double bearing = 354.5020;
            double distance = 50;

            double expectedX = -4.4978;
            double expectedY = 49.7973;

            var decimalDegree = DMSToDecimalDegrees(new DMS() { Degrees = 354, Minutes = 50, Seconds = 20 });
            var radians = DecimalDegreesToRadians(decimalDegree);

            double depature = distance * Math.Sin(radians);
            double latitude = distance * Math.Cos(radians);

            double newEast = Math.Round(startCoordx + depature, 4);
            double newNorth = Math.Round(startCoordy + latitude, 4);

            Assert.AreEqual(expectedX, newEast);
            Assert.AreEqual(expectedY, newNorth);
        }

        private struct Coordinate
        {
            public double x;
            public double y;
        }

        [TestMethod]
        public void TestClosure()
        {
            var dmsList = new List<DMS>();
            var coordinates = new List<Coordinate>();

            coordinates.Add(new Coordinate() { x = 0, y = 0 });

            double distance = 50;

            dmsList.Add(new DMS() { Degrees = 354, Minutes = 0, Seconds = 0 });
            dmsList.Add(new DMS() { Degrees = 84, Minutes = 0, Seconds = 0 });
            dmsList.Add(new DMS() { Degrees = 174, Minutes = 0, Seconds = 0 });

            int i = 0;

            foreach (var dms in dmsList)
            {
                var dec = DMSToDecimalDegrees(dms);
                var rad = DecimalDegreesToRadians(dec);

                double depature = distance * Math.Sin(rad);
                double latitude = distance * Math.Cos(rad);

                var startingX = coordinates[i].x;
                var startingY = coordinates[i].y;

                double newEast = Math.Round(startingX + depature, 4);
                double newNorth = Math.Round(startingY + latitude, 4);

                coordinates.Add(new Coordinate() { x = newEast, y = newNorth });

                i++;
            }

            //work out last bearing and distance
            int lastIndex = coordinates.Count - 1;
            int firstIndex = 0; 

            var x = Math.Abs(coordinates[lastIndex].x - coordinates[firstIndex].x);
            var y = Math.Abs(coordinates[lastIndex].y - coordinates[firstIndex].y);

            var distanceBetween = Math.Round(Math.Sqrt((x * x) + (y * y)), 4);

            Assert.AreEqual(distance, distanceBetween);

            var angleRad = Math.Atan2(coordinates[firstIndex].x - coordinates[lastIndex].x, coordinates[firstIndex].y - coordinates[lastIndex].y);

            if (angleRad < 0)
                angleRad += 2 * Math.PI; // if radians is less than 0 add 2PI

            var decDeg = Math.Abs(angleRad) * 180 / Math.PI;
            var resultDMS = DecimalDegreesToDMS(decDeg);

            Assert.AreEqual(264, resultDMS.Degrees);
        }

        [TestMethod]
        public void TestDMStoDecimalDegrees()
        {
            var dms = new DMS();
            dms.Degrees = 57;
            dms.Minutes = 12;
            dms.Seconds = 34;

            double expectedResult = 57.2094;
            double result = DMSToDecimalDegrees(dms);

            Assert.AreEqual(expectedResult, Math.Round(result, 4));
        }

        [TestMethod]
        public void TestDecimalDegreeToDMS()
        {
            var decimalDegree = 57.2094;
            var expectedDMS = new DMS() { Degrees = 57, Minutes = 12, Seconds = 33 }; //seconds should be 34

            var result = DecimalDegreesToDMS(decimalDegree);

            Assert.AreEqual(expectedDMS, result);
        }

        [TestMethod]
        public void TestDecimalDegreesToRadians()
        {
            double expectedResult = 0.9985;
            double decimalBearing = 57.2094;

            double result = DecimalDegreesToRadians(decimalBearing);

            Assert.AreEqual(expectedResult, Math.Round(result, 4));
        }

        private static double DMSToDecimalDegrees(DMS dMS)
        {
            double minutes = Convert.ToDouble(dMS.Minutes) / 60;
            double seconds = Convert.ToDouble(dMS.Seconds) / 3600;

            double decimalDegree = dMS.Degrees + minutes + seconds;

            return decimalDegree;
        }

        private static double DecimalDegreesToRadians(double decimalDegrees)
        {
            return decimalDegrees * (Math.PI / 180);
        }

        private static DMS DecimalDegreesToDMS(double decimalDegrees)
        {
            //gets the degree
            //result.Degrees = (int)Math.Floor(angleInDegrees);
            //var delta = angleInDegrees - result.Degrees;

            //gets minutes and seconds
            //var seconds = (int)Math.Floor(3600.0 * delta);
            //result.Seconds = seconds % 60;
            //result.Minutes = (int)Math.Floor(seconds / 60.0);
            //delta = delta * 3600.0 - seconds;

            int degrees = (int)Math.Floor(decimalDegrees);
            var delta = decimalDegrees - degrees;

            var secondsTemp = (int)Math.Floor(3600.0 * delta);
            int seconds = secondsTemp % 60;
            int minutes = (int)Math.Floor(secondsTemp / 60.0);

            //double degrees = Math.Truncate(decimalDegrees);
            //double minutes = (decimalDegrees - Math.Floor(decimalDegrees)) * 60.0;
            //double seconds = (minutes - Math.Floor(minutes)) * 60.0;
            // get rid of fractional part
            //minutes = Math.Floor(minutes);
            //seconds = Math.Floor(seconds);

            return new DMS() { Degrees=Convert.ToInt32(degrees), Minutes= Convert.ToInt32(minutes), Seconds= Convert.ToInt32(seconds) };
        }

    }
}
