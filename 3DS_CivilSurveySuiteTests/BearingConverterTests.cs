using _3DS_CivilSurveySuite.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace _3DS_CivilSurveySuiteTests
{
    [TestClass]
    public class BearingConverterTests
    {
        [TestMethod]
        public void TestBearingString()
        {
            DMS dms = new DMS { Degrees = 225, Minutes = 21, Seconds = 57 };
            string bearing = dms.ToString();

        }
    }
}
