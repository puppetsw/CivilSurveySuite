using System.Threading;
using CivilSurveySuite.ACAD;
using Autodesk.AutoCAD.DatabaseServices;
using NUnit.Framework;

namespace _3DS_CivilSurveySuiteAcadCoreTests
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