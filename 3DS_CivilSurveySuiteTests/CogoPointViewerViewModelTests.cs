using System.Collections.Generic;
using System.Linq;
using _3DS_CivilSurveySuite.Model;
using _3DS_CivilSurveySuite.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace _3DS_CivilSurveySuiteTests
{
    [TestClass]
    public class CogoPointViewerViewModelTests
    {
        private Mock<ICogoPointViewerService> _mock;

        [TestInitialize]
        public void Setup()
        {
            _mock = new Mock<ICogoPointViewerService>();
            _mock.Setup(m => m.GetPoints()).Returns(() => new List<CivilPoint>
            {
                new CivilPoint { RawDescription = "Scott" },
                new CivilPoint { RawDescription = "Jeff" },
                new CivilPoint { RawDescription = "Gus" },
            });

        }


        [TestMethod]
        public void SelectedCommand_Execute_HasSelectedItem()
        {
            var vm = new CogoPointViewerViewModel(_mock.Object);

            vm.SelectedItem = new CivilPoint();

            vm.SelectCommand.CanExecute(true);
            vm.SelectCommand.Execute(null);
        }

        [TestMethod]
        public void UpdateCommand_Execute_HasSelectedItem()
        {
            var vm = new CogoPointViewerViewModel(_mock.Object);

            vm.SelectedItem = new CivilPoint();

            vm.UpdateCommand.CanExecute(true);
            vm.UpdateCommand.Execute(null);
        }

        [TestMethod]
        public void ZoomToCommand_Execute_HasSelectedItem()
        {
            var vm = new CogoPointViewerViewModel(_mock.Object);

            vm.SelectedItem = new CivilPoint();

            vm.ZoomToCommand.CanExecute(true);
            vm.ZoomToCommand.Execute(null);
        }

        [TestMethod]
        public void Filter()
        {
            var vm = new CogoPointViewerViewModel(_mock.Object);
            Assert.AreEqual(3, vm.CogoPoints.Count);

            vm.FilterText = "Scott";

            Assert.AreEqual(1, vm.ItemsView.Cast<object>().Count());
        }
    }
}
