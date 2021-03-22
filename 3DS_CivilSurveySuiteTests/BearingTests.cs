using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using static _3DS_CivilSurveySuiteTests.TraverseTests;

namespace _3DS_CivilSurveySuiteTests
{
    [TestClass]
    public class BearingTests
    {
        [TestMethod]
        public void TestAddingBearing1()
        {
            DMS dms1 = new DMS() { Degrees = 22, Minutes = 18, Seconds = 13 };
            DMS dms2 = new DMS() { Degrees = 10, Minutes = 11, Seconds = 25 };
            DMS expectedDMS = new DMS() { Degrees = 32, Minutes = 29, Seconds = 38 };
        }

        [TestMethod]
        public void TestStrip1()
        {
            string bearing = "354°20'50\"";
            string expectedBearing = "354.2050";

            string result = StripDMSSymbols(bearing);

            Assert.AreEqual(expectedBearing, result);
        }

        [TestMethod]
        public void TestStrip2()
        {
            string bearing = "scott354°20'50\"";
            string expectedBearing = "354.2050";

            string result = StripDMSSymbols(bearing);

            Assert.AreEqual(expectedBearing, result);
        }

        [TestMethod]
        public void TestStrip3()
        {
            string bearing = "scott354°°20'50\"°°'''''";
            string expectedBearing = "354.2050";

            string result = StripDMSSymbols(bearing);

            Assert.AreEqual(expectedBearing, result);
        }

        [TestMethod]
        public void OppositeAngleTest()
        {
            // (alpha + 180) % 360
            double testAngle = 84.5020;
            double oppositeAngle = 95.0940;

            var dms1 = new DMS() { Degrees = 84, Minutes = 50, Seconds = 20 };
            var dms2 = new DMS() { Degrees = 180, Minutes = 0, Seconds = 0 };

            var dmsExpected = new DMS() { Degrees = 95, Minutes = 9, Seconds = 40 };

            var dmsResult = BearingSubtraction(180, testAngle);
            var result = Math.Round((double)dmsResult.Degrees + ((double)dmsResult.Minutes / 100) + ((double)dmsResult.Seconds / 10000), 4);

            Assert.AreEqual(oppositeAngle, result);

        }

        public static string StripDMSSymbols(string bearingWithSymbols)
        {
            //check if we have symbols?
            //TODO: what if only one symbol?
            string cleanedString = ReplaceFirst(bearingWithSymbols, "°", ".");
            cleanedString = RemoveAlphaCharacters(cleanedString);

            return cleanedString;
        }

        public static string ReplaceFirst(string text, string search, string replace)
        {
            int pos = text.IndexOf(search);
            if (pos < 0)
            {
                return text;
            }
            return text.Substring(0, pos) + replace + text.Substring(pos + search.Length);
        }

        public static string RemoveAlphaCharacters(string source)
        {
            var numbers = new[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
            var chars = new[] { '.', };

            return new string(source
                    .Where(x => numbers.Contains(x) || chars.Contains(x))
                    .ToArray()).Trim(chars);
        }
    }
}
