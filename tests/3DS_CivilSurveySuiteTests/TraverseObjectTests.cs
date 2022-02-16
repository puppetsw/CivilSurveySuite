using _3DS_CivilSurveySuite.UI.Models;
using NUnit.Framework;

namespace _3DS_CivilSurveySuiteTests
{
    [TestFixture]
    public class TraverseObjectTests
    {
        [Test]
        public void TraverseObject_New_SetBearing_Invalid()
        {
            var traverseObject = new TraverseObject();
            traverseObject.Bearing = 400; //Invalid Bearing

            var expected = new Angle();
            Assert.AreEqual(expected.ToDouble(), traverseObject.Bearing);
        }
    }
}