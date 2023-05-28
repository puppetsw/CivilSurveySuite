using System.Collections.Generic;
using CivilSurveySuite.Common.Models;
using CivilSurveySuite.Common.Services.Interfaces;
using CivilSurveySuite.UI.ViewModels;
using Moq;
using NUnit.Framework;

namespace CivilSurveySuiteTests
{
    [TestFixture]
    public class SurfaceSelectViewModelTests
    {
        [Test]
        public void SurfaceNames_OnConstruct()
        {
            var mock = new Mock<ICivilSelectService>();
            mock.Setup(m => m.GetSurfaces()).Returns(() => new List<CivilSurface>
            {
                new CivilSurface(),
                new CivilSurface(),
            });

            var vm = new SelectSurfaceViewModel(mock.Object);
            var expectedCount = 2;
            Assert.AreEqual(expectedCount, vm.Surfaces.Count);
        }

        [Test]
        public void Set_SelectedSurfaceName_Property()
        {
            var mock = new Mock<ICivilSelectService>();
            mock.Setup(m => m.GetSurfaces()).Returns(() => new List<CivilSurface>
            {
                new CivilSurface { Name = "EG"},
                new CivilSurface(),
            });

            var vm = new SelectSurfaceViewModel(mock.Object);
            vm.SelectedSurface = vm.Surfaces[0];

            var expectedName = "EG";

            Assert.AreEqual(expectedName, vm.SelectedSurface.Name);
        }

        [Test]
        public void SelectSurfaceCommand_Execute()
        {
            var selectableSurface = new CivilSurface { Name = "EG" };

            var mock = new Mock<ICivilSelectService>();
            mock.Setup(m => m.GetSurfaces()).Returns(() => new List<CivilSurface>
            {
                new CivilSurface { Name = "Test" },
                selectableSurface
            });

            mock.Setup(m => m.SelectSurface()).Returns(() => selectableSurface);

            var vm = new SelectSurfaceViewModel(mock.Object);
            vm.SelectSurfaceCommand.CanExecute(true);
            vm.SelectSurfaceCommand.Execute(null);

            Assert.AreEqual(selectableSurface, vm.SelectedSurface);
        }

        [Test]
        public void SelectSurfaceCommand_Execute_SurfaceExists()
        {
            var mock = new Mock<ICivilSelectService>();
            mock.Setup(m => m.GetSurfaces()).Returns(() => new List<CivilSurface>
            {
                new CivilSurface { Name = "EG"},
                new CivilSurface(),
            });
            mock.Setup(m => m.SelectSurface()).Returns(() => new CivilSurface { Name = "EG" });

            var vm = new SelectSurfaceViewModel(mock.Object);

            Assert.IsTrue(vm.SelectSurfaceCommand.CanExecute(true));
            vm.SelectSurfaceCommand.Execute(null);
        }

        [Test]
        public void SelectSurfaceCommand_Execute_SelectionCancelled()
        {
            var mock = new Mock<ICivilSelectService>();
            mock.Setup(m => m.GetSurfaces()).Returns(() => new List<CivilSurface>
            {
                new CivilSurface { Name = "EG"},
                new CivilSurface(),
            });
            mock.Setup(m => m.SelectSurface()).Returns(() => null);

            var vm = new SelectSurfaceViewModel(mock.Object);

            Assert.IsTrue(vm.SelectSurfaceCommand.CanExecute(true));
            vm.SelectSurfaceCommand.Execute(null);
        }

    }
}