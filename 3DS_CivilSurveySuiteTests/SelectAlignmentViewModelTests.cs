// Copyright Scott Whitney. All Rights Reserved.
// Reproduction or transmission in whole or in part, any form or by any
// means, electronic, mechanical or otherwise, is prohibited without the
// prior written consent of the copyright owner.

using System.Collections.Generic;
using _3DS_CivilSurveySuite.Model;
using _3DS_CivilSurveySuite.UI.Services;
using _3DS_CivilSurveySuite.UI.ViewModels;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace _3DS_CivilSurveySuiteTests
{
    [TestClass]
    public class SelectAlignmentViewModelTests
    {
        [TestMethod]
        public void AlignmentNames_OnConstruct()
        {
            var mock = new Mock<ISelectAlignmentService>();
            mock.Setup(m => m.GetAlignments()).Returns(() => new List<CivilAlignment>
            {
                new CivilAlignment(),
                new CivilAlignment(),
            });

            var vm = new SelectAlignmentViewModel(mock.Object);
            var expectedCount = 2;
            Assert.AreEqual(expectedCount, vm.Alignments.Count);
        }

        [TestMethod]
        public void Set_SelectedAlignmentName_Property()
        {
            var mock = new Mock<ISelectAlignmentService>();
            mock.Setup(m => m.GetAlignments()).Returns(() => new List<CivilAlignment>
            {
                new CivilAlignment() { Name = "EG"},
                new CivilAlignment(),
            });

            var vm = new SelectAlignmentViewModel(mock.Object);
            vm.SelectedAlignment = vm.Alignments[0];

            var expectedName = "EG";

            Assert.AreEqual(expectedName, vm.SelectedAlignment.Name);
        }

        [TestMethod]
        public void SelectAlignmentCommand_Execute()
        {
            var selectableAlignment = new CivilAlignment() { Name = "EG" };

            var mock = new Mock<ISelectAlignmentService>();
            mock.Setup(m => m.GetAlignments()).Returns(() => new List<CivilAlignment>
            {
                new CivilAlignment { Name = "Test" },
                selectableAlignment
            });

            mock.Setup(m => m.SelectAlignment()).Returns(() => selectableAlignment);

            var vm = new SelectAlignmentViewModel(mock.Object);
            vm.SelectAlignmentCommand.CanExecute(true);
            vm.SelectAlignmentCommand.Execute(null);
            
            Assert.AreEqual(selectableAlignment, vm.SelectedAlignment);
        }

        [TestMethod]
        public void SelectAlignmentCommand_Execute_SurfaceExists()
        {
            var mock = new Mock<ISelectAlignmentService>();
            mock.Setup(m => m.GetAlignments()).Returns(() => new List<CivilAlignment>
            {
                new CivilAlignment { Name = "EG"},
                new CivilAlignment(),
            });
            mock.Setup(m => m.SelectAlignment()).Returns(() => new CivilAlignment { Name = "EG" });

            var vm = new SelectAlignmentViewModel(mock.Object);

            vm.SelectAlignmentCommand.CanExecute(true);
            vm.SelectAlignmentCommand.Execute(null);
        }

        [TestMethod]
        public void SelectAlignmentCommand_Execute_SelectionCancelled()
        {
            var mock = new Mock<ISelectAlignmentService>();
            mock.Setup(m => m.GetAlignments()).Returns(() => new List<CivilAlignment>
            {
                new CivilAlignment { Name = "EG"},
                new CivilAlignment(),
            });
            mock.Setup(m => m.SelectAlignment()).Returns(() => null);

            var vm = new SelectAlignmentViewModel(mock.Object);

            vm.SelectAlignmentCommand.CanExecute(true);
            vm.SelectAlignmentCommand.Execute(null);
        }



    }
}