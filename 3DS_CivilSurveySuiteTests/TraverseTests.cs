using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace _3DS_CivilSurveySuiteTests
{
    [TestClass]
    public class TraverseTests
    {
        [TestMethod]
        public void TestParsingBearing8Length()
        {
            var expectedResult = new DMS() { Degrees = 354, Minutes = 50, Seconds = 20 };
            var testString = "354.5020";
            Assert.AreEqual(expectedResult, ParseBearing(testString));
        }

        [TestMethod]
        public void TestParsingBearing6Length()
        {
            var expectedResult = new DMS() { Degrees = 354, Minutes = 50, Seconds = 0 };
            var testString = "354.50";
            Assert.AreEqual(expectedResult, ParseBearing(testString));
        }

        [TestMethod]
        public void TestParsingBearing3Length()
        {
            var expectedResult = new DMS() { Degrees = 354, Minutes = 0, Seconds = 0 };
            var testString = "354";
            Assert.AreEqual(expectedResult, ParseBearing(testString));
        }

        [TestMethod]
        public void TestParsingBearing8LengthDouble()
        {
            var expectedResult = new DMS() { Degrees = 354, Minutes = 50, Seconds = 20 };
            var testString = 354.5020;
            Assert.AreEqual(expectedResult, ParseBearing(testString));
        }

        [TestMethod]
        public void TestParsingBearing6LengthDouble()
        {
            var expectedResult = new DMS() { Degrees = 354, Minutes = 50, Seconds = 0 };
            var testString = 354.50;
            Assert.AreEqual(expectedResult, ParseBearing(testString));
        }

        [TestMethod]
        public void TestParsingBearing3LengthDouble()
        {
            var expectedResult = new DMS() { Degrees = 354, Minutes = 0, Seconds = 0 };
            var testString = 354;
            Assert.AreEqual(expectedResult, ParseBearing(testString));
        }

        [TestMethod]
        public void TestParsingBearing2LengthDouble()
        {
            var expectedResult = new DMS() { Degrees = 54, Minutes = 0, Seconds = 0 };
            var testString = 54;
            Assert.AreEqual(expectedResult, ParseBearing(testString));
        }

        [TestMethod]
        public void TestParsingBearing4LengthDouble()
        {
            var expectedResult = new DMS() { Degrees = 54, Minutes = 20, Seconds = 0 };
            var testString = 54.20;
            Assert.AreEqual(expectedResult, ParseBearing(testString));
        }

        [TestMethod]
        public void TestParsingBearing3ShortLengthDouble()
        {
            var expectedResult = new DMS() { Degrees = 5, Minutes = 20, Seconds = 0 };
            var testString = 5.20;
            Assert.AreEqual(expectedResult, ParseBearing(testString));
        }

        [TestMethod]
        public void TestBearingAddition()
        {
            double bearing1 = 25.4538;
            double bearing2 = 40.1747;

            double expectedResultDegrees = 66;
            double expectedResultMinutes = 3;
            double expectedResultSeconds = 25;

            var result = BearingAddition(bearing1, bearing2);

            Assert.AreEqual(expectedResultDegrees, result.Degrees);
            Assert.AreEqual(expectedResultMinutes, result.Minutes);
            Assert.AreEqual(expectedResultSeconds, result.Seconds);
        }

        [TestMethod]
        public void TestBearingSubtraction()
        {
            double bearing1 = 85.1537;
            double bearing2 = 46.2245;

            double expectedResultDegrees = 38;
            double expectedResultMinutes =52;
            double expectedResultSeconds = 52;

            var result = BearingSubtraction(bearing1, bearing2);

            Assert.AreEqual(expectedResultDegrees, result.Degrees);
            Assert.AreEqual(expectedResultMinutes, result.Minutes);
            Assert.AreEqual(expectedResultSeconds, result.Seconds);
        }

        [TestMethod]
        public void TestValidBearing()
        {
            var expectedResult = true;
            var bearing = 354.5020;

            var dms = ParseBearing(bearing);

            var actual = IsValid(dms);

            Assert.AreEqual(expectedResult, actual);
        }

        [TestMethod]
        public void TestInvalidBearing()
        {
            var expectedResult = false;
            var bearing = 374.5020;

            var dms = ParseBearing(bearing);

            var actual = IsValid(dms);

            Assert.AreEqual(expectedResult, actual);
        }

        [TestMethod]
        public void TestInvalidBearing2()
        {
            var expectedResult = false;
            var bearing = 354.6120;

            var dms = ParseBearing(bearing);

            var actual = IsValid(dms);

            Assert.AreEqual(expectedResult, actual);
        }

        [TestMethod]
        public void TestFeetConversion1()
        {
            var feetExpect = 47.244;
            var inchExpect = 0.1524;
            double expectedResult = feetExpect + inchExpect;

            var result = ConvertFeetToMeters(155.06);

            Assert.AreEqual(expectedResult, Math.Round(result, 4));
        }

        [TestMethod]
        public void TestFeetConversion2()
        {
            var feetExpect = 47.244;
            var inchExpect = 0.254;
            double expectedResult = feetExpect + inchExpect;

            var result = ConvertFeetToMeters(155.10);

            Assert.AreEqual(expectedResult, Math.Round(result, 4));
        }

        [TestMethod]
        public void TestLinkConversion1()
        {
            var expected = 20.1168;
            var link = 100;

            var result = ConvertLinkToMeters(link);

            Assert.AreEqual(expected, result);

        }

        /// <summary>
        /// Converts link to meters
        /// </summary>
        /// <param name="link"></param>
        /// <returns></returns>
        private double ConvertLinkToMeters(double link)
        {
            const double linkConversion = 0.201168;

            return link * linkConversion;
        }


        /// <summary>
        /// Converts feet and inches to meters
        /// </summary>
        /// <param name="feetAndInches">
        /// Feet and inches represented as decimal. 5feet 2inch 5.02.
        /// Inches less than 10 must have a preceeding 0. 
        /// </param>
        /// <returns></returns>
        private double ConvertFeetToMeters(double feetAndInches)
        {
            const double feetConversion = 0.3048;
            const double inchConversion = 0.0254;

            var feet = Math.Truncate(feetAndInches) * feetConversion;
            var inch1 = feetAndInches - Math.Truncate(feetAndInches);
            var inch2 = (inch1 * 100) * inchConversion;

            return feet + inch2;
        }

        private DMS BearingAddition(double bearing1, double bearing2)
        {
            var dms1 = ParseBearing(bearing1);
            var dms2 = ParseBearing(bearing2);

            var degrees = dms1.Degrees + dms2.Degrees;
            var minutes = dms1.Minutes + dms2.Minutes;
            var seconds = dms1.Seconds + dms2.Seconds;

            //work out seconds first, carry over to minutes
            if (seconds > 60)
            {
                seconds -= 60;
                minutes++;
            }

            //work out minutes, carry over to degrees
            if (minutes > 60)
            {
                minutes -= 60;
                degrees++;
            }

            return new DMS() { Degrees = degrees, Minutes = minutes, Seconds = seconds };
        }

        private DMS BearingSubtraction(double bearing1, double bearing2)
        {
            var dms1 = ParseBearing(bearing1);
            var dms2 = ParseBearing(bearing2);

            var degrees = dms1.Degrees - dms2.Degrees;
            var minutes = dms1.Minutes - dms2.Minutes;
            var seconds = dms1.Seconds - dms2.Seconds;

            //work out seconds first, carry over to minutes
            if (dms1.Seconds < dms2.Seconds)
            {
                minutes--;
                seconds += 60;
            }

            //work out minutes, carry over to degrees
            if (dms1.Minutes < dms2.Minutes)
            {
                degrees--;
                minutes += 60;
            }
            return new DMS() { Degrees = degrees, Minutes = minutes, Seconds = seconds };
        }

        public struct DMS
        {
            public int Degrees;
            public int Minutes;
            public int Seconds;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bearing"></param>
        /// <returns></returns>
        public DMS ParseBearing(string bearing)
        {
            if (bearing.Length == 8)
            {
                var degrees = Convert.ToInt32(bearing.Substring(0, 3));
                var minutes = Convert.ToInt32(bearing.Substring(4, 2));
                var seconds = Convert.ToInt32(bearing.Substring(6));
                return new DMS() { Degrees = degrees, Minutes = minutes, Seconds = seconds };
            } else if (bearing.Length == 6)
            {
                var degrees = Convert.ToInt32(bearing.Substring(0, 3));
                var minutes = Convert.ToInt32(bearing.Substring(4));
                return new DMS() { Degrees = degrees, Minutes = minutes };
            } else if (bearing.Length == 3)
            {
                var degrees = Convert.ToInt32(bearing);
                return new DMS() { Degrees = degrees };
            }
            return new DMS();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bearing"></param>
        /// <returns></returns>
        public DMS ParseBearing(double bearing)
        {
            var degrees = Convert.ToInt32(Math.Truncate(bearing));
            var minutes = Convert.ToInt32((bearing - degrees) * 100);
            var seconds = Convert.ToInt32((((bearing - degrees) * 100) - minutes) * 100);
            return new DMS() { Degrees = degrees, Minutes = minutes, Seconds = seconds };
        }

        private bool IsValid(DMS dMS)
        {
            return dMS.Degrees < 360 && dMS.Minutes < 60 && dMS.Seconds < 60;
        }

    }
}
