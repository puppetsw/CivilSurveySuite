// Copyright Scott Whitney. All Rights Reserved.
// Reproduction or transmission in whole or in part, any form or by any
// means, electronic, mechanical or otherwise, is prohibited without the
// prior written consent of the copyright owner.
// 
// Filename: TraverseTests.cs
// Date:     01/07/2021
// Author:   scott

using System;
using _3DS_CivilSurveySuite.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace _3DS_CivilSurveySuiteTests
{
    [TestClass]
    public class TraverseTests
    {
        [TestMethod]
        public void TestParsingBearing8Length()
        {
            var expectedResult = new Angle() { Degrees = 354, Minutes = 50, Seconds = 20 };
            var testString = "354.5020";
            Assert.AreEqual(expectedResult, ParseBearing(testString));
        }

        [TestMethod]
        public void TestParsingBearing6Length()
        {
            var expectedResult = new Angle() { Degrees = 354, Minutes = 50, Seconds = 0 };
            var testString = "354.50";
            Assert.AreEqual(expectedResult, ParseBearing(testString));
        }

        [TestMethod]
        public void TestParsingBearing3Length()
        {
            var expectedResult = new Angle() { Degrees = 354, Minutes = 0, Seconds = 0 };
            var testString = "354";
            Assert.AreEqual(expectedResult, ParseBearing(testString));
        }

        [TestMethod]
        public void TestParsingBearing8LengthDouble()
        {
            var expectedResult = new Angle() { Degrees = 354, Minutes = 50, Seconds = 20 };
            double testString = 354.5020;
            Assert.AreEqual(expectedResult, ParseBearing(testString));
        }

        [TestMethod]
        public void TestParsingBearing6LengthDouble()
        {
            var expectedResult = new Angle() { Degrees = 354, Minutes = 50, Seconds = 0 };
            double testString = 354.50;
            Assert.AreEqual(expectedResult, ParseBearing(testString));
        }

        [TestMethod]
        public void TestParsingBearing3LengthDouble()
        {
            var expectedResult = new Angle() { Degrees = 354, Minutes = 0, Seconds = 0 };
            double testString = 354;
            Assert.AreEqual(expectedResult, ParseBearing(testString));
        }

        [TestMethod]
        public void TestParsingBearing2LengthDouble()
        {
            var expectedResult = new Angle() { Degrees = 54, Minutes = 0, Seconds = 0 };
            double testString = 54;
            Assert.AreEqual(expectedResult, ParseBearing(testString));
        }

        [TestMethod]
        public void TestParsingBearing4LengthDouble()
        {
            var expectedResult = new Angle() { Degrees = 54, Minutes = 20, Seconds = 0 };
            double testString = 54.20;
            Assert.AreEqual(expectedResult, ParseBearing(testString));
        }

        [TestMethod]
        public void TestParsingBearing3ShortLengthDouble()
        {
            var expectedResult = new Angle() { Degrees = 5, Minutes = 20, Seconds = 0 };
            double testString = 5.20;
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

            Angle result = BearingAddition(bearing1, bearing2);

            Assert.AreEqual(expectedResultDegrees, result.Degrees);
            Assert.AreEqual(expectedResultMinutes, result.Minutes);
            Assert.AreEqual(expectedResultSeconds, result.Seconds);
        }

        [TestMethod]
        public void TestBearingAddition2()
        {
            double bearing1 = 90.30;
            double bearing2 = 90.30;

            double expectedResultDegrees = 181;
            double expectedResultMinutes = 0;
            double expectedResultSeconds = 0;

            Angle result = BearingAddition(bearing1, bearing2);

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
            double expectedResultMinutes = 52;
            double expectedResultSeconds = 52;

            Angle result = BearingSubtraction(bearing1, bearing2);

            Assert.AreEqual(expectedResultDegrees, result.Degrees);
            Assert.AreEqual(expectedResultMinutes, result.Minutes);
            Assert.AreEqual(expectedResultSeconds, result.Seconds);
        }

        [TestMethod]
        public void TestBearingSubtraction2()
        {
            const double bearing2 = 84.5020;
            const double bearing1 = 180;

            double expectedResultDegrees = 95;
            double expectedResultMinutes = 9;
            double expectedResultSeconds = 40;

            Angle result = BearingSubtraction(bearing1, bearing2);

            Assert.AreEqual(expectedResultDegrees, result.Degrees);
            Assert.AreEqual(expectedResultMinutes, result.Minutes);
            Assert.AreEqual(expectedResultSeconds, result.Seconds);
        }

        [TestMethod]
        public void TestValidBearing()
        {
            const bool expectedResult = true;
            const double bearing = 354.5020;

            Angle dms = ParseBearing(bearing);

            bool actual = IsValid(dms);

            Assert.AreEqual(expectedResult, actual);
        }

        [TestMethod]
        public void TestInvalidBearing()
        {
            const bool expectedResult = false;
            const double bearing = 374.5020;

            Angle dms = ParseBearing(bearing);

            bool actual = IsValid(dms);

            Assert.AreEqual(expectedResult, actual);
        }

        [TestMethod]
        public void TestInvalidBearing2()
        {
            const double bearing = 354.6120;

            Angle dms = ParseBearing(bearing);

            bool actual = IsValid(dms);

            Assert.AreEqual(false, actual);
        }

        [TestMethod]
        public void TestFeetConversion1()
        {
            double feetExpect = 47.244;
            double inchExpect = 0.1524;
            double expectedResult = feetExpect + inchExpect;

            double result = ConvertFeetToMeters(155.06);

            Assert.AreEqual(expectedResult, Math.Round(result, 4));
        }

        [TestMethod]
        public void TestFeetConversion2()
        {
            double feetExpect = 47.244;
            double inchExpect = 0.254;
            double expectedResult = feetExpect + inchExpect;

            double result = ConvertFeetToMeters(155.10);

            Assert.AreEqual(expectedResult, Math.Round(result, 4));
        }

        [TestMethod]
        public void TestLinkConversion1()
        {
            double expected = 20.1168;
            double link = 100;

            double result = ConvertLinkToMeters(link);

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

            double feet = Math.Truncate(feetAndInches) * feetConversion;
            double inch1 = feetAndInches - Math.Truncate(feetAndInches);
            double inch2 = (inch1 * 100) * inchConversion;

            return feet + inch2;
        }

        private Angle BearingAddition(double bearing1, double bearing2)
        {
            Angle dms1 = ParseBearing(bearing1);
            Angle dms2 = ParseBearing(bearing2);

            int degrees = dms1.Degrees + dms2.Degrees;
            int minutes = dms1.Minutes + dms2.Minutes;
            int seconds = dms1.Seconds + dms2.Seconds;

            //work out seconds first, carry over to minutes
            if (seconds >= 60)
            {
                seconds -= 60;
                minutes++;
            }

            //work out minutes, carry over to degrees
            if (minutes >= 60)
            {
                minutes -= 60;
                degrees++;
            }

            return new Angle() { Degrees = degrees, Minutes = minutes, Seconds = seconds };
        }

        public static Angle BearingSubtraction(double bearing1, double bearing2)
        {
            Angle dms1 = ParseBearing(bearing1);
            Angle dms2 = ParseBearing(bearing2);

            int degrees = dms1.Degrees - dms2.Degrees;
            int minutes = dms1.Minutes - dms2.Minutes;
            int seconds = dms1.Seconds - dms2.Seconds;

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

            return new Angle() { Degrees = degrees, Minutes = minutes, Seconds = seconds };
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bearing"></param>
        /// <returns></returns>
        public static Angle ParseBearing(string bearing)
        {
            if (bearing.Length == 8)
            {
                var degrees = Convert.ToInt32(bearing.Substring(0, 3));
                var minutes = Convert.ToInt32(bearing.Substring(4, 2));
                var seconds = Convert.ToInt32(bearing.Substring(6));
                return new Angle() { Degrees = degrees, Minutes = minutes, Seconds = seconds };
            }
            else if (bearing.Length == 6)
            {
                var degrees = Convert.ToInt32(bearing.Substring(0, 3));
                var minutes = Convert.ToInt32(bearing.Substring(4));
                return new Angle() { Degrees = degrees, Minutes = minutes };
            }
            else if (bearing.Length == 3)
            {
                var degrees = Convert.ToInt32(bearing);
                return new Angle() { Degrees = degrees };
            }

            return new Angle();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bearing"></param>
        /// <returns></returns>
        public static Angle ParseBearing(double bearing)
        {
            var degrees = Convert.ToInt32(Math.Truncate(bearing));
            var minutes = Convert.ToInt32((bearing - degrees) * 100);
            var seconds = Convert.ToInt32((((bearing - degrees) * 100) - minutes) * 100);
            return new Angle() { Degrees = degrees, Minutes = minutes, Seconds = seconds };
        }

        private bool IsValid(Angle dms)
        {
            return dms.Degrees < 360 && dms.Minutes < 60 && dms.Seconds < 60;
        }
    }
}