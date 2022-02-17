using System.Threading;
using _3DS_CivilSurveySuite.C3D2017;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.Civil.DatabaseServices;
using NUnit.Framework;

namespace _3DS_CivilSurveySuiteAcadCoreTests
{
    [TestFixture, Apartment(ApartmentState.STA)]
    public class CogoPointTests : TestBase
    {
        [Test]
        public void Test_Create_CogoPoint()
        {
            CogoPoint result = null;

            void TestAction(Database db, Transaction tr)
            {
                CogoPointUtils.CreatePoint(tr, Point3d.Origin);
                result = CogoPointUtils.GetByPointNumber(tr, 1);
            }

            ExecuteTestActions(null, TestAction);

            Assert.IsTrue(result != null);
        }
    }
}