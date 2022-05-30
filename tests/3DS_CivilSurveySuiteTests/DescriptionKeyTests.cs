using _3DS_CivilSurveySuite.Shared.Models;
using NUnit.Framework;

namespace _3DS_CivilSurveySuiteTests
{
    [TestFixture]
    public class DescriptionKeyTests
    {
        [Test]
        public void Property_Description_StoresCorrectly()
        {
            var desKey = new DescriptionKey();
            desKey.Description = "TestId";

            Assert.AreEqual("TestId", desKey.Description);
        }

        [Test]
        public void Property_Key_StoresCorrectly()
        {
            var desKey = new DescriptionKey();
            desKey.Key = "TST";

            Assert.AreEqual("TST", desKey.Key);
        }

        [Test]
        public void Property_Layer_StoresCorrectly()
        {
            var desKey = new DescriptionKey();
            desKey.Layer = "TestLayer";

            Assert.AreEqual("TestLayer", desKey.Layer);
        }

        [Test]
        public void Property_Draw2D_StoresCorrectly()
        {
            var desKey = new DescriptionKey();
            desKey.Draw2D = true;

            Assert.IsTrue(desKey.Draw2D);

            desKey.Draw2D = false;

            Assert.IsFalse(desKey.Draw2D);
        }

        [Test]
        public void Property_Draw3D_StoresCorrectly()
        {
            var desKey = new DescriptionKey();
            desKey.Draw3D = true;

            Assert.IsTrue(desKey.Draw3D);

            desKey.Draw3D = false;

            Assert.IsFalse(desKey.Draw3D);
        }

    }
}