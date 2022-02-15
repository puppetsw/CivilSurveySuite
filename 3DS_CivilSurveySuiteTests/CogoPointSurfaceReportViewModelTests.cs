using System.Collections.Generic;
using _3DS_CivilSurveySuite.Model;
using _3DS_CivilSurveySuite.UI.Services;
using _3DS_CivilSurveySuite.UI.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace _3DS_CivilSurveySuiteTests
{
    [TestClass]
    public class CogoPointSurfaceReportViewModelTests
    {
        private CogoPointSurfaceReportViewModel _viewModel;
        private List<CivilPointGroup> _pointGroups;
        private List<CivilSurface> _surfaces;
        private List<CivilAlignment> _alignments;

        private const string TEST_FILE_NAME = "";

        [TestInitialize]
        public void Setup()
        {
            _pointGroups = new List<CivilPointGroup>
            {
                new CivilPointGroup { Name = "PointGroup1" },
                new CivilPointGroup { Name = "PointGroup2" },
                new CivilPointGroup { Name = "SelectedPointGroup" }
            };

            _surfaces = new List<CivilSurface>
            {
                new CivilSurface { Name = "Surface1" },
                new CivilSurface { Name = "Surface2" },
                new CivilSurface { Name = "SelectedSurface" }
            };

            _alignments = new List<CivilAlignment>
            {
                new CivilAlignment { Name = "Alignment1" },
                new CivilAlignment { Name = "Alignment2" },
                new CivilAlignment { Name = "SelectedAlignment" }
            };

            var surface = new CivilSurface { Name = "SelectedSurface" };
            var alignment = new CivilAlignment { Name = "SelectedAlignment" };
            var pointGroup = new CivilPointGroup { Name = "SelectedPointGroup" };

            var reportService = new Mock<ICogoPointSurfaceReportService>();
            reportService.Setup(m => m.GetPointGroups()).Returns(() => _pointGroups);
            reportService.Setup(m => m.GetAlignments()).Returns(() => _alignments);
            reportService.Setup(m => m.GetSurfaces()).Returns(() => _surfaces);
            reportService.Setup(m => m.SelectSurface()).Returns(() => surface);
            reportService.Setup(m => m.SelectAlignment()).Returns(() => alignment);
            reportService.Setup(m => m.SelectPointGroup()).Returns(() => pointGroup);

            var saveService = new Mock<ISaveFileDialogService>();
            saveService.Setup(m => m.ShowDialog()).Returns(() => true);
            saveService.Setup(m => m.FileName).Returns(TEST_FILE_NAME);

            _viewModel = new CogoPointSurfaceReportViewModel(reportService.Object, saveService.Object);
        }


        [TestMethod]
        public void SelectSurfaceCommand_Execute()
        {
            _viewModel.SelectSurfaceCommand.CanExecute(true);
            _viewModel.SelectSurfaceCommand.Execute(null);

            Assert.AreEqual("SelectedSurface", _viewModel.SelectedSurface.Name);
        }

        [TestMethod]
        public void SelectSurfaceCommand_Execute_NullReturn()
        {
            var reportService = new Mock<ICogoPointSurfaceReportService>();
            reportService.Setup(m => m.SelectSurface()).Returns(() => null);
            var viewModel = new CogoPointSurfaceReportViewModel(reportService.Object, null);

            Assert.IsTrue(viewModel.SelectSurfaceCommand.CanExecute(true));
            viewModel.SelectSurfaceCommand.Execute(null);
        }

        [TestMethod]
        public void SelectPointGroupCommand_Execute()
        {
            _viewModel.SelectPointGroupCommand.CanExecute(true);
            _viewModel.SelectPointGroupCommand.Execute(null);

            Assert.AreEqual("SelectedPointGroup", _viewModel.SelectedPointGroup.Name);
        }

        [TestMethod]
        public void SelectPointGroupCommand_Execute_NullReturn()
        {
            var reportService = new Mock<ICogoPointSurfaceReportService>();
            reportService.Setup(m => m.SelectPointGroup()).Returns(() => null);
            var viewModel = new CogoPointSurfaceReportViewModel(reportService.Object, null);

            Assert.IsTrue(viewModel.SelectPointGroupCommand.CanExecute(true));
            viewModel.SelectPointGroupCommand.Execute(null);
        }

        [TestMethod]
        public void Property_CalculatePointNearSurfaceEdge_StoresCorrectly()
        {
            var reportService = new Mock<ICogoPointSurfaceReportService>();
            var viewModel = new CogoPointSurfaceReportViewModel(reportService.Object, null);

            viewModel.CalculatePointNearSurfaceEdge = true;
            Assert.IsTrue(viewModel.CalculatePointNearSurfaceEdge);

            viewModel.CalculatePointNearSurfaceEdge = false;
            Assert.IsFalse(viewModel.CalculatePointNearSurfaceEdge);
        }


        [TestMethod]
        public void CreateReportCommand_Execute()
        {
            //Assert.IsTrue(_viewModel.CreateReportCommand.CanExecute(true));
            //_viewModel.CreateReportCommand.Execute(null);
        }
    }
}