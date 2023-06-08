using System.Collections.Generic;
using CivilSurveySuite.Common.Models;
using CivilSurveySuite.Common.Services.Interfaces;
using CivilSurveySuite.UI.ViewModels;
using Moq;
using NUnit.Framework;

namespace CivilSurveySuiteTests
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
            reportService.Setup(m => m.MoveDown(null));
            reportService.Setup(m => m.MoveUp(null));

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

            _viewModel = new CogoPointSurfaceReportViewModel(reportService.Object, saveService.Object);
        }

        [Test]
        public void GenerateReportCommand_Execute()
        {
            Assert.IsTrue(_viewModel.GenerateReportCommand.CanExecute(true));
            _viewModel.GenerateReportCommand.Execute(null);
        }
    }
}