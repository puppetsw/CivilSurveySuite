using System.Collections.Generic;
using System.Linq;
using _3DS_CivilSurveySuite.Model;
using _3DS_CivilSurveySuite.UI.Services;
using _3DS_CivilSurveySuite.UI.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace _3DS_CivilSurveySuiteTests
{
    [TestClass]
    public class CogoPointViewerViewModelTests
    {
        private Mock<ICogoPointEditorService> _mock;

        [TestInitialize]
        public void Setup()
        {
            _mock = new Mock<ICogoPointEditorService>();
            _mock.Setup(m => m.GetPoints()).Returns(() => new List<CivilPoint>
            {
                new CivilPoint { RawDescription = "Scott" },
                new CivilPoint { RawDescription = "Jeff" },
                new CivilPoint { RawDescription = "Gus" },
            });

        }

        [TestMethod]
        public void SelectedCommand_Execute()
        {
            var vm = new CogoPointEditorViewModel(_mock.Object);

            vm.SelectedItem = new CivilPoint();

            vm.SelectCommand.CanExecute(true);
            vm.SelectCommand.Execute(null);
        }

        [TestMethod]
        public void CopyDescriptionFormatCommand_SelectedItems()
        {
            var vm = new CogoPointEditorViewModel(_mock.Object);

            vm.SelectionChangedCommand.CanExecute(true);
            vm.SelectionChangedCommand.Execute(vm.CogoPoints);

            vm.CopyDescriptionFormatCommand.Execute(null);
        }

        [TestMethod]
        public void CopyDescriptionFormatCommand_NoSelectedItems()
        {
            var vm = new CogoPointEditorViewModel(_mock.Object);

            vm.CopyDescriptionFormatCommand.CanExecute(true);
            vm.CopyDescriptionFormatCommand.Execute(null);
        }

        [TestMethod]
        public void CopyRawDescriptionCommand_SelectedItems()
        {
            var vm = new CogoPointEditorViewModel(_mock.Object);

            vm.SelectionChangedCommand.CanExecute(true);
            vm.SelectionChangedCommand.Execute(vm.CogoPoints);

            vm.CopyRawDescriptionCommand.Execute(null);
        }

        [TestMethod]
        public void CopyRawDescriptionCommand_NoSelectedItems()
        {
            var vm = new CogoPointEditorViewModel(_mock.Object);

            vm.CopyRawDescriptionCommand.CanExecute(true);
            vm.CopyRawDescriptionCommand.Execute(null);
        }

        [TestMethod]
        public void UpdateCommand_Execute()
        {
            var vm = new CogoPointEditorViewModel(_mock.Object);

            vm.SelectedItem = new CivilPoint();

            vm.UpdateCommand.CanExecute(true);
            vm.UpdateCommand.Execute(null);
        }

        [TestMethod]
        public void ZoomToCommand_Execute()
        {
            var vm = new CogoPointEditorViewModel(_mock.Object);

            vm.SelectedItem = new CivilPoint();

            vm.ZoomToCommand.CanExecute(true);
            vm.ZoomToCommand.Execute(null);
        }

        [TestMethod]
        public void SelectionChangedCommand_Execute()
        {
            var vm = new CogoPointEditorViewModel(_mock.Object);

            vm.SelectionChangedCommand.CanExecute(true);
            vm.SelectionChangedCommand.Execute(vm.CogoPoints);

            Assert.AreEqual(3, vm.SelectedItems.Count);
        }

        [TestMethod]
        public void SelectionChangedCommand_Execute_NoItems()
        {
            var vm = new CogoPointEditorViewModel(_mock.Object);

            vm.SelectionChangedCommand.CanExecute(true);
            vm.SelectionChangedCommand.Execute(null);
        }

        [TestMethod]
        public void Filter_Property_Changed()
        {
            var vm = new CogoPointEditorViewModel(_mock.Object);
            Assert.AreEqual(3, vm.CogoPoints.Count);

            vm.FilterText = "Scott";

            Assert.AreEqual(1, vm.ItemsView.Cast<object>().Count());
        }
    }
}
