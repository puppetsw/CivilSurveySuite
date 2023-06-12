// Copyright Scott Whitney. All Rights Reserved.
// Reproduction or transmission in whole or in part, any form or by any
// means, electronic, mechanical or otherwise, is prohibited without the
// prior written consent of the copyright owner.

using CivilSurveySuite.Common.Models;
using NUnit.Framework;

namespace CivilSurveySuiteTests
{
    [TestFixture]
    public class CivilPointTests
    {
        [Test]
        public void Property_PointNumber_StoresCorrectly()
        {
            var civilPoint = new CivilPoint();
            civilPoint.PointNumber = 1000;

            Assert.AreEqual((uint)1000, civilPoint.PointNumber);
        }

        [Test]
        public void Property_Easting_StoresCorrectly()
        {
            var civilPoint = new CivilPoint();
            civilPoint.Easting = 1000;

            Assert.AreEqual(1000, civilPoint.Easting);
        }

        [Test]
        public void Property_Northing_StoresCorrectly()
        {
            var civilPoint = new CivilPoint();
            civilPoint.Northing = 1000;

            Assert.AreEqual(1000, civilPoint.Northing);
        }

        [Test]
        public void Property_Elevation_StoresCorrectly()
        {
            var civilPoint = new CivilPoint();
            civilPoint.Elevation = 1000;

            Assert.AreEqual(1000, civilPoint.Elevation);
        }

        [Test]
        public void Property_RawDescription_StoresCorrectly()
        {
            var civilPoint = new CivilPoint();
            civilPoint.RawDescription = "Test";

            Assert.AreEqual("Test", civilPoint.RawDescription);
        }

        [Test]
        public void Property_DescriptionFormat_StoresCorrectly()
        {
            var civilPoint = new CivilPoint();
            civilPoint.DescriptionFormat = "Test";

            Assert.AreEqual("Test", civilPoint.DescriptionFormat);
        }

        [Test]
        public void Property_ObjectIdHandle_StoresCorrectly()
        {
            var civilPoint = new CivilPoint();
            civilPoint.ObjectId = "Test";

            Assert.AreEqual("Test", civilPoint.ObjectId);
        }

        [Test]
        public void Property_PointName_StoresCorrectly()
        {
            var civilPoint = new CivilPoint();
            civilPoint.PointName = "Test";

            Assert.AreEqual("Test", civilPoint.PointName);
        }

        [Test]
        public void CivilPoint_Equality()
        {
            var civilPoint1 = new CivilPoint() { ObjectId = "Test" } ;
            var civilPoint2 = new CivilPoint() { ObjectId = "Test" } ;

            Assert.IsTrue(civilPoint1.Equals(civilPoint2));
            Assert.IsTrue(civilPoint2.Equals(civilPoint1));

            Assert.IsTrue(civilPoint1.GetHashCode() == civilPoint2.GetHashCode());

        }

        [Test]
        public void CivilPoint_Equality_SameInstance()
        {
            var civilPoint1 = new CivilPoint();

            Assert.IsTrue(civilPoint1.Equals(civilPoint1));
        }

        [Test]
        public void CivilPoint_Equality_Null()
        {
            var civilPoint1 = new CivilPoint();

            Assert.IsFalse(civilPoint1.Equals(null));
        }

        [Test]
        public void CivilPoint_Equality_Object_ShouldBeFalse()
        {
            var civilPoint1 = new CivilPoint();
            object testObject = new object();

            Assert.IsFalse(Equals(civilPoint1, testObject));
            // ReSharper disable once PossibleUnintendedReferenceComparison
            Assert.IsFalse(testObject == civilPoint1);
            Assert.IsFalse(civilPoint1.Equals(testObject));
        }
    }
}