using _3DS_CivilSurveySuite.UI.ViewModels;
using NUnit.Framework;

namespace _3DS_CivilSurveySuiteTests
{
    [TestFixture]
    public class AngleCalculatorViewModelTests
    {
        [Test]
        public void Test_AngleCalculatorViewModel_Constructor()
        {
            var vm = new AngleCalculatorViewModel();
            Assert.IsNotNull(vm);
        }

        [Test]
        public void DigitButtonPressCommand_Execute_NumberKey()
        {
            var vm = new AngleCalculatorViewModel();
            vm.DigitButtonPressCommand.Execute("1");
            Assert.AreEqual("1", vm.Display);
            vm.DigitButtonPressCommand.Execute("2");
            Assert.AreEqual("12", vm.Display);
        }

        [Test]
        public void DigitButtonPressCommand_Execute_DelKey()
        {
            var vm = new AngleCalculatorViewModel();
            vm.Display = "10";
            vm.DigitButtonPressCommand.Execute("Del");
            Assert.AreEqual("1", vm.Display);
        }

        [Test]
        public void DigitButtonPressCommand_Execute_DelKey_Last_Number()
        {
            var vm = new AngleCalculatorViewModel();
            vm.Display = "1";
            vm.DigitButtonPressCommand.Execute("Del");
            Assert.AreEqual("0", vm.Display);
        }

        [Test]
        public void DigitButtonPressCommand_Execute_DecimalKey()
        {
            var vm = new AngleCalculatorViewModel();
            vm.Display = "10";
            vm.DigitButtonPressCommand.Execute(".");
            Assert.AreEqual("10.", vm.Display);
        }

        [Test]
        public void DigitButtonPressCommand_Execute_DecimalKey_Zero()
        {
            var vm = new AngleCalculatorViewModel();
            vm.Display = "0";
            vm.DigitButtonPressCommand.Execute(".");
            Assert.AreEqual("0.", vm.Display);
        }

        [Test]
        public void DigitButtonPressCommand_Execute_ClearKey()
        {
            var vm = new AngleCalculatorViewModel();
            vm.Display = "10";
            vm.DigitButtonPressCommand.Execute("C");
            Assert.AreEqual("0", vm.Display);
        }

        [Test]
        public void OperationButtonPressCommand_Execute_Addition()
        {
            var vm = new AngleCalculatorViewModel();
            vm.Display = "10";
            vm.OperationButtonPressCommand.Execute("+");
            vm.Display = "20";
            vm.OperationButtonPressCommand.Execute("=");
            Assert.AreEqual("30", vm.Display);
        }

        [Test]
        public void OperationButtonPressCommand_Execute_Subtraction()
        {
            var vm = new AngleCalculatorViewModel();
            vm.DigitButtonPressCommand.Execute("2");
            vm.DigitButtonPressCommand.Execute("0");
            vm.OperationButtonPressCommand.Execute("-");
            vm.DigitButtonPressCommand.Execute("1");
            vm.DigitButtonPressCommand.Execute("0");
            vm.OperationButtonPressCommand.Execute("=");
            Assert.AreEqual("10", vm.Display);
        }

        [Test]
        public void OperationButtonPressCommand_Execute_Subtraction_2()
        {
            var vm = new AngleCalculatorViewModel();
            vm.DigitButtonPressCommand.Execute("2");
            vm.DigitButtonPressCommand.Execute("0");
            vm.OperationButtonPressCommand.Execute("-");
            vm.DigitButtonPressCommand.Execute("1");
            vm.DigitButtonPressCommand.Execute("0");
            vm.OperationButtonPressCommand.Execute("-");
            Assert.AreEqual("10", vm.Display);
        }

        [Test]
        public void OperationButtonPressCommand_Execute_Addition_NewValue_After()
        {
            var vm = new AngleCalculatorViewModel();
            vm.DigitButtonPressCommand.Execute("1");
            vm.DigitButtonPressCommand.Execute("0");
            vm.OperationButtonPressCommand.Execute("+");
            vm.DigitButtonPressCommand.Execute("2");
            vm.DigitButtonPressCommand.Execute("0");
            vm.OperationButtonPressCommand.Execute("=");
            Assert.AreEqual("30", vm.Display);
            vm.DigitButtonPressCommand.Execute("1");
            vm.DigitButtonPressCommand.Execute("0");
            vm.OperationButtonPressCommand.Execute("+");
            vm.DigitButtonPressCommand.Execute("2");
            vm.DigitButtonPressCommand.Execute("0");
            vm.OperationButtonPressCommand.Execute("=");
            Assert.AreEqual("30", vm.Display);
        }

        [Test]
        public void OperationButtonPressCommand_Execute_Equals_Double_Press()
        {
            var vm = new AngleCalculatorViewModel();
            vm.DigitButtonPressCommand.Execute("1");
            vm.DigitButtonPressCommand.Execute("0");
            vm.OperationButtonPressCommand.Execute("+");
            vm.DigitButtonPressCommand.Execute("2");
            vm.DigitButtonPressCommand.Execute("0");
            vm.OperationButtonPressCommand.Execute("=");
            vm.OperationButtonPressCommand.Execute("=");
            Assert.AreEqual("30", vm.Display);
        }

    }
}