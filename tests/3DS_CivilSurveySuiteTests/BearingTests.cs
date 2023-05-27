using System;
using CivilSurveySuite.Shared.Models;
using NUnit.Framework;

namespace CivilSurveySuiteTests
{
    [TestFixture]
    public class BearingTests
    {
        [Test]
        public void Test_TwoDMSAreEqual_True()
        {
            var dms1 = new Angle { Degrees = 22, Minutes = 18, Seconds = 13 };
            var dms2 = new Angle { Degrees = 22, Minutes = 18, Seconds = 13 };

            Assert.AreEqual(dms1, dms2);
        }

        [Test]
        public void Test_TwoDMSAreNotEqual_True()
        {
            var dms1 = new Angle { Degrees = 22, Minutes = 18, Seconds = 10 };
            var dms2 = new Angle { Degrees = 22, Minutes = 18, Seconds = 13 };

            Assert.AreNotEqual(dms1, dms2);
        }

        [Test]
        public void TestAddingBearing1()
        {
            var dms1 = new Angle { Degrees = 22, Minutes = 18, Seconds = 13 };
            var dms2 = new Angle { Degrees = 10, Minutes = 11, Seconds = 25 };
            var expectedDMS = new Angle { Degrees = 32, Minutes = 29, Seconds = 38 };

            Assert.AreEqual(expectedDMS, dms1 + dms2);
        }

        [Test]
        public void TestSubtractingBearing1()
        {
            var dms1 = new Angle { Degrees = 22, Minutes = 18, Seconds = 13 };
            var dms2 = new Angle { Degrees = 10, Minutes = 11, Seconds = 25 };
            var expectedDMS = new Angle { Degrees = 12, Minutes = 6, Seconds = 48 };

            Assert.AreEqual(expectedDMS, dms1 - dms2);
        }

        [Test]
        public void OppositeAngleTest()
        {
            // (alpha + 180) % 360
            double testAngle = 84.5020;
            double oppositeAngle = 95.0940;

            var dmsResult = Angle.Subtract(180, testAngle);
            var result = Math.Round(dmsResult.Degrees + ((double) dmsResult.Minutes / 100) + ((double) dmsResult.Seconds / 10000), 4);

            Assert.AreEqual(oppositeAngle, result);
        }
    }
}