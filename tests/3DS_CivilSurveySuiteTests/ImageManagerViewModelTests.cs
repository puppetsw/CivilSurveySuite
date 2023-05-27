using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using CivilSurveySuite.Shared.Services.Interfaces;
using CivilSurveySuite.UI.ViewModels;
using Moq;
using NUnit.Framework;

namespace CivilSurveySuiteTests
{
    [TestFixture]
    public class ImageManagerViewModelTests
    {
        private const string TEST_IMAGE_FILE_NAME = "testimage";

        private string TestDirectory { get; set; }

        [SetUp]
        public void TestSetup()
        {
            string testFile = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? throw new InvalidOperationException(), "TestFiles");
            TestDirectory = testFile;
        }

        [Test]
        public void LoadImages_ShowDialog_Returns_True()
        {
            var mockBrowseService = new Mock<IFolderBrowserDialogService>();
            mockBrowseService.Setup(m => m.ShowDialog()).Returns(() => true);
            mockBrowseService.Setup(m => m.SelectedPath).Returns(TestDirectory);

            var mockRasterImageService = new Mock<IRasterImageService>();
            mockRasterImageService.Setup(m => m.InsertRasterImage(new List<string>(), 0, 0, 0, 0));

            var vm = new ImageManagerViewModel(mockBrowseService.Object, mockRasterImageService.Object);

            vm.LoadImagesCommand.CanExecute(true);
            vm.LoadImagesCommand.Execute(null);

            Assert.IsTrue(vm.Images[0].Name == TEST_IMAGE_FILE_NAME);
            Assert.IsTrue(vm.HasImages);
        }

        [Test]
        public void LoadImages_ShowDialog_Returns_False()
        {
            var mockBrowseService = new Mock<IFolderBrowserDialogService>();
            mockBrowseService.Setup(m => m.ShowDialog()).Returns(() => false);

            var mockRasterImageService = new Mock<IRasterImageService>();
            mockRasterImageService.Setup(m => m.InsertRasterImage(new List<string>(), 0, 0, 0, 0));

            var vm = new ImageManagerViewModel(mockBrowseService.Object, mockRasterImageService.Object);

            Assert.IsTrue(vm.LoadImagesCommand.CanExecute(true));
            vm.LoadImagesCommand.Execute(null);
        }

        [Test]
        public void InsertImagesCommand_With_SelectedImages()
        {
            var mockBrowseService = new Mock<IFolderBrowserDialogService>();
            mockBrowseService.Setup(m => m.ShowDialog()).Returns(() => true);
            mockBrowseService.Setup(m => m.SelectedPath).Returns(TestDirectory);

            var mockRasterImageService = new Mock<IRasterImageService>();

            var vm = new ImageManagerViewModel(mockBrowseService.Object, mockRasterImageService.Object);

            vm.LoadImagesCommand.CanExecute(true);
            vm.LoadImagesCommand.Execute(null);

            vm.Images[0].IsSelected = true;

            Assert.IsTrue(vm.InsertImagesCommand.CanExecute(true));
            vm.InsertImagesCommand.Execute(null);
        }

        [Test]
        public void SelectAllCommand_With_SelectedImages()
        {
            var mockBrowseService = new Mock<IFolderBrowserDialogService>();
            mockBrowseService.Setup(m => m.ShowDialog()).Returns(() => true);
            mockBrowseService.Setup(m => m.SelectedPath).Returns(TestDirectory);

            var mockRasterImageService = new Mock<IRasterImageService>();

            var vm = new ImageManagerViewModel(mockBrowseService.Object, mockRasterImageService.Object);

            vm.LoadImagesCommand.CanExecute(true);
            vm.LoadImagesCommand.Execute(null);

            vm.SelectAllCommand.CanExecute(true);
            vm.SelectAllCommand.Execute(null);

            Assert.IsTrue(vm.Images[0].IsSelected);
        }

        [Test]
        public void SelectNoneCommand_With_SelectedImages()
        {
            var mockBrowseService = new Mock<IFolderBrowserDialogService>();
            mockBrowseService.Setup(m => m.ShowDialog()).Returns(() => true);
            mockBrowseService.Setup(m => m.SelectedPath).Returns(TestDirectory);

            var mockRasterImageService = new Mock<IRasterImageService>();

            var vm = new ImageManagerViewModel(mockBrowseService.Object, mockRasterImageService.Object);

            vm.LoadImagesCommand.CanExecute(true);
            vm.LoadImagesCommand.Execute(null);

            vm.Images[0].IsSelected = true;

            vm.SelectNoneCommand.CanExecute(true);
            vm.SelectNoneCommand.Execute(null);

            Assert.IsFalse(vm.Images[0].IsSelected);
        }
    }
}