using _3DS_CivilSurveySuite.UI.Models;
using _3DS_CivilSurveySuite.UI.Services.Interfaces;
using _3DS_CivilSurveySuite.UI.ViewModels;
using Moq;
using NUnit.Framework;

namespace _3DS_CivilSurveySuiteTests
{
    [TestFixture]
    public class PointGroupSelectViewModelTests
    {
        [Test]
        public void ViewModel_Construct_SelectsFirst()
        {
            var pointGroup1 = new CivilPointGroup();
            var pointGroup2 = new CivilPointGroup();

            var pointGroupSelectService = new Mock<ICivilSelectService>();
            pointGroupSelectService.Setup(m => m.GetPointGroups()).Returns(() => new []{ pointGroup1, pointGroup2 });

            var vm = new SelectPointGroupViewModel(pointGroupSelectService.Object);


            Assert.AreEqual(2, vm.PointGroups.Count);
            Assert.AreEqual(pointGroup1, vm.SelectedPointGroup);
        }
    }
}
