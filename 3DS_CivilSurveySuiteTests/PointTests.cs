// Copyright Scott Whitney. All Rights Reserved.
// Reproduction or transmission in whole or in part, any form or by any
// means, electronic, mechanical or otherwise, is prohibited without the
// prior written consent of the copyright owner.

using _3DS_CivilSurveySuite.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace _3DS_CivilSurveySuiteTests
{
    [TestClass]
    public class PointTests
    {
        [TestMethod]
        public void IsValid_ShouldBeTrue()
        {
            var point = new Point(100, 100, 50);

            var result = point.IsValid();

            Assert.IsTrue(result);
        }

        [TestMethod]
        public void IsValid_ShouldBeFalse()
        {
            var point = new Point(double.MinValue, 0, 0);

            var result = point.IsValid();

            Assert.IsFalse(result);
        }

        [TestMethod]
        public void ToStringTest()
        {
            var point = new Point(0, 0, 0);

            var expectedString = "X:0,Y:0,Z:0";

            var result = point.ToString();

            Assert.AreEqual(expectedString, result);
        }
    }
}