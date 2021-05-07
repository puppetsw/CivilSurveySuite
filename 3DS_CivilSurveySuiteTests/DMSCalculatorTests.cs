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
            DMS dMS1 = new DMS() { Degrees = 50, Minutes = 10, Seconds = 10 };
            DMS dMS2 = new DMS() { Degrees = 50, Minutes = 10, Seconds = 10 };

            var result = DMS.Add(dMS1, dMS2);

            var expected = new DMS() { Degrees = 100, Minutes = 20, Seconds = 20 };

            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void Add_TwoDMSGreaterThan360Degrees_ReturnSum()
        {
            DMS dMS1 = new DMS() { Degrees = 360, Minutes = 0, Seconds = 0 };
            DMS dMS2 = new DMS() { Degrees = 100, Minutes = 0, Seconds = 0 };

            var result = DMS.Add(dMS1, dMS2);

            var expected = new DMS() { Degrees = 460, Minutes = 0, Seconds = 0 };

            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void Subtract_TwoDMSLessThan0Degrees_ReturnSum()
        {
            DMS dMS1 = new DMS() { Degrees = 10, Minutes = 0, Seconds = 0 };
            DMS dMS2 = new DMS() { Degrees = 100, Minutes = 0, Seconds = 0 };

            var result = DMS.Subtract(dMS1, dMS2);

            var expected = new DMS() { Degrees = -90, Minutes = 0, Seconds = 0 };

            Assert.AreEqual(expected, result);
        }




    }

}
