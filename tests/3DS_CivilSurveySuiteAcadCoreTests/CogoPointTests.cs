﻿using System.Threading;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.Civil.DatabaseServices;
using CivilSurveySuite.CIVIL;
using NUnit.Framework;

namespace CivilSurveySuiteAcadCoreTests
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