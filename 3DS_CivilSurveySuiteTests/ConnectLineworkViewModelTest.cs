using System.IO;
using _3DS_CivilSurveySuite.Model;
using _3DS_CivilSurveySuite.UI.Services;
using _3DS_CivilSurveySuite.UI.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace _3DS_CivilSurveySuiteTests
{
    [TestClass]
    public class ConnectLineworkViewModelTest
    {
        private const string _testFileName = "TestFiles\\3DS_DescriptionKeys.xml";

        private string _testPath;

        private Mock<IConnectLineworkService> _mock;

        [TestInitialize]
        public void Setup()
        {
            var directory = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            _testPath = Path.Combine(directory, _testFileName);

            _mock = new Mock<IConnectLineworkService>();
            _mock.SetupAllProperties();
            _mock.Object.DescriptionKeyFile = _testPath;
        }

        [TestMethod]
        public void LoadSettings_From_File()
        {
            var vm = new ConnectLineworkViewModel(_mock.Object);
        }

        [TestMethod]
        public void SaveSettings_To_File()
        {
            var vm = new ConnectLineworkViewModel(_mock.Object);
            vm.Save(_testPath);
        }

        [TestMethod]
        public void AddRowCommand_Execute()
        {
            var vm = new ConnectLineworkViewModel(_mock.Object);

            vm.AddRowCommand.CanExecute(true);
            vm.AddRowCommand.Execute(null);

            Assert.IsTrue(vm.DescriptionKeys.Count == 1);
        }

        [TestMethod]
        public void RemoveRowCommand_Execute()
        {
            var vm = new ConnectLineworkViewModel(_mock.Object);

            vm.AddRowCommand.CanExecute(true);
            vm.AddRowCommand.Execute(null);

            Assert.IsTrue(vm.DescriptionKeys.Count == 1);

            vm.SelectedKey = vm.DescriptionKeys[0];

            vm.RemoveRowCommand.CanExecute(true);
            vm.RemoveRowCommand.Execute(null);

            Assert.IsTrue(vm.DescriptionKeys.Count == 0);
        }

        [TestMethod]
        public void ConnectCommand_Execute()
        {
            var vm = new ConnectLineworkViewModel(_mock.Object);

            vm.ConnectCommand.CanExecute(true);
            vm.ConnectCommand.Execute(true);
        }
    }
}
