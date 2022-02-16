// Copyright Scott Whitney. All Rights Reserved.
// Reproduction or transmission in whole or in part, any form or by any
// means, electronic, mechanical or otherwise, is prohibited without the
// prior written consent of the copyright owner.
//
// Filename: TraverseTests.cs
// Date:     01/07/2021
// Author:   scott

using _3DS_CivilSurveySuite.UI.Models;
using NUnit.Framework;

namespace _3DS_CivilSurveySuiteTests
{
    [TestFixture]
    public class TraverseTests
    {
        [Test]
        public void TestParsingBearing8Length()
        {
            var expectedResult = new Angle { Degrees = 354, Minutes = 50, Seconds = 20 };
            var testString = "354.5020";
            var angle = new Angle(testString);

            Assert.AreEqual(expectedResult, angle);
        }

        [Test]
        public void TestParsingBearing6Length()
        {
            var expectedResult = new Angle() { Degrees = 354, Minutes = 50, Seconds = 0 };
            var testString = "354.50";
            var angle = new Angle(testString);
            Assert.AreEqual(expectedResult, angle);
        }

        [Test]
        public void TestParsingBearing3Length()
        {
            var expectedResult = new Angle() { Degrees = 354, Minutes = 0, Seconds = 0 };
            var testString = "354";
            var angle = new Angle(testString);
            Assert.AreEqual(expectedResult, angle);
        }

        [Test]
        public void TestParsingBearing8LengthDouble()
        {
            var expectedResult = new Angle() { Degrees = 354, Minutes = 50, Seconds = 20 };
            double testString = 354.5020;
            var angle = new Angle(testString);
            Assert.AreEqual(expectedResult, angle);
        }

        [Test]
        public void TestParsingBearing6LengthDouble()
        {
            var expectedResult = new Angle() { Degrees = 354, Minutes = 50, Seconds = 0 };
            double testString = 354.50;
            var angle = new Angle(testString);
            Assert.AreEqual(expectedResult, angle);
        }

        [Test]
        public void TestParsingBearing3LengthDouble()
        {
            var expectedResult = new Angle() { Degrees = 354, Minutes = 0, Seconds = 0 };
            double testString = 354;
            var angle = new Angle(testString);
            Assert.AreEqual(expectedResult, angle);
        }

        [Test]
        public void TestParsingBearing2LengthDouble()
        {
            var expectedResult = new Angle() { Degrees = 54, Minutes = 0, Seconds = 0 };
            double testString = 54;
            var angle = new Angle(testString);
            Assert.AreEqual(expectedResult, angle);
        }

        [Test]
        public void TestParsingBearing4LengthDouble()
        {
            var expectedResult = new Angle() { Degrees = 54, Minutes = 20, Seconds = 0 };
            double testString = 54.20;
            Assert.AreEqual(expectedResult, Angle.Parse(testString));
        }

        [Test]
        public void TestParsingBearing3ShortLengthDouble()
        {
            var expectedResult = new Angle() { Degrees = 5, Minutes = 20, Seconds = 0 };
            double testString = 5.20;
            var angle = new Angle(testString);
            Assert.AreEqual(expectedResult, angle);
        }

        [Test]
        public void TestBearingAddition()
        {
            double bearing1 = 25.4538;
            double bearing2 = 40.1747;

            double expectedResultDegrees = 66;
            double expectedResultMinutes = 3;
            double expectedResultSeconds = 25;

            Angle result = Angle.Add(bearing1, bearing2);

            Assert.AreEqual(expectedResultDegrees, result.Degrees);
            Assert.AreEqual(expectedResultMinutes, result.Minutes);
            Assert.AreEqual(expectedResultSeconds, result.Seconds);
        }

        [Test]
        public void TestBearingAddition2()
        {
            double bearing1 = 90.30;
            double bearing2 = 90.30;

            double expectedResultDegrees = 181;
            double expectedResultMinutes = 0;
            double expectedResultSeconds = 0;

            Angle result = Angle.Add(bearing1, bearing2);

            Assert.AreEqual(expectedResultDegrees, result.Degrees);
            Assert.AreEqual(expectedResultMinutes, result.Minutes);
            Assert.AreEqual(expectedResultSeconds, result.Seconds);
        }

        [Test]
        public void TestBearingSubtraction()
        {
            double bearing1 = 85.1537;
            double bearing2 = 46.2245;

            double expectedResultDegrees = 38;
            double expectedResultMinutes = 52;
            double expectedResultSeconds = 52;

            Angle result = Angle.Subtract(bearing1, bearing2);

            Assert.AreEqual(expectedResultDegrees, result.Degrees);
            Assert.AreEqual(expectedResultMinutes, result.Minutes);
            Assert.AreEqual(expectedResultSeconds, result.Seconds);
        }

        [Test]
        public void TestBearingSubtraction2()
        {
            const double bearing2 = 84.5020;
            const double bearing1 = 180;

            double expectedResultDegrees = 95;
            double expectedResultMinutes = 9;
            double expectedResultSeconds = 40;

            Angle result = Angle.Subtract(bearing1, bearing2);

            Assert.AreEqual(expectedResultDegrees, result.Degrees);
            Assert.AreEqual(expectedResultMinutes, result.Minutes);
            Assert.AreEqual(expectedResultSeconds, result.Seconds);
        }

        [Test]
        public void TestValidBearing()
        {
            const bool expectedResult = true;
            const double bearing = 354.5020;

            Angle dms = new Angle(bearing);

            bool actual = Angle.IsValid(dms);

            Assert.AreEqual(expectedResult, actual);
        }

        [Test]
        public void TestInvalidBearing()
        {
            const bool expectedResult = false;
            const double bearing = 374.5020;

            Angle dms = new Angle(bearing);

            bool actual = Angle.IsValid(dms);

            Assert.AreEqual(expectedResult, actual);
        }

        [Test]
        public void TestInvalidBearing2()
        {
            const double bearing = 354.6120;

            Angle dms = new Angle(bearing);

            bool actual = Angle.IsValid(dms);

            Assert.AreEqual(false, actual);
        }
    }
}