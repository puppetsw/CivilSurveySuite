using System.Collections.Generic;
using _3DS_CivilSurveySuite.UI.Models;
using _3DS_CivilSurveySuite.UI.Services;
using _3DS_CivilSurveySuite.UI.Services.Interfaces;
using _3DS_CivilSurveySuite.UI.ViewModels;
using Moq;
using NUnit.Framework;

namespace _3DS_CivilSurveySuiteTests
{
    [TestFixture]
    public class CogoPointSurfaceReportViewModelTests
    {
        private CogoPointSurfaceReportViewModel _viewModel;
        private List<CivilPointGroup> _pointGroups;
        private List<CivilSurface> _surfaces;
        private List<CivilAlignment> _alignments;

        private const string TEST_FILE_NAME = "";

        [SetUp]
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

            var selectService = new Mock<ICivilSelectService>();

            selectService.Setup(m => m.GetPointGroups()).Returns(() => _pointGroups);
            selectService.Setup(m => m.GetAlignments()).Returns(() => _alignments);
            selectService.Setup(m => m.GetSurfaces()).Returns(() => _surfaces);
            selectService.Setup(m => m.SelectSurface()).Returns(() => surface);
            selectService.Setup(m => m.SelectAlignment()).Returns(() => alignment);
            selectService.Setup(m => m.SelectPointGroup()).Returns(() => pointGroup);

            var saveService = new Mock<ISaveFileDialogService>();
            saveService.Setup(m => m.ShowDialog()).Returns(() => true);
            saveService.Setup(m => m.FileName).Returns(TEST_FILE_NAME);

            _viewModel = new CogoPointSurfaceReportViewModel(reportService.Object, saveService.Object, selectService.Object);
        }


        [Test]
        public void SelectSurfaceCommand_Execute()
        {
            _viewModel.SelectSurfaceCommand.CanExecute(true);
            _viewModel.SelectSurfaceCommand.Execute(null);

            Assert.AreEqual("SelectedSurface", _viewModel.SelectedSurface.Name);
        }

        [Test]
        public void SelectSurfaceCommand_Execute_NullReturn()
        {
            var reportService = new Mock<ICogoPointSurfaceReportService>();
            var selectService = new Mock<ICivilSelectService>();
            selectService.Setup(m => m.SelectSurface()).Returns(() => null);
            var viewModel = new CogoPointSurfaceReportViewModel(reportService.Object, null, selectService.Object);

            Assert.IsTrue(viewModel.SelectSurfaceCommand.CanExecute(true));
            viewModel.SelectSurfaceCommand.Execute(null);
        }

        [Test]
        public void SelectPointGroupCommand_Execute()
        {
            _viewModel.SelectPointGroupCommand.CanExecute(true);
            _viewModel.SelectPointGroupCommand.Execute(null);

            Assert.AreEqual("SelectedPointGroup", _viewModel.SelectedPointGroup.Name);
        }

        [Test]
        public void SelectPointGroupCommand_Execute_NullReturn()
        {
            var reportService = new Mock<ICogoPointSurfaceReportService>();
            var selectService = new Mock<ICivilSelectService>();
            selectService.Setup(m => m.SelectPointGroup()).Returns(() => null);
            var viewModel = new CogoPointSurfaceReportViewModel(reportService.Object, null, selectService.Object);

            Assert.IsTrue(viewModel.SelectPointGroupCommand.CanExecute(true));
            viewModel.SelectPointGroupCommand.Execute(null);
        }

        [Test]
        public void Property_CalculatePointNearSurfaceEdge_StoresCorrectly()
        {
            var reportService = new Mock<ICogoPointSurfaceReportService>();
            var viewModel = new CogoPointSurfaceReportViewModel(reportService.Object, null, null);

            viewModel.CalculatePointNearSurfaceEdge = true;
            Assert.IsTrue(viewModel.CalculatePointNearSurfaceEdge);

            viewModel.CalculatePointNearSurfaceEdge = false;
            Assert.IsFalse(viewModel.CalculatePointNearSurfaceEdge);
        }


        [Test]
        public void CreateReportCommand_Execute()
        {
            //Assert.IsTrue(_viewModel.CreateReportCommand.CanExecute(true));
            //_viewModel.CreateReportCommand.Execute(null);
        }
    }
}