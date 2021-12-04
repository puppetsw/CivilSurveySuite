using System.Threading;
using _3DS_CivilSurveySuite.ACAD2017;
using Autodesk.AutoCAD.DatabaseServices;
using NUnit.Framework;

namespace _3DS_CivilSurveySuiteAcadTests
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