using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using _3DS_CivilSurveySuite.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace _3DS_CivilSurveySuiteTests
{ 
    [TestClass]
    public class CivilObjectEqualityTests
    {
        //TODO: Implement tests for equality.


        [TestMethod]
        public void TestAlignmentEquality()
        {
            var alignment = new CivilAlignment { Name = "Test" };
            var alignment2 = new CivilAlignment { Name = "Test" };
            var list = new List<CivilAlignment>();


            list.Add(alignment);

            var result = list.Contains(alignment2);
            
            Assert.IsTrue(result);

        }



    }
}
