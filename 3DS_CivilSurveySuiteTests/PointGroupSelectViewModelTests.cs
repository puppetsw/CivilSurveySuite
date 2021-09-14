using _3DS_CivilSurveySuite.Model;
using _3DS_CivilSurveySuite.UI.Services;
using _3DS_CivilSurveySuite.UI.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace _3DS_CivilSurveySuiteTests
{
    [TestClass]
    public class PointGroupSelectViewModelTests
    {
        [TestMethod]
        public void ViewModel_Construct_SelectsFirst()
        {
            var pointGroup1 = new CivilPointGroup();
            var pointGroup2 = new CivilPointGroup();

            var pointGroupSelectService = new Mock<ISelectPointGroupService>();
            pointGroupSelectService.Setup(m => m.GetPointGroups()).Returns(() => new []{ pointGroup1, pointGroup2 });

            var vm = new SelectPointGroupViewModel(pointGroupSelectService.Object);


            Assert.AreEqual(2, vm.PointGroups.Count);
            Assert.AreEqual(pointGroup1, vm.SelectedPointGroup);
        }
    }
}
