﻿using _3DS_CivilSurveySuite.UI.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace _3DS_CivilSurveySuiteTests
{
    [TestClass]
    public class ViewModelBaseTests
    {
        [TestMethod]
        public void SetProperty_Is_Success()
        {
            var value = "Test";
            var expected = "Test";
            var vm = new TestViewModelBase();
            vm.TestProperty = value;
            Assert.AreEqual(expected, vm.TestProperty);
        }

        [TestMethod]
        public void SetProperty_Is_The_Same()
        {
            var value = "Test";
            var expected = "Test";
            var vm = new TestViewModelBase() { TestProperty = "Test" };

            vm.TestProperty = value;
            Assert.AreEqual(expected, vm.TestProperty);
        }

        private class TestViewModelBase : ViewModelBase
        {
            private string _testProperty;

            public string TestProperty
            {
                get => _testProperty;
                set => SetProperty(ref _testProperty, value);
            }
        }
    }


}