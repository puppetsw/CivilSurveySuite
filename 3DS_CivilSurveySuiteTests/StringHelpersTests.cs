using _3DS_CivilSurveySuite.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace _3DS_CivilSurveySuiteTests
{
    [TestClass]
    public class StringHelpersTests
    {
        [TestMethod]
        [TestCategory("Helpers")]
        public void StringHelpers_RemoveAlphaCharactersTest()
        {
            var inputString = "ABCD100.00ABCD";

            var result = StringHelpers.RemoveAlphaCharacters(inputString);
            var expected = "100.00";

            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        [TestCategory("Helpers")]
        public void StringHelpers_ExtractDoubleFromStringTest()
        {
            var inputString = "100.00 ++ 100.00abcd!@#$%";

            var result = StringHelpers.ExtractDoubleFromString(inputString);
            var expected = 100.00;

            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        [TestCategory("Helpers")]
        public void StringHelpers_RemoveWhitespaceTest()
        {
            var inputString = " 100.00 ";

            var result = StringHelpers.RemoveWhitespace(inputString);
            var expected = "100.00";

            Assert.AreEqual(expected, result);
        }
    }
}