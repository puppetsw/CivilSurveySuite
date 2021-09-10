using _3DS_CivilSurveySuite.UI.Services;
using _3DS_CivilSurveySuite.UI.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace _3DS_CivilSurveySuiteTests
{
    [TestClass]
    public class CogoPointMoveLabelViewModelTests
    {
        [TestMethod]
        public void Property_DeltaX_StoresCorrectly()
        {
            var moveLabelService = new Mock<ICogoPointMoveLabelService>();
            var vm = new CogoPointMoveLabelViewModel(moveLabelService.Object);

            vm.DeltaX = 100.00;

            Assert.AreEqual(100.00, vm.DeltaX);
        }

        [TestMethod]
        public void Property_DeltaY_StoresCorrectly()
        {
            var moveLabelService = new Mock<ICogoPointMoveLabelService>();
            var vm = new CogoPointMoveLabelViewModel(moveLabelService.Object);

            vm.DeltaY = 100.00;

            Assert.AreEqual(100.00, vm.DeltaY);
        }

        [TestMethod]
        public void MoveCommand_Execute()
        {
            var moveLabelService = new Mock<ICogoPointMoveLabelService>();
            var vm = new CogoPointMoveLabelViewModel(moveLabelService.Object);
            vm.MoveCommand.CanExecute(true);
            vm.MoveCommand.Execute(null);
        }
    }
}
