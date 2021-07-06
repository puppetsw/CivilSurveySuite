// Copyright Scott Whitney. All Rights Reserved.
// Reproduction or transmission in whole or in part, any form or by any
// means, electronic, mechanical or otherwise, is prohibited without the
// prior written consent of the copyright owner.
// 
// Filename: BearingTests.cs
// Date:     01/07/2021
// Author:   scott

using System;
using System.Linq;
using _3DS_CivilSurveySuite.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace _3DS_CivilSurveySuiteTests
{
    [TestClass]
    public class BearingTests
    {
        [TestMethod]
        public void Test_TwoDMSAreEqual_True()
        {
            var dms1 = new Angle { Degrees = 22, Minutes = 18, Seconds = 13 };
            var dms2 = new Angle { Degrees = 22, Minutes = 18, Seconds = 13 };

            Assert.AreEqual(dms1, dms2);
        }

        [TestMethod]
        public void Test_TwoDMSAreNotEqual_True()
        {
            var dms1 = new Angle { Degrees = 22, Minutes = 18, Seconds = 10 };
            var dms2 = new Angle { Degrees = 22, Minutes = 18, Seconds = 13 };

            Assert.AreNotEqual(dms1, dms2);
        }

        [TestMethod]
        public void TestAddingBearing1()
        {
            var dms1 = new Angle { Degrees = 22, Minutes = 18, Seconds = 13 };
            var dms2 = new Angle { Degrees = 10, Minutes = 11, Seconds = 25 };
            var expectedDMS = new Angle { Degrees = 32, Minutes = 29, Seconds = 38 };

            Assert.AreEqual(expectedDMS, dms1 + dms2);
        }

        [TestMethod]
        public void TestSubtractingBearing1()
        {
            var dms1 = new Angle { Degrees = 22, Minutes = 18, Seconds = 13 };
            var dms2 = new Angle { Degrees = 10, Minutes = 11, Seconds = 25 };
            var expectedDMS = new Angle { Degrees = 12, Minutes = 6, Seconds = 48 };

            Assert.AreEqual(expectedDMS, dms1 - dms2);
        }

        [TestMethod]
        public void TestStrip1()
        {
            const string bearing = "354°20'50\"";
            const string expectedBearing = "354.2050";

            string result = StripDMSSymbols(bearing);

            Assert.AreEqual(expectedBearing, result);
        }

        [TestMethod]
        public void TestStrip2()
        {
            const string bearing = "scott354°20'50\"";
            const string expectedBearing = "354.2050";

            string result = StripDMSSymbols(bearing);

            Assert.AreEqual(expectedBearing, result);
        }

        [TestMethod]
        public void TestStrip3()
        {
            const string bearing = "scott354°°20'50\"°°'''''";
            const string expectedBearing = "354.2050";

            string result = StripDMSSymbols(bearing);

            Assert.AreEqual(expectedBearing, result);
        }

        [TestMethod]
        public void OppositeAngleTest()
        {
            // (alpha + 180) % 360
            double testAngle = 84.5020;
            double oppositeAngle = 95.0940;

            var dmsResult = TraverseTests.BearingSubtraction(180, testAngle);
            var result = Math.Round(dmsResult.Degrees + ((double) dmsResult.Minutes / 100) + ((double) dmsResult.Seconds / 10000), 4);

            Assert.AreEqual(oppositeAngle, result);
        }

        private static string StripDMSSymbols(string bearingWithSymbols)
        {
            //check if we have symbols?
            //TODO: what if only one symbol?
            string cleanedString = ReplaceFirst(bearingWithSymbols, "°", ".");
            return RemoveAlphaCharacters(cleanedString);
        }

        private static string ReplaceFirst(string text, string search, string replace)
        {
            int pos = text.IndexOf(search, StringComparison.Ordinal);

            if (pos < 0)
            {
                return text;
            }

            return text.Substring(0, pos) + replace + text.Substring(pos + search.Length);
        }

        private static string RemoveAlphaCharacters(string source)
        {
            var numbers = new[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
            var chars = new[] { '.', };

            return new string(source
                .Where(x => numbers.Contains(x) || chars.Contains(x))
                .ToArray()).Trim(chars);
        }
    }
}