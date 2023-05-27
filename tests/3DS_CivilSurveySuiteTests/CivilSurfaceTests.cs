using CivilSurveySuite.Shared.Models;
using NUnit.Framework;

namespace CivilSurveySuiteTests
{
    [TestFixture]
    public class CivilSurfaceTests
    {

        [Test]
        public void Property_ObjectId_StoresCorrectly()
        {
            var cs = new CivilSurface();
            cs.ObjectId = "TestId";

            Assert.AreEqual("TestId", cs.ObjectId);
        }

        [Test]
        public void Property_Name_StoresCorrectly()
        {
            var cs = new CivilSurface();
            cs.Name = "TestName";

            Assert.AreEqual("TestName", cs.Name);
        }

        [Test]
        public void Property_Description_StoresCorrectly()
        {
            var cs = new CivilSurface();
            cs.Description = "TestDescription";

            Assert.AreEqual("TestDescription", cs.Description);
        }

        [Test]
        public void CivilPointGroup_Equality()
        {
            var cs1 = new CivilSurface();
            var cs2 = new CivilSurface();

            Assert.IsTrue(cs1.Equals(cs2));
            Assert.IsTrue(cs2.Equals(cs1));
            Assert.IsTrue(cs1.GetHashCode() == cs2.GetHashCode());

        }

        [Test]
        public void CivilPointGroup_Equality_SameInstance()
        {
            var cs1 = new CivilSurface();

            Assert.IsTrue(cs1.Equals(cs1));
        }

        [Test]
        public void CivilPointGroup_Equality_Null()
        {
            var cs1 = new CivilSurface();

            Assert.IsFalse(cs1.Equals(null));
        }

        [Test]
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