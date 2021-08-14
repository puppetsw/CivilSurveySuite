using System.Linq;
using _3DS_CivilSurveySuite.Model;
using _3DS_CivilSurveySuite.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
// ReSharper disable ExpressionIsAlwaysNull

namespace _3DS_CivilSurveySuiteTests
{
    [TestClass]
    public class TraverseAngleViewModelTests
    {
        [TestMethod]
        [TestCategory("Properties")]
        public void CloseBearing_Property_Change()
        {
            var result = false;
            var expectedString = "100.1000";
            Mock<IViewerService> mockService = new Mock<IViewerService>();
            var vm = new TraverseAngleViewModel(mockService.Object, null);
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
            var vm = new TraverseAngleViewModel(mockService.Object, null);
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
        [TestCategory("Properties")]
        public void RotationDirectionValues_Get()
        {
            Mock<IViewerService> mockService = new Mock<IViewerService>();
            var vm = new TraverseAngleViewModel(mockService.Object, null);
            var values = vm.RotationDirectionValues;
            var expectedNumberOfValues = 2;

            Assert.AreEqual(expectedNumberOfValues, values.Count());
        }

        [TestMethod]
        [TestCategory("Properties")]
        public void ReferenceDirectionValues_Get()
        {
            Mock<IViewerService> mockService = new Mock<IViewerService>();
            var vm = new TraverseAngleViewModel(mockService.Object, null);
            var values = vm.ReferenceDirectionValues;
            var expectedNumberOfValues = 2;

            Assert.AreEqual(expectedNumberOfValues, values.Count());
        }

        [TestMethod]
        [TestCategory("Commands")]
        public void AddRowCommand_Execute()
        {
            Mock<IViewerService> mockService = new Mock<IViewerService>();
            var vm = new TraverseAngleViewModel(mockService.Object, null);

            vm.AddRowCommand.CanExecute(true);
            vm.AddRowCommand.Execute(null);

            Assert.IsTrue(vm.TraverseAngles.Count == 1);
            Assert.AreEqual(0, vm.TraverseAngles[0].Index);
        }

        [TestMethod]
        [TestCategory("Commands")]
        public void AddRowCommand_Execute_MultipleAdd_IndexShouldIncrease()
        {
            Mock<IViewerService> mockService = new Mock<IViewerService>();
            var vm = new TraverseAngleViewModel(mockService.Object, null);

            vm.AddRowCommand.CanExecute(true);
            vm.AddRowCommand.Execute(null);
            vm.AddRowCommand.Execute(null);
            vm.AddRowCommand.Execute(null);

            Assert.IsTrue(vm.TraverseAngles.Count == 3);
            Assert.AreEqual(0, vm.TraverseAngles[0].Index);
            Assert.AreEqual(1, vm.TraverseAngles[1].Index);
            Assert.AreEqual(2, vm.TraverseAngles[2].Index);
        }

        [TestMethod]
        [TestCategory("Commands")]
        public void RemoveRowCommand_Execute()
        {
            Mock<IViewerService> mockService = new Mock<IViewerService>();
            var vm = new TraverseAngleViewModel(mockService.Object, null);

            vm.AddRowCommand.CanExecute(true);
            vm.AddRowCommand.Execute(null);

            Assert.IsTrue(vm.TraverseAngles.Count == 1);

            vm.SelectedTraverseAngle = vm.TraverseAngles[0];

            vm.RemoveRowCommand.CanExecute(true);
            vm.RemoveRowCommand.Execute(null);

            Assert.IsTrue(vm.TraverseAngles.Count == 0);
        }

        [TestMethod]
        [TestCategory("Commands")]
        public void RemoveRowCommand_Execute_SelectedTraverseAngle_Is_Null()
        {
            Mock<IViewerService> mockService = new Mock<IViewerService>();
            var vm = new TraverseAngleViewModel(mockService.Object, null);

            vm.AddRowCommand.CanExecute(true);
            vm.AddRowCommand.Execute(null);

            Assert.IsTrue(vm.TraverseAngles.Count == 1);

            vm.SelectedTraverseAngle = null;

            vm.RemoveRowCommand.CanExecute(true);
            vm.RemoveRowCommand.Execute(null);

            Assert.IsTrue(vm.TraverseAngles.Count == 1);
        }

        [TestMethod]
        [TestCategory("Commands")]
        public void ClearTraverseCommand_Execute()
        {
            Mock<IViewerService> mockService = new Mock<IViewerService>();
            var vm = new TraverseAngleViewModel(mockService.Object, null);

            vm.AddRowCommand.CanExecute(true);
            vm.AddRowCommand.Execute(null);

            Assert.IsTrue(vm.TraverseAngles.Count == 1);

            vm.ClearCommand.CanExecute(true);
            vm.ClearCommand.Execute(null);

            Assert.IsTrue(vm.TraverseAngles.Count == 0);
        }

        [TestMethod]
        [TestCategory("Commands")]
        public void GridUpdatedCommand_Execute()
        {
            Mock<IViewerService> mockService = new Mock<IViewerService>();
            var vm = new TraverseAngleViewModel(mockService.Object, null);

            vm.TraverseAngles.Add(new TraverseAngleObject(0, 30));
            vm.TraverseAngles.Add(new TraverseAngleObject(90, 10));
            vm.TraverseAngles.Add(new TraverseAngleObject(90, 30));

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
            var vm = new TraverseAngleViewModel(mockService.Object, null);

            vm.TraverseAngles.Add(new TraverseAngleObject(0, 30));

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
            var vm = new TraverseAngleViewModel(mockService.Object, null);

            vm.AddRowCommand.CanExecute(true);
            vm.AddRowCommand.Execute(null);

            Assert.IsTrue(vm.TraverseAngles.Count == 1);

            vm.SelectedTraverseAngle = vm.TraverseAngles[0];
            vm.SelectedTraverseAngle.Distance = feetAndInchesValue;

            vm.FeetToMetersCommand.CanExecute(true);
            vm.FeetToMetersCommand.Execute(null);

            Assert.AreEqual(expectedValue, vm.TraverseAngles[0].Distance);
        }

        [TestMethod]
        [TestCategory("Commands")]
        public void FeetToMetersCommand_Execute_SelectedTraverseAngle_Is_Null()
        {
            var feetAndInchesValue = 100.10;
            var expectedValue = 100.10;

            Mock<IViewerService> mockService = new Mock<IViewerService>();
            var vm = new TraverseAngleViewModel(mockService.Object, null);

            vm.AddRowCommand.CanExecute(true);
            vm.AddRowCommand.Execute(null);
            vm.TraverseAngles[0].Distance = feetAndInchesValue;

            vm.SelectedTraverseAngle = null;

            vm.FeetToMetersCommand.CanExecute(true);
            vm.FeetToMetersCommand.Execute(null);

            Assert.AreEqual(expectedValue, vm.TraverseAngles[0].Distance);
        }

        [TestMethod]
        [TestCategory("Commands")]
        public void LinksToMetersCommand_Execute()
        {
            var linkValue = 100;
            var expectedValue = 20.1168;

            Mock<IViewerService> mockService = new Mock<IViewerService>();
            var vm = new TraverseAngleViewModel(mockService.Object, null);

            vm.AddRowCommand.CanExecute(true);
            vm.AddRowCommand.Execute(null);

            Assert.IsTrue(vm.TraverseAngles.Count == 1);

            vm.SelectedTraverseAngle = vm.TraverseAngles[0];
            vm.SelectedTraverseAngle.Distance = linkValue;

            vm.LinksToMetersCommand.CanExecute(true);
            vm.LinksToMetersCommand.Execute(null);

            Assert.AreEqual(expectedValue, vm.TraverseAngles[0].Distance);
        }

        [TestMethod]
        [TestCategory("Commands")]
        public void LinksToMetersCommand_Execute_SelectedTraverseAngle_Is_Null()
        {
            var linkValue = 100;
            var expectedValue = 100;

            Mock<IViewerService> mockService = new Mock<IViewerService>();
            var vm = new TraverseAngleViewModel(mockService.Object, null);

            vm.AddRowCommand.CanExecute(true);
            vm.AddRowCommand.Execute(null);
            vm.TraverseAngles[0].Distance = linkValue;

            vm.SelectedTraverseAngle = null;

            vm.LinksToMetersCommand.CanExecute(true);
            vm.LinksToMetersCommand.Execute(null);

            Assert.AreEqual(expectedValue, vm.TraverseAngles[0].Distance);
        }

        [TestMethod]
        [TestCategory("Commands")]
        public void FlipBearingCommand_Execute_Less_Than_180()
        {
            var bearing = 90;
            var expectedValue = new Angle(270);

            Mock<IViewerService> mockService = new Mock<IViewerService>();
            var vm = new TraverseAngleViewModel(mockService.Object, null);

            vm.AddRowCommand.CanExecute(true);
            vm.AddRowCommand.Execute(null);

            Assert.IsTrue(vm.TraverseAngles.Count == 1);

            vm.TraverseAngles[0].Bearing = bearing;

            vm.SelectedTraverseAngle = vm.TraverseAngles[0];
            
            vm.FlipBearingCommand.CanExecute(true);
            vm.FlipBearingCommand.Execute(null);

            Assert.AreEqual(expectedValue, vm.TraverseAngles[0].Angle);
        }

        [TestMethod]
        [TestCategory("Commands")]
        public void FlipBearingCommand_Execute_Greater_Than_180()
        {
            var bearing = 270;
            var expectedValue = new Angle(90);

            Mock<IViewerService> mockService = new Mock<IViewerService>();
            var vm = new TraverseAngleViewModel(mockService.Object, null);

            vm.AddRowCommand.CanExecute(true);
            vm.AddRowCommand.Execute(null);

            Assert.IsTrue(vm.TraverseAngles.Count == 1);

            vm.TraverseAngles[0].Bearing = bearing;

            vm.SelectedTraverseAngle = vm.TraverseAngles[0];
            
            vm.FlipBearingCommand.CanExecute(true);
            vm.FlipBearingCommand.Execute(null);

            Assert.AreEqual(expectedValue, vm.TraverseAngles[0].Angle);
        }

        [TestMethod]
        [TestCategory("Commands")]
        public void FlipBearingCommand_Execute_SelectedTraverseAngle_Is_Null()
        {
            var bearing = 90;
            var expectedValue = new Angle(90);

            Mock<IViewerService> mockService = new Mock<IViewerService>();
            var vm = new TraverseAngleViewModel(mockService.Object, null);

            vm.AddRowCommand.CanExecute(true);
            vm.AddRowCommand.Execute(null);

            Assert.IsTrue(vm.TraverseAngles.Count == 1);

            vm.TraverseAngles[0].Bearing = bearing;

            vm.SelectedTraverseAngle = null;
            
            vm.FlipBearingCommand.CanExecute(true);
            vm.FlipBearingCommand.Execute(null);

            Assert.AreEqual(expectedValue, vm.TraverseAngles[0].Angle);
        }

        [TestMethod]
        [TestCategory("Commands")]
        public void DrawCommand_Execute()
        {
            Mock<ITraverseService> traverseService = new Mock<ITraverseService>();
            Mock<IViewerService> mockService = new Mock<IViewerService>();
            var vm = new TraverseAngleViewModel(mockService.Object, traverseService.Object);

            vm.DrawCommand.CanExecute(true);
            vm.DrawCommand.Execute(null);
        }
    }
}
