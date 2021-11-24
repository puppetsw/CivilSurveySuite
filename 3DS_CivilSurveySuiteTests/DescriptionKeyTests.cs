using _3DS_CivilSurveySuite.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace _3DS_CivilSurveySuiteTests
{
    [TestClass]
    public class DescriptionKeyTests
    {
        [TestMethod]
        public void Property_Description_StoresCorrectly()
        {
            var desKey = new DescriptionKey();
            desKey.Description = "TestId";

            Assert.AreEqual("TestId", desKey.Description);
        }

        [TestMethod]
        public void Property_Key_StoresCorrectly()
        {
            var desKey = new DescriptionKey();
            desKey.Key = "TST";

            Assert.AreEqual("TST", desKey.Key);
        }

        [TestMethod]
        public void Property_Layer_StoresCorrectly()
        {
            var desKey = new DescriptionKey();
            desKey.Layer = "TestLayer";

            Assert.AreEqual("TestLayer", desKey.Layer);
        }

        [TestMethod]
        public void Property_Draw2D_StoresCorrectly()
        {
            var desKey = new DescriptionKey();
            desKey.Draw2D = true;

            Assert.IsTrue(desKey.Draw2D);

            desKey.Draw2D = false;

            Assert.IsFalse(desKey.Draw2D);
        }

        [TestMethod]
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