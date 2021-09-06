// Copyright Scott Whitney. All Rights Reserved.
// Reproduction or transmission in whole or in part, any form or by any
// means, electronic, mechanical or otherwise, is prohibited without the
// prior written consent of the copyright owner.

using _3DS_CivilSurveySuite.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace _3DS_CivilSurveySuiteTests
{
    [TestClass]
    public class CivilPointTests
    {
        [TestMethod]
        public void Property_PointNumber_StoresCorrectly()
        {
            var civilPoint = new CivilPoint();
            civilPoint.PointNumber = 1000;
            
            Assert.AreEqual((uint)1000, civilPoint.PointNumber);
        }

        [TestMethod]
        public void Property_Easting_StoresCorrectly()
        {
            var civilPoint = new CivilPoint();
            civilPoint.Easting = 1000;
            
            Assert.AreEqual(1000, civilPoint.Easting);
        }

        [TestMethod]
        public void Property_Northing_StoresCorrectly()
        {
            var civilPoint = new CivilPoint();
            civilPoint.Northing = 1000;
            
            Assert.AreEqual(1000, civilPoint.Northing);
        }

        [TestMethod]
        public void Property_Elevation_StoresCorrectly()
        {
            var civilPoint = new CivilPoint();
            civilPoint.Elevation = 1000;
            
            Assert.AreEqual(1000, civilPoint.Elevation);
        }

        [TestMethod]
        public void Property_RawDescription_StoresCorrectly()
        {
            var civilPoint = new CivilPoint();
            civilPoint.RawDescription = "Test";
            
            Assert.AreEqual("Test", civilPoint.RawDescription);
        }

        [TestMethod]
        public void Property_DescriptionFormat_StoresCorrectly()
        {
            var civilPoint = new CivilPoint();
            civilPoint.DescriptionFormat = "Test";
            
            Assert.AreEqual("Test", civilPoint.DescriptionFormat);
        }

        [TestMethod]
        public void Property_ObjectIdHandle_StoresCorrectly()
        {
            var civilPoint = new CivilPoint();
            civilPoint.ObjectIdHandle = "Test";
            
            Assert.AreEqual("Test", civilPoint.ObjectIdHandle);
        }

        [TestMethod]
        public void Property_PointName_StoresCorrectly()
        {
            var civilPoint = new CivilPoint();
            civilPoint.PointName = "Test";
            
            Assert.AreEqual("Test", civilPoint.PointName);
        }

        [TestMethod]
        public void CivilPoint_Equality()
        {
            var civilPoint1 = new CivilPoint();
            var civilPoint2 = new CivilPoint();
           

            Assert.IsTrue(civilPoint1.Equals(civilPoint2) && civilPoint2.Equals(civilPoint1));

            Assert.IsTrue(civilPoint1.GetHashCode() == civilPoint2.GetHashCode());

        }

        [TestMethod]
        public void CivilPoint_Equality_SameInstance()
        {
            var civilPoint1 = new CivilPoint();

            Assert.IsTrue(civilPoint1.Equals(civilPoint1));
        }

        [TestMethod]
        public void CivilPoint_Equality_Null()
        {
            var civilPoint1 = new CivilPoint();

            Assert.IsFalse(civilPoint1.Equals(null));
        }

        [TestMethod]
        public void CivilPoint_Equality_Object_ShouldBeFalse()
        {
            var civilPoint1 = new CivilPoint();
            object testObject = new object();

            Assert.IsFalse(Equals(civilPoint1, testObject));
            Assert.IsFalse(testObject == civilPoint1);
            Assert.IsFalse(civilPoint1.Equals(testObject));
        }
    }
}