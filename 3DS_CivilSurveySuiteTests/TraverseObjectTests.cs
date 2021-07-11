using _3DS_CivilSurveySuite.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace _3DS_CivilSurveySuiteTests
{
    [TestClass]
    public class TraverseObjectTests
    {
        [TestMethod]
        public void TraverseObject_New_SetBearing_Invalid()
        {
            var traverseObject = new TraverseObject();
            traverseObject.Bearing = 400; //Invalid Bearing

            var expected = new Angle();
            Assert.AreEqual(expected.ToDouble(), traverseObject.Bearing);
        }
    }
}