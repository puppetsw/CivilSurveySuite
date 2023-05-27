using System.Collections.Generic;
using CivilSurveySuite.Shared.Models;
using NUnit.Framework;

namespace _3DS_CivilSurveySuiteTests
{
    [TestFixture]
    public class CivilObjectEqualityTests
    {
        [Test]
        public void Alignment_Equality()
        {
            var obj1 = new CivilAlignment { Name = "Test" };
            var obj2 = new CivilAlignment { Name = "Test" };
            var list = new List<CivilAlignment>();
            list.Add(obj1);

            var result = list.Contains(obj2);
            Assert.IsTrue(obj1.Equals(obj2));
            Assert.IsTrue(result);
            Assert.IsTrue(obj1.GetHashCode() == obj2.GetHashCode());
        }

        [Test]
        public void Alignment_Equality_Is_Null()
        {
            var obj1 = new CivilAlignment();
            Assert.IsFalse(obj1.Equals(null));
        }

        [Test]
        public void CivilProfile_Equality()
        {
            var obj1 = new CivilProfile { Name = "Test" };
            var obj2 = new CivilProfile { Name = "Test" };
            var list = new List<CivilProfile>();
            list.Add(obj1);

            var result = list.Contains(obj2);
            Assert.IsTrue(obj1.Equals(obj1));
            Assert.IsTrue(result);
            Assert.IsTrue(obj1.GetHashCode() == obj2.GetHashCode());
        }

        [Test]
        public void CivilProfile_Equality_Is_Null()
        {
            var obj1 = new CivilProfile();
            Assert.IsFalse(obj1.Equals(null));
        }

        [Test]
        public void CivilProfile_Equality_As_Object()
        {
            var obj1 = new CivilProfile { Name = "Test" };
            var obj2 = new CivilProfile { Name = "Test" };

            var temp = obj2 as object;

            Assert.IsTrue(obj1.Equals(temp));
        }

        [Test]
        public void CivilSite_Equality()
        {
            var obj1 = new CivilSite { Name = "Test" };
            var obj2 = new CivilSite { Name = "Test" };
            var list = new List<CivilSite>();
            list.Add(obj1);

            var result = list.Contains(obj2);
            Assert.IsTrue(obj1.Equals(obj1));
            Assert.IsTrue(result);
            Assert.IsTrue(obj1.GetHashCode() == obj2.GetHashCode());
        }

        [Test]
        public void CivilSite_Equality_Is_Null()
        {
            var obj1 = new CivilSite();
            Assert.IsFalse(obj1.Equals(null));
        }

        [Test]
        public void CivilSite_Equality_As_Object()
        {
            var obj1 = new CivilSite { Name = "Test" };
            var obj2 = new CivilSite { Name = "Test" };

            var temp = obj2 as object;

            Assert.IsTrue(obj1.Equals(temp));
        }

    }
}