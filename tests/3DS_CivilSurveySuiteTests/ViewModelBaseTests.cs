using _3DS_CivilSurveySuite.Shared.Models;
using NUnit.Framework;

namespace _3DS_CivilSurveySuiteTests
{
    [TestFixture]
    public class ViewModelBaseTests
    {
        [Test]
        public void SetProperty_Is_Success()
        {
            var value = "Test";
            var expected = "Test";
            var vm = new TestViewModelBase
            {
                TestProperty = value
            };
            Assert.AreEqual(expected, vm.TestProperty);
        }

        [Test]
        public void SetProperty_Is_The_Same()
        {
            var value = "Test";
            var expected = "Test";
            var vm = new TestViewModelBase() { TestProperty = "Test" };

            vm.TestProperty = value;
            Assert.AreEqual(expected, vm.TestProperty);
        }

        private class TestViewModelBase : ObservableObject
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