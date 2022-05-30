// Copyright Scott Whitney. All Rights Reserved.
// Reproduction or transmission in whole or in part, any form or by any
// means, electronic, mechanical or otherwise, is prohibited without the
// prior written consent of the copyright owner.

using System.Collections.Generic;
using _3DS_CivilSurveySuite.Shared.Models;
using _3DS_CivilSurveySuite.Shared.Services.Interfaces;
using _3DS_CivilSurveySuite.UI.ViewModels;
using Moq;
using NUnit.Framework;

namespace _3DS_CivilSurveySuiteTests
{
    [TestFixture]
    public class SelectAlignmentViewModelTests
    {
        [Test]
        public void AlignmentNames_OnConstruct()
        {
            var mock = new Mock<ICivilSelectService>();
            mock.Setup(m => m.GetAlignments()).Returns(() => new List<CivilAlignment>
            {
                new CivilAlignment(),
                new CivilAlignment(),
            });

            var vm = new SelectAlignmentViewModel(mock.Object);
            var expectedCount = 2;
            Assert.AreEqual(expectedCount, vm.Alignments.Count);
        }

        [Test]
        public void Set_SelectedAlignmentName_Property()
        {
            var mock = new Mock<ICivilSelectService>();
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

        [Test]
        public void SelectAlignmentCommand_Execute()
        {
            var selectableAlignment = new CivilAlignment() { Name = "EG" };

            var mock = new Mock<ICivilSelectService>();
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

        [Test]
        public void SelectAlignmentCommand_Execute_SurfaceExists()
        {
            var mock = new Mock<ICivilSelectService>();
            mock.Setup(m => m.GetAlignments()).Returns(() => new List<CivilAlignment>
            {
                new CivilAlignment { Name = "EG"},
                new CivilAlignment(),
            });
            mock.Setup(m => m.SelectAlignment()).Returns(() => new CivilAlignment { Name = "EG" });

            var vm = new SelectAlignmentViewModel(mock.Object);

            Assert.IsTrue(vm.SelectAlignmentCommand.CanExecute(true));
            vm.SelectAlignmentCommand.Execute(null);
        }

        [Test]
        public void SelectAlignmentCommand_Execute_SelectionCancelled()
        {
            var mock = new Mock<ICivilSelectService>();
            mock.Setup(m => m.GetAlignments()).Returns(() => new List<CivilAlignment>
            {
                new CivilAlignment { Name = "EG"},
                new CivilAlignment(),
            });
            mock.Setup(m => m.SelectAlignment()).Returns(() => null);

            var vm = new SelectAlignmentViewModel(mock.Object);

            Assert.IsTrue(vm.SelectAlignmentCommand.CanExecute(true));
            vm.SelectAlignmentCommand.Execute(null);
        }



    }
}