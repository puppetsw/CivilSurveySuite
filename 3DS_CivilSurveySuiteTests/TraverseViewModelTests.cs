using _3DS_CivilSurveySuite.Model;
using _3DS_CivilSurveySuite.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
// ReSharper disable ExpressionIsAlwaysNull

namespace _3DS_CivilSurveySuiteTests
{
    [TestClass]
    public class TraverseViewModelTests
    {
        [TestMethod]
        [TestCategory("Properties")]
        public void CloseBearing_Property_Change()
        {
            var result = false;
            var expectedString = "100.1000";
            Mock<IViewerService> mockService = new Mock<IViewerService>();
            var vm = new TraverseViewModel(mockService.Object, null);
            vm.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(vm.CloseBearing))
                {
                    result = true;
                }
            };
            vm.CloseBearing = "100.1000";

            Assert.AreEqual(true, result);
            Assert.AreEqual(expectedString, vm.CloseBearing);
        }

        [TestMethod]
        [TestCategory("Properties")]
        public void CloseDistance_Property_Change()
        {
            var result = false;
            var expectedString = "100.1000";
            Mock<IViewerService> mockService = new Mock<IViewerService>();
            var vm = new TraverseViewModel(mockService.Object, null);
            vm.PropertyChanged += (s, e) =>
            {
                if (e.PropertyName == nameof(vm.CloseDistance))
                {
                    result = true;
                }
            };
            vm.CloseDistance = "100.1000";

            Assert.AreEqual(true, result);
            Assert.AreEqual(expectedString, vm.CloseDistance);
        }

        [TestMethod]
        [TestCategory("Commands")]
        public void AddRowCommand_Execute()
        {
            Mock<IViewerService> mockService = new Mock<IViewerService>();
            var vm = new TraverseViewModel(mockService.Object, null);

            vm.AddRowCommand.CanExecute(true);
            vm.AddRowCommand.Execute(null);

            Assert.IsTrue(vm.TraverseItems.Count == 1);
        }

        [TestMethod]
        [TestCategory("Commands")]
        public void AddRowCommand_Execute_MultipleAdd_IndexShouldIncrease()
        {
            Mock<IViewerService> mockService = new Mock<IViewerService>();
            var vm = new TraverseViewModel(mockService.Object, null);

            vm.AddRowCommand.CanExecute(true);
            vm.AddRowCommand.Execute(null);
            vm.AddRowCommand.Execute(null);
            vm.AddRowCommand.Execute(null);

            Assert.IsTrue(vm.TraverseItems.Count == 3);
            Assert.AreEqual(0, vm.TraverseItems[0].Index);
            Assert.AreEqual(1, vm.TraverseItems[1].Index);
            Assert.AreEqual(2, vm.TraverseItems[2].Index);
        }

        [TestMethod]
        [TestCategory("Commands")]
        public void RemoveRowCommand_Execute()
        {
            Mock<IViewerService> mockService = new Mock<IViewerService>();
            var vm = new TraverseViewModel(mockService.Object, null);

            vm.AddRowCommand.CanExecute(true);
            vm.AddRowCommand.Execute(null);

            Assert.IsTrue(vm.TraverseItems.Count == 1);

            vm.SelectedTraverseItem = vm.TraverseItems[0];

            vm.RemoveRowCommand.CanExecute(true);
            vm.RemoveRowCommand.Execute(null);

            Assert.IsTrue(vm.TraverseItems.Count == 0);
        }

        [TestMethod]
        [TestCategory("Commands")]
        public void RemoveRowCommand_Execute_SelectedTraverseAngle_Is_Null()
        {
            Mock<IViewerService> mockService = new Mock<IViewerService>();
            var vm = new TraverseViewModel(mockService.Object, null);

            vm.AddRowCommand.CanExecute(true);
            vm.AddRowCommand.Execute(null);

            Assert.IsTrue(vm.TraverseItems.Count == 1);

            vm.SelectedTraverseItem = null;

            vm.RemoveRowCommand.CanExecute(true);
            vm.RemoveRowCommand.Execute(null);

            Assert.IsTrue(vm.TraverseItems.Count == 1);
        }

        [TestMethod]
        [TestCategory("Commands")]
        public void ClearTraverseCommand_Execute()
        {
            Mock<IViewerService> mockService = new Mock<IViewerService>();
            var vm = new TraverseViewModel(mockService.Object, null);

            vm.AddRowCommand.CanExecute(true);
            vm.AddRowCommand.Execute(null);

            Assert.IsTrue(vm.TraverseItems.Count == 1);

            vm.ClearCommand.CanExecute(true);
            vm.ClearCommand.Execute(null);

            Assert.IsTrue(vm.TraverseItems.Count == 0);
        }

        [TestMethod]
        [TestCategory("Commands")]
        public void GridUpdatedCommand_Execute()
        {
            Mock<IViewerService> mockService = new Mock<IViewerService>();
            var vm = new TraverseViewModel(mockService.Object, null);

            vm.TraverseItems.Add(new TraverseObject(0, 30));
            vm.TraverseItems.Add(new TraverseObject(90, 10));
            vm.TraverseItems.Add(new TraverseObject(180, 30));

            var expectedCloseDistance = "10.000";

            vm.GridUpdatedCommand.CanExecute(true);
            vm.GridUpdatedCommand.Execute(null);

            Assert.AreEqual(expectedCloseDistance, vm.CloseDistance);
        }

        [TestMethod]
        [TestCategory("Commands")]
        public void GridUpdatedCommand_Execute_Traverse_Count_Less_Than_Two()
        {
            Mock<IViewerService> mockService = new Mock<IViewerService>();
            var vm = new TraverseViewModel(mockService.Object, null);

            vm.TraverseItems.Add(new TraverseObject(0, 30));

            string expectedCloseDistance = null;

            vm.GridUpdatedCommand.CanExecute(true);
            vm.GridUpdatedCommand.Execute(null);

            Assert.AreEqual(expectedCloseDistance, vm.CloseDistance);
        }

        [TestMethod]
        [TestCategory("Commands")]
        public void FeetToMetersCommand_Execute()
        {
            var feetAndInchesValue = 100.10;
            var expectedValue = 30.734;

            Mock<IViewerService> mockService = new Mock<IViewerService>();
            var vm = new TraverseViewModel(mockService.Object, null);

            vm.AddRowCommand.CanExecute(true);
            vm.AddRowCommand.Execute(null);

            Assert.IsTrue(vm.TraverseItems.Count == 1);

            vm.SelectedTraverseItem = vm.TraverseItems[0];
            vm.SelectedTraverseItem.Distance = feetAndInchesValue;

            vm.FeetToMetersCommand.CanExecute(true);
            vm.FeetToMetersCommand.Execute(null);

            Assert.AreEqual(expectedValue, vm.TraverseItems[0].Distance);
        }

        [TestMethod]
        [TestCategory("Commands")]
        public void FeetToMetersCommand_Execute_SelectedTraverseAngle_Is_Null()
        {
            var feetAndInchesValue = 100.10;
            var expectedValue = 100.10;

            Mock<IViewerService> mockService = new Mock<IViewerService>();
            var vm = new TraverseViewModel(mockService.Object, null);

            vm.AddRowCommand.CanExecute(true);
            vm.AddRowCommand.Execute(null);
            vm.TraverseItems[0].Distance = feetAndInchesValue;

            vm.SelectedTraverseItem = null;

            vm.FeetToMetersCommand.CanExecute(true);
            vm.FeetToMetersCommand.Execute(null);

            Assert.AreEqual(expectedValue, vm.TraverseItems[0].Distance);
        }

        [TestMethod]
        [TestCategory("Commands")]
        public void LinksToMetersCommand_Execute()
        {
            var linkValue = 100;
            var expectedValue = 20.1168;

            Mock<IViewerService> mockService = new Mock<IViewerService>();
            var vm = new TraverseViewModel(mockService.Object, null);

            vm.AddRowCommand.CanExecute(true);
            vm.AddRowCommand.Execute(null);

            Assert.IsTrue(vm.TraverseItems.Count == 1);

            vm.SelectedTraverseItem = vm.TraverseItems[0];
            vm.SelectedTraverseItem.Distance = linkValue;

            vm.LinksToMetersCommand.CanExecute(true);
            vm.LinksToMetersCommand.Execute(null);

            Assert.AreEqual(expectedValue, vm.TraverseItems[0].Distance);
        }

        [TestMethod]
        [TestCategory("Commands")]
        public void LinksToMetersCommand_Execute_SelectedTraverseAngle_Is_Null()
        {
            var linkValue = 100;
            var expectedValue = 100;

            Mock<IViewerService> mockService = new Mock<IViewerService>();
            var vm = new TraverseViewModel(mockService.Object, null);

            vm.AddRowCommand.CanExecute(true);
            vm.AddRowCommand.Execute(null);
            vm.TraverseItems[0].Distance = linkValue;

            vm.SelectedTraverseItem = null;

            vm.LinksToMetersCommand.CanExecute(true);
            vm.LinksToMetersCommand.Execute(null);

            Assert.AreEqual(expectedValue, vm.TraverseItems[0].Distance);
        }

        [TestMethod]
        [TestCategory("Commands")]
        public void FlipBearingCommand_Execute_Less_Than_180()
        {
            var bearing = 90;
            var expectedValue = new Angle(270);

            Mock<IViewerService> mockService = new Mock<IViewerService>();
            var vm = new TraverseViewModel(mockService.Object, null);

            vm.AddRowCommand.CanExecute(true);
            vm.AddRowCommand.Execute(null);

            Assert.IsTrue(vm.TraverseItems.Count == 1);

            vm.TraverseItems[0].Bearing = bearing;

            vm.SelectedTraverseItem = vm.TraverseItems[0];
            
            vm.FlipBearingCommand.CanExecute(true);
            vm.FlipBearingCommand.Execute(null);

            Assert.AreEqual(expectedValue, vm.TraverseItems[0].Angle);
        }

        [TestMethod]
        [TestCategory("Commands")]
        public void FlipBearingCommand_Execute_Greater_Than_180()
        {
            var bearing = 270;
            var expectedValue = new Angle(90);

            Mock<IViewerService> mockService = new Mock<IViewerService>();
            var vm = new TraverseViewModel(mockService.Object, null);

            vm.AddRowCommand.CanExecute(true);
            vm.AddRowCommand.Execute(null);

            Assert.IsTrue(vm.TraverseItems.Count == 1);

            vm.TraverseItems[0].Bearing = bearing;

            vm.SelectedTraverseItem = vm.TraverseItems[0];
            
            vm.FlipBearingCommand.CanExecute(true);
            vm.FlipBearingCommand.Execute(null);

            Assert.AreEqual(expectedValue, vm.TraverseItems[0].Angle);
        }

        [TestMethod]
        [TestCategory("Commands")]
        public void FlipBearingCommand_Execute_SelectedTraverseAngle_Is_Null()
        {
            var bearing = 90;
            var expectedValue = new Angle(90);

            Mock<IViewerService> mockService = new Mock<IViewerService>();
            var vm = new TraverseViewModel(mockService.Object, null);

            vm.AddRowCommand.CanExecute(true);
            vm.AddRowCommand.Execute(null);

            Assert.IsTrue(vm.TraverseItems.Count == 1);

            vm.TraverseItems[0].Bearing = bearing;

            vm.SelectedTraverseItem = null;
            
            vm.FlipBearingCommand.CanExecute(true);
            vm.FlipBearingCommand.Execute(null);

            Assert.AreEqual(expectedValue, vm.TraverseItems[0].Angle);
        }
    }
}
