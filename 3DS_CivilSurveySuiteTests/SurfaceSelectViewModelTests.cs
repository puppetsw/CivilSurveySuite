using System.Collections.Generic;
using _3DS_CivilSurveySuite.Model;
using _3DS_CivilSurveySuite.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace _3DS_CivilSurveySuiteTests
{
    [TestClass]
    public class SurfaceSelectViewModelTests
    {
        [TestMethod]
        public void SurfaceNames_OnConstruct()
        {
            var _mock = new Mock<ISurfaceSelectService>();
            _mock.Setup(m => m.GetSurfaces()).Returns(() => new List<CivilSurface>
            {
                new CivilSurface(),
                new CivilSurface(),
            });

            var vm = new SurfaceSelectViewModel(_mock.Object);
            var expectedCount = 2;
            Assert.AreEqual(expectedCount, vm.Surfaces.Count);
        }

        [TestMethod]
        public void Set_SelectedSurfaceName_Property()
        {
            var _mock = new Mock<ISurfaceSelectService>();
            _mock.Setup(m => m.GetSurfaces()).Returns(() => new List<CivilSurface>
            {
                new CivilSurface { Name = "EG"},
                new CivilSurface(),
            });

            var vm = new SurfaceSelectViewModel(_mock.Object);
            vm.SelectedSurface = vm.Surfaces[0];

            var expectedName = "EG";

            Assert.AreEqual(expectedName, vm.SelectedSurface.Name);
        }




    }
}
