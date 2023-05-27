using CivilSurveySuite.Shared.Models;
using NUnit.Framework;

namespace _3DS_CivilSurveySuiteTests
{
    [TestFixture]
    public class CivilPointGroupTests
    {
        [Test]
        public void Property_ObjectId_StoresCorrectly()
        {
            var cpg = new CivilPointGroup();
            cpg.ObjectId = "TestId";

            Assert.AreEqual("TestId", cpg.ObjectId);
        }

        [Test]
        public void Property_Name_StoresCorrectly()
        {
            var cpg = new CivilPointGroup();
            cpg.Name = "TestName";

            Assert.AreEqual("TestName", cpg.Name);
        }

        [Test]
        public void Property_Description_StoresCorrectly()
        {
            var cpg = new CivilPointGroup();
            cpg.Description = "TestDescription";

            Assert.AreEqual("TestDescription", cpg.Description);
        }

        [Test]
        public void CivilPointGroup_Equality()
        {
            var cpg1 = new CivilPointGroup();
            var cpg2 = new CivilPointGroup();

            Assert.IsTrue(cpg1.Equals(cpg2));
            Assert.IsTrue(cpg2.Equals(cpg1));

            Assert.IsTrue(cpg1.GetHashCode() == cpg2.GetHashCode());

        }

        [Test]
        public void CivilPointGroup_Equality_SameInstance()
        {
            var cpg1 = new CivilPointGroup();

            Assert.IsTrue(cpg1.Equals(cpg1));
        }

        [Test]
        public void CivilPointGroup_Equality_Null()
        {
            var cpg1 = new CivilPointGroup();

            Assert.IsFalse(cpg1.Equals(null));
        }

        [Test]
        public void CivilPointGroup_Equality_Object_ShouldBeFalse()
        {
            var cpg1 = new CivilPointGroup();
            object testObject = new object();

            Assert.IsFalse(Equals(cpg1, testObject));
            // ReSharper disable once PossibleUnintendedReferenceComparison
            Assert.IsFalse(testObject == cpg1);
            Assert.IsFalse(cpg1.Equals(testObject));
        }

    }
}