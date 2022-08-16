using System.IO;
using _3DS_CivilSurveySuite.Shared.Services.Interfaces;
using _3DS_CivilSurveySuite.UI.ViewModels;
using Moq;
using NUnit.Framework;

namespace _3DS_CivilSurveySuiteTests
{
    [TestFixture]
    public class ConnectLineworkViewModelTest
    {
        private const string TEST_FILE_NAME = "TestFiles\\3DS_DescriptionKeys.xml";

        private string _testPath;

        private Mock<IConnectLineworkService> _mock;

        [SetUp]
        public void Setup()
        {
            string directory = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);

            if (directory != null)
                _testPath = Path.Combine(directory, TEST_FILE_NAME);

            _mock = new Mock<IConnectLineworkService>();
            _mock.SetupAllProperties();
            _mock.Object.DescriptionKeyFile = _testPath;
        }

        [Test]
        public void LoadSettings_FileDoesNotExist()
        {
            var cls = new Mock<IConnectLineworkService>();
            var vm = new ConnectLineworkViewModel(cls.Object);
            Assert.AreEqual(0, vm.DescriptionKeys.Count);
        }


        [Test]
        public void LoadSettings_From_File()
        {
            var _ = new ConnectLineworkViewModel(_mock.Object);
        }

        [Test]
        public void SaveSettings_To_File()
        {
            var vm = new ConnectLineworkViewModel(_mock.Object);
            vm.SaveSettings(_testPath);
        }

        [Test]
        public void AddRowCommand_Execute()
        {
            var vm = new ConnectLineworkViewModel(_mock.Object);

            vm.AddRowCommand.CanExecute(true);
            vm.AddRowCommand.Execute(null);

            Assert.IsTrue(vm.DescriptionKeys.Count > 1);
        }

        [Test]
        public void RemoveRowCommand_Execute()
        {
            var vm = new ConnectLineworkViewModel(_mock.Object);

            vm.AddRowCommand.CanExecute(true);
            vm.AddRowCommand.Execute(null);

            Assert.IsTrue(vm.DescriptionKeys.Count == 36);

            vm.SelectedKey = vm.DescriptionKeys[0];

            vm.RemoveRowCommand.CanExecute(true);
            vm.RemoveRowCommand.Execute(null);

            Assert.IsTrue(vm.DescriptionKeys.Count == 35);
        }

        [Test]
        public void ConnectCommand_Execute()
        {
            var vm = new ConnectLineworkViewModel(_mock.Object);

            Assert.IsTrue(vm.ConnectCommand.CanExecute(true));
            vm.ConnectCommand.Execute(true);
        }
    }
}