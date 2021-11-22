using _3DS_CivilSurveySuite.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace _3DS_CivilSurveySuiteTests
{
    [TestClass]
    public class CivilSurfaceTests
    {

        [TestMethod]
        public void Property_ObjectId_StoresCorrectly()
        {
            var cs = new CivilSurface();
            cs.ObjectId = "TestId";

            Assert.AreEqual("TestId", cs.ObjectId);
        }

        [TestMethod]
        public void Property_Name_StoresCorrectly()
        {
            var cs = new CivilSurface();
            cs.Name = "TestName";

            Assert.AreEqual("TestName", cs.Name);
        }

        [TestMethod]
        public void Property_Description_StoresCorrectly()
        {
            var cs = new CivilSurface();
            cs.Description = "TestDescription";

            Assert.AreEqual("TestDescription", cs.Description);
        }

        [TestMethod]
        public void CivilPointGroup_Equality()
        {
            var cs1 = new CivilSurface();
            var cs2 = new CivilSurface();


            Assert.IsTrue(cs1.Equals(cs2) && cs2.Equals(cs1));

            Assert.IsTrue(cs1.GetHashCode() == cs2.GetHashCode());

        }

        [TestMethod]
        public void CivilPointGroup_Equality_SameInstance()
        {
            var cs1 = new CivilSurface();

            Assert.IsTrue(cs1.Equals(cs1));
        }

        [TestMethod]
        public void CivilPointGroup_Equality_Null()
        {
            var cs1 = new CivilSurface();

            Assert.IsFalse(cs1.Equals(null));
        }

        [TestMethod]
        public void CivilPointGroup_Equality_Object_ShouldBeFalse()
        {
            var cs1 = new CivilSurface();
            object testObject = new object();

            Assert.IsFalse(Equals(cs1, testObject));
            // ReSharper disable once PossibleUnintendedReferenceComparison
            Assert.IsFalse(testObject == cs1);
            Assert.IsFalse(cs1.Equals(testObject));
        }


    }
}