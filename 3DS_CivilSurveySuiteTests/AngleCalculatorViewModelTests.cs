using System.Collections.Generic;
using _3DS_CivilSurveySuite.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace _3DS_CivilSurveySuiteTests
{
    [TestClass]
    public class AngleCalculatorViewModelTests
    {
        [TestMethod]
        public void InputBearing_Property_Changed()
        {
            var result = false;
            var expectedString = "100.1000";
            var vm = new AngleCalculatorViewModel();
            vm.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(vm.InputBearing))
                {
                    result = true;
                }
            };
            vm.InputBearing = "100.1000";

            Assert.AreEqual(true, result);
            Assert.AreEqual(expectedString, vm.InputBearing);
        }

        [TestMethod]
        public void EnterDMSCommand_Execute()
        {
            var vm = new AngleCalculatorViewModel();
            vm.InputBearing = "100.1000";
            vm.EnterDMSCommand.CanExecute(true);
            vm.EnterDMSCommand.Execute(null);

            Assert.AreEqual(vm.DMSList.Count, 1);
        }
    }
}
