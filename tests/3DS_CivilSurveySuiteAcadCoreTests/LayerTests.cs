using System.Threading;
using Autodesk.AutoCAD.DatabaseServices;
using CivilSurveySuite.ACAD;
using NUnit.Framework;

namespace CivilSurveySuiteAcadCoreTests
{
    [TestFixture, Apartment(ApartmentState.STA)]
    public class LayerTests : TestBase
    {
        [Test]
        public void Test_Create_Layer()
        {
            var result = false;

            void TestAction(Database db, Transaction tr)
            {
                LayerUtils.CreateLayer("Test Layer", tr);
                result = LayerUtils.HasLayer("Test Layer", tr);
            }

            ExecuteTestActions(null, TestAction);

            Assert.IsTrue(result);
        }
    }
}