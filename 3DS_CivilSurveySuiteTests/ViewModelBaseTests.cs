using System;
using _3DS_CivilSurveySuite.ViewModels;
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
        [ExpectedException(typeof(Exception))]
        public void SetProperty_Is_Failed()
        {
            var value = "Test";
            var expected = "Test";
            var vm = new TestViewModelBase();
            vm.InvalidTestProperty = value;
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

            protected override bool ThrowOnInvalidPropertyName => true;

            public string TestProperty
            {
                get => _testProperty;
                set => SetProperty(ref _testProperty, value);
            }

            public string InvalidTestProperty
            {
                get => _testProperty;
                set => SetProperty(ref _testProperty, value, "Invalid");
            }
        }
    }

  
}
