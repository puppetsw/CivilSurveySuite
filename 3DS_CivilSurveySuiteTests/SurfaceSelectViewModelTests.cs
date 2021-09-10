﻿using System.Collections.Generic;
using _3DS_CivilSurveySuite.Model;
using _3DS_CivilSurveySuite.UI.Services;
using _3DS_CivilSurveySuite.UI.ViewModels;
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
            var mock = new Mock<ISurfaceSelectService>();
            mock.Setup(m => m.GetSurfaces()).Returns(() => new List<CivilSurface>
            {
                new CivilSurface(),
                new CivilSurface(),
            });

            var vm = new SurfaceSelectViewModel(mock.Object);
            var expectedCount = 2;
            Assert.AreEqual(expectedCount, vm.Surfaces.Count);
        }

        [TestMethod]
        public void Set_SelectedSurfaceName_Property()
        {
            var mock = new Mock<ISurfaceSelectService>();
            mock.Setup(m => m.GetSurfaces()).Returns(() => new List<CivilSurface>
            {
                new CivilSurface { Name = "EG"},
                new CivilSurface(),
            });

            var vm = new SurfaceSelectViewModel(mock.Object);
            vm.SelectedSurface = vm.Surfaces[0];

            var expectedName = "EG";

            Assert.AreEqual(expectedName, vm.SelectedSurface.Name);
        }

        [TestMethod]
        public void SelectSurfaceCommand_Execute()
        {
            var selectableSurface = new CivilSurface { Name = "EG" };

            var mock = new Mock<ISurfaceSelectService>();
            mock.Setup(m => m.GetSurfaces()).Returns(() => new List<CivilSurface>
            {
                new CivilSurface { Name = "Test" },
                selectableSurface
            });

            mock.Setup(m => m.SelectSurface()).Returns(() => selectableSurface);

            var vm = new SurfaceSelectViewModel(mock.Object);
            vm.SelectSurfaceCommand.CanExecute(true);
            vm.SelectSurfaceCommand.Execute(null);
            
            Assert.AreEqual(selectableSurface, vm.SelectedSurface);
        }

        [TestMethod]
        public void SelectSurfaceCommand_Execute_SurfaceExists()
        {
            var mock = new Mock<ISurfaceSelectService>();
            mock.Setup(m => m.GetSurfaces()).Returns(() => new List<CivilSurface>
            {
                new CivilSurface { Name = "EG"},
                new CivilSurface(),
            });
            mock.Setup(m => m.SelectSurface()).Returns(() => new CivilSurface { Name = "EG" });

            var vm = new SurfaceSelectViewModel(mock.Object);

            vm.SelectSurfaceCommand.CanExecute(true);
            vm.SelectSurfaceCommand.Execute(null);
        }

        [TestMethod]
        public void SelectSurfaceCommand_Execute_SelectionCancelled()
        {
            var mock = new Mock<ISurfaceSelectService>();
            mock.Setup(m => m.GetSurfaces()).Returns(() => new List<CivilSurface>
            {
                new CivilSurface { Name = "EG"},
                new CivilSurface(),
            });
            mock.Setup(m => m.SelectSurface()).Returns(() => null);

            var vm = new SurfaceSelectViewModel(mock.Object);

            vm.SelectSurfaceCommand.CanExecute(true);
            vm.SelectSurfaceCommand.Execute(null);
        }

    }
}
