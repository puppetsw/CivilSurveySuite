using _3DS_CivilSurveySuite.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace _3DS_CivilSurveySuiteTests
{
    [TestClass]
    public class ViewModelAbstractionTests
    {
        [TestMethod]
        public void TestViewModel()
        {
            var vm = new TraverseAngleViewModel();

            Assert.AreEqual(typeof(TraverseAngleViewModel), vm.GetType());
        }
    }
}
