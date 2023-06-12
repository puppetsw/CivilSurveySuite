using System.Threading;
using Autodesk.AutoCAD.DatabaseServices;
using CivilSurveySuite.ACAD;
using NUnit.Framework;

namespace CivilSurveySuiteAcadCoreTests
{
    [TestFixture, Apartment(ApartmentState.STA)]
    public class LineTypeTests : TestBase
    {
        [Test]
        public void Test_Load_LineType()
        {
            var result = false;

            var lineTypeName = "DASHED";

            void TestAction(Database db, Transaction tr)
            {
                LineTypeUtils.LoadLineType(lineTypeName);
                result = LineTypeUtils.IsLineTypeLoaded(lineTypeName);
            }

            ExecuteTestActions(null, TestAction);

            Assert.IsTrue(result);
        }
    }
}