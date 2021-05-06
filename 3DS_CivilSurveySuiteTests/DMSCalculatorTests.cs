using _3DS_CivilSurveySuite.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace _3DS_CivilSurveySuiteTests
{
    [TestClass]
    public class DMSCalculatorTests
    {
        [TestMethod]
        public void Add_TwoDMSLessThan360Degrees_ReturnSum()
        {
            DMSCalc dMS1 = new DMSCalc() { Degrees = 50, Minutes = 10, Seconds = 10 };
            DMSCalc dMS2 = new DMSCalc() { Degrees = 50, Minutes = 10, Seconds = 10 };

            var result = dMS1 + dMS2;

            var expected = new DMSCalc() { Degrees = 100, Minutes = 20, Seconds = 20 };

            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void Add_TwoDMSGreaterThan360Degrees_ReturnSum()
        {
            DMSCalc dMS1 = new DMSCalc() { Degrees = 360, Minutes = 0, Seconds = 0 };
            DMSCalc dMS2 = new DMSCalc() { Degrees = 100, Minutes = 0, Seconds = 0 };

            var result = dMS1 + dMS2;

            var expected = new DMSCalc() { Degrees = 460, Minutes = 0, Seconds = 0 };

            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void Subtract_TwoDMSLessThan0Degrees_ReturnSum()
        {
            DMSCalc dMS1 = new DMSCalc() { Degrees = 10, Minutes = 0, Seconds = 0 };
            DMSCalc dMS2 = new DMSCalc() { Degrees = 100, Minutes = 0, Seconds = 0 };

            var result = dMS1 - dMS2;

            var expected = new DMSCalc() { Degrees = -90, Minutes = 0, Seconds = 0 };

            Assert.AreEqual(expected, result);
        }



    }

}
