using CivilSurveySuite.Shared.Models;
using NUnit.Framework;

namespace CivilSurveySuiteTests
{
    [TestFixture]
    public class BearingConverterTests
    {
        [Test]
        public void Test_DMS_ToString()
        {
            var dms = new Angle { Degrees = 225, Minutes = 21, Seconds = 57 };
            string bearing = dms.ToString();

            const string expectedResult = "225°21'57\"";

            Assert.AreEqual(expectedResult, bearing);
        }

        [Test]
        public void Test_DMS_ToDouble()
        {
            var dms = new Angle { Degrees = 225, Minutes = 21, Seconds = 57 };
            double bearing = dms.ToDouble();

            const double expectedResult = 225.2157;

            Assert.AreEqual(expectedResult, bearing);
        }
    }
}