// Copyright Scott Whitney. All Rights Reserved.
// Reproduction or transmission in whole or in part, any form or by any
// means, electronic, mechanical or otherwise, is prohibited without the
// prior written consent of the copyright owner.
// 
// Filename: DMSCalculatorTests.cs
// Date:     01/07/2021
// Author:   scott

using System;
using _3DS_CivilSurveySuite.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace _3DS_CivilSurveySuiteTests
{
    [TestClass]
    public class DMSCalculatorTests
    {
        [TestMethod]
        public void Add_TwoDMSLessThan360Degrees_ReturnSum()
        {
            var dms1 = new DMS { Degrees = 50, Minutes = 10, Seconds = 10 };
            var dms2 = new DMS { Degrees = 50, Minutes = 10, Seconds = 10 };

            var result = DMS.Add(dms1, dms2);

            var expected = new DMS { Degrees = 100, Minutes = 20, Seconds = 20 };

            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void Add_TwoDMSGreaterThan360Degrees_ReturnSum()
        {
            var dms1 = new DMS { Degrees = 360, Minutes = 0, Seconds = 0 };
            var dms2 = new DMS { Degrees = 100, Minutes = 0, Seconds = 0 };

            var result = DMS.Add(dms1, dms2);

            var expected = new DMS { Degrees = 460, Minutes = 0, Seconds = 0 };

            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void Subtract_TwoDMSLessThan0Degrees_ReturnSum()
        {
            var dms1 = new DMS { Degrees = 10, Minutes = 0, Seconds = 0 };
            var dms2 = new DMS { Degrees = 100, Minutes = 0, Seconds = 0 };

            var result = DMS.Subtract(dms1, dms2);

            var expected = new DMS { Degrees = -90, Minutes = 0, Seconds = 0 };

            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void Add_TwoDMSWithNegative()
        {
            var dms1 = new DMS { Degrees = 10, Minutes = 0, Seconds = 0 };
            var dms2 = new DMS { Degrees = -100, Minutes = 0, Seconds = 0 };

            var result = DMS.Add(dms1, dms2);

            var expected = new DMS { Degrees = -90, Minutes = 0, Seconds = 0};

            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void Test_PlusMinusDMS_PositiveIntoNegative()
        {
            var dms1 = new DMS { Degrees = 10, Minutes = 0, Seconds = 0 };
            var result = PlusMinusDMS(dms1);
            var expected = new DMS { Degrees = -10, Minutes = 0, Seconds = 0 };

            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void Test_PlusMinusDMS_NegativeIntoPositive()
        {
            var dms1 = new DMS { Degrees = -10, Minutes = 0, Seconds = 0 };
            var result = PlusMinusDMS(dms1);
            var expected = new DMS { Degrees = 10, Minutes = 0, Seconds = 0 };

            Assert.AreEqual(expected, result);
        }

        private static DMS PlusMinusDMS(DMS dms)
        {
            if (dms.Degrees > 0)
                return new DMS { Degrees = dms.Degrees * -1, Minutes = dms.Minutes, Seconds = dms.Seconds };
            else
                return new DMS { Degrees = Math.Abs(dms.Degrees), Minutes = dms.Minutes, Seconds = dms.Seconds };
        }
    }
}