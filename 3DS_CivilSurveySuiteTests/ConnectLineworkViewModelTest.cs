using System.IO;
using _3DS_CivilSurveySuite.Model;
using _3DS_CivilSurveySuite.UI.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace _3DS_CivilSurveySuiteTests
{
    [TestClass]
    public class ConnectLineworkViewModelTest
    {
        [TestMethod]
        public void LoadSettings_From_File()
        {
            var directory = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            var path = Path.Combine(directory, "TestFiles\\3DS_DescriptionKeys.xml");

            var vm = new ConnectLineworkViewModel(path, null);
        }

        [TestMethod]
        public void SaveSettings_To_File()
        {
            var directory = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
            var path = Path.Combine(directory, "TestFiles\\3DS_DescriptionKeys.xml");

            var vm = new ConnectLineworkViewModel(path, null);
            vm.Save(path);
        }

        [TestMethod]
        [TestCategory("Commands")]
        public void AddRowCommand_Execute()
        {
            var vm = new ConnectLineworkViewModel("", null);

            vm.AddRowCommand.CanExecute(true);
            vm.AddRowCommand.Execute(null);

            Assert.IsTrue(vm.DescriptionKeys.Count == 1);
        }

        [TestMethod]
        [TestCategory("Commands")]
        public void RemoveRowCommand_Execute()
        {
            var vm = new ConnectLineworkViewModel("", null);

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
            Mock<IConnectLineworkService> mockService = new Mock<IConnectLineworkService>();
            var vm = new ConnectLineworkViewModel(string.Empty, mockService.Object);

            vm.ConnectCommand.CanExecute(true);
            vm.ConnectCommand.Execute(true);
        }
    }
}
