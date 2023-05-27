using CivilSurveySuite.Shared.Services.Interfaces;
using CivilSurveySuite.UI.ViewModels;
using Moq;
using NUnit.Framework;

namespace CivilSurveySuiteTests
{
    [TestFixture]
    public class CogoPointMoveLabelViewModelTests
    {
        [Test]
        public void Property_DeltaX_StoresCorrectly()
        {
            var moveLabelService = new Mock<ICogoPointService>();
            var vm = new CogoPointMoveLabelViewModel(moveLabelService.Object)
            {
                DeltaX = 100.00
            };

            Assert.AreEqual(100.00, vm.DeltaX);
        }

        [Test]
        public void Property_DeltaY_StoresCorrectly()
        {
            var moveLabelService = new Mock<ICogoPointService>();
            var vm = new CogoPointMoveLabelViewModel(moveLabelService.Object)
            {
                DeltaY = 100.00
            };

            Assert.AreEqual(100.00, vm.DeltaY);
        }

        [Test]
        public void MoveCommand_Execute()
        {
            var moveLabelService = new Mock<ICogoPointService>();
            var vm = new CogoPointMoveLabelViewModel(moveLabelService.Object);
            Assert.IsTrue(vm.MoveCommand.CanExecute(true));
            vm.MoveCommand.Execute(null);
        }
    }
}