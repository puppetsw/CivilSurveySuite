// Copyright Scott Whitney. All Rights Reserved.
// Reproduction or transmission in whole or in part, any form or by any
// means, electronic, mechanical or otherwise, is prohibited without the
// prior written consent of the copyright owner.
//
// Filename: BearingConverterTests.cs
// Date:     01/07/2021
// Author:   scott

using _3DS_CivilSurveySuite.UI.Models;
using NUnit.Framework;

namespace _3DS_CivilSurveySuiteTests
{
    [TestFixture]
    public class BearingConverterTests
    {
        [Test]
        public void Test_DMS_ToString()
        {
            var dms = new Angle { Degrees = 225, Minutes = 21, Seconds = 57 };
            string bearing = dms.ToString();

            const string expectedResult = "225°21'57\"";

            Assert.AreEqual(expectedResult, bearing);
        }

        [Test]
        public void Test_DMS_ToDouble()
        {
            var dms = new Angle { Degrees = 225, Minutes = 21, Seconds = 57 };
            double bearing = dms.ToDouble();

            const double expectedResult = 225.2157;

            Assert.AreEqual(expectedResult, bearing);
        }
    }
}