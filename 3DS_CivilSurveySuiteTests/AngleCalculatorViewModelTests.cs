using _3DS_CivilSurveySuite.Model;
using _3DS_CivilSurveySuite.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace _3DS_CivilSurveySuiteTests
{
    [TestClass]
    public class AngleCalculatorViewModelTests
    {
        [TestMethod]
        public void AddCommand_Execute()
        {
            var vm = new AngleCalculatorViewModel();
            vm.FirstAngle = new Angle(90);
            vm.SecondAngle = new Angle(90);

            var expected = new Angle(180);

            vm.AddCommand.CanExecute(true);
            vm.AddCommand.Execute(null);

            Assert.AreEqual(expected.ToString(), vm.Result);
        }

        [TestMethod]
        public void AddCommand_Execute_Null_Property()
        {
            var vm = new AngleCalculatorViewModel();
            vm.FirstAngle = null;
            vm.SecondAngle = new Angle(90);

            var expected = "";

            vm.AddCommand.CanExecute(true);
            vm.AddCommand.Execute(null);

            Assert.AreEqual(expected.ToString(), vm.Result);
        }

        [TestMethod]
        public void SubtractCommand_Execute()
        {
            var vm = new AngleCalculatorViewModel();
            vm.FirstAngle = new Angle(180);
            vm.SecondAngle = new Angle(90);

            var expected = new Angle(90);

            vm.SubtractCommand.CanExecute(true);
            vm.SubtractCommand.Execute(null);

            Assert.AreEqual(expected.ToString(), vm.Result);
        }

        [TestMethod]
        public void SubtractCommand_Execute_Null_Property()
        {
            var vm = new AngleCalculatorViewModel();
            vm.FirstAngle = null;
            vm.SecondAngle = new Angle(90);

            var expected = "";

            vm.SubtractCommand.CanExecute(true);
            vm.SubtractCommand.Execute(null);

            Assert.AreEqual(expected.ToString(), vm.Result);
        }

        [TestMethod]
        public void FirstBearing_Property_Set_Valid()
        {
            var vm = new AngleCalculatorViewModel();
            vm.FirstBearing = 90;
            var expected = new Angle(90);

            Assert.AreEqual(90, vm.FirstBearing);
            Assert.AreEqual(expected, vm.FirstAngle);
        }

        [TestMethod]
        public void FirstBearing_Property_Set_Invalid()
        {
            var vm = new AngleCalculatorViewModel();
            vm.FirstBearing = 90.99999;
            var expected = new Angle(0);

            Assert.AreEqual(expected, vm.FirstAngle);
        }

        [TestMethod]
        public void SecondBearing_Property_Set_Valid()
        {
            var vm = new AngleCalculatorViewModel();
            vm.SecondBearing = 90;
            var expected = new Angle(90);

            Assert.AreEqual(90, vm.SecondBearing);
            Assert.AreEqual(expected, vm.SecondAngle);
        }

        [TestMethod]
        public void SecondBearing_Property_Set_Invalid()
        {
            var vm = new AngleCalculatorViewModel();
            vm.SecondBearing = 90.999999;
            var expected = new Angle(0);

            Assert.AreEqual(expected, vm.SecondAngle);
        }

    }
}
