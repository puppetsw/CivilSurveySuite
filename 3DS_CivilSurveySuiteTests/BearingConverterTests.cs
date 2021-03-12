using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static _3DS_CivilSurveySuiteTests.TraverseTests;

namespace _3DS_CivilSurveySuiteTests
{
    [TestClass]
    public class BearingConverterTests
    {
        [TestMethod]
        public void TestBearingString()
        {
            DMS dms = new DMS() { Degrees = 225, Minutes = 21, Seconds = 57 };
            string bearing = dms.ToString();
            
        }


    }
}
