﻿using System.Linq;
using _3DS_CivilSurveySuite.Model;
using _3DS_CivilSurveySuite.UI.Services;
using _3DS_CivilSurveySuite.UI.ViewModels;
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
            var vm = new TraverseAngleViewModel(null, null);
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
            var vm = new TraverseAngleViewModel(null, null);
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
            var vm = new TraverseAngleViewModel(null, null);
            var values = vm.RotationDirectionValues;
            var expectedNumberOfValues = 2;

            Assert.AreEqual(expectedNumberOfValues, values.Count());
        }

        [TestMethod]
        [TestCategory("Properties")]
        public void ReferenceDirectionValues_Get()
        {
            var vm = new TraverseAngleViewModel(null, null);
            var values = vm.ReferenceDirectionValues;
            var expectedNumberOfValues = 2;

            Assert.AreEqual(expectedNumberOfValues, values.Count());
        }

        [TestMethod]
        [TestCategory("Commands")]
        public void AddRowCommand_Execute()
        {
            var vm = new TraverseAngleViewModel(null, null);

            vm.AddRowCommand.CanExecute(true);
            vm.AddRowCommand.Execute(null);

            Assert.IsTrue(vm.TraverseAngles.Count == 1);
            Assert.AreEqual(0, vm.TraverseAngles[0].Index);
        }

        [TestMethod]
        [TestCategory("Commands")]
        public void AddRowCommand_Execute_MultipleAdd_IndexShouldIncrease()
        {
            var vm = new TraverseAngleViewModel(null, null);

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
            var vm = new TraverseAngleViewModel(null, null);

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
            var vm = new TraverseAngleViewModel(null, null);

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
            var vm = new TraverseAngleViewModel(null, null);

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
            var vm = new TraverseAngleViewModel(null, null);

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
            var vm = new TraverseAngleViewModel(null, null);

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

            var vm = new TraverseAngleViewModel(null, null);

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

            var vm = new TraverseAngleViewModel(null, null);

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

            var vm = new TraverseAngleViewModel(null, null);

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

            var vm = new TraverseAngleViewModel(null, null);

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

            var vm = new TraverseAngleViewModel(null, null);

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

            var vm = new TraverseAngleViewModel(null, null);

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

            var vm = new TraverseAngleViewModel(null, null);

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
            var vm = new TraverseAngleViewModel(traverseService.Object, null);

            vm.DrawCommand.CanExecute(true);
            vm.DrawCommand.Execute(null);
        }

        [TestMethod]
        public void ShowHelpCommand_Execute()
        {
            var mockTrav = new Mock<ITraverseService>();
            var mockProc = new Mock<IProcessService>();

            var vm = new TraverseAngleViewModel(mockTrav.Object, mockProc.Object);
            vm.ShowHelpCommand.CanExecute(true);
            vm.ShowHelpCommand.Execute(true);
        }

        
        [TestMethod]
        public void CloseWindowCommand_Execute()
        {
            var travServ = new Mock<ITraverseService>();
            var procServ = new Mock<IProcessService>();

            var vm = new TraverseAngleViewModel(travServ.Object, procServ.Object);

            vm.CloseWindowCommand.CanExecute(true);
            vm.CloseWindowCommand.Execute(null);
        }

        [TestMethod]
        public void SetBasePointCommand_Execute()
        {
            var travServ = new Mock<ITraverseService>();
            var procServ = new Mock<IProcessService>();

            var vm = new TraverseAngleViewModel(travServ.Object, procServ.Object);

            vm.SetBasePointCommand.CanExecute(true);
            vm.SetBasePointCommand.Execute(null);
        }

        [TestMethod]
        public void SetBasePointCommand_Execute_TraverseCountGreaterThanZero()
        {
            var travServ = new Mock<ITraverseService>();
            var procServ = new Mock<IProcessService>();

            var vm = new TraverseAngleViewModel(travServ.Object, procServ.Object);

            vm.TraverseAngles.Add(new TraverseAngleObject());
            vm.TraverseAngles.Add(new TraverseAngleObject());

            vm.SetBasePointCommand.CanExecute(true);
            vm.SetBasePointCommand.Execute(null);
        }




    }
}
