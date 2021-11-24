using System.Collections.Generic;
using _3DS_CivilSurveySuite.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace _3DS_CivilSurveySuiteTests
{
    [TestClass]
    public class StringHelpersTests
    {
        [TestMethod]
        public void StringHelpers_RemoveAlphaCharactersTest()
        {
            var inputString = "ABCD100.00ABCD";

            var result = StringHelpers.RemoveAlphaCharacters(inputString);
            var expected = "100.00";

            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void StringHelpers_ExtractDoubleFromStringTest()
        {
            var inputString = "100.00 ++ 100.00abcd!@#$%";

            var result = StringHelpers.ExtractDoubleFromString(inputString);
            var expected = 100.00;

            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void StringHelpers_RemoveWhitespaceTest()
        {
            var inputString = " 100.00 ";

            var result = StringHelpers.RemoveWhitespace(inputString);
            var expected = "100.00";

            Assert.AreEqual(expected, result);
        }

        [TestMethod]
        public void IsNumeric_ShouldBeTrue()
        {
            var testString = "1";
            var result = StringHelpers.IsNumeric(testString);
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void IsNumeric_ShouldBeFalse()
        {
            var testString = "NotANumber1";
            var result = StringHelpers.IsNumeric(testString);
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void RangeString_CorrectRange()
        {
            var expectedString = "1-4,10-12";
            var list = new List<string> { "1", "2", "3", "4", "10", "11", "12" };
            var result = StringHelpers.GetRangeString(list);
            Assert.AreEqual(expectedString, result);
        }

        [TestMethod]
        public void RangeString_EmptyArray()
        {
            var expectedString = string.Empty;
            var result = StringHelpers.GetRangeString(new List<string>());
            Assert.AreEqual(expectedString, result);
        }

        [TestMethod]
        public void RangeString_OneValue()
        {
            var expectedString = "1";
            var list = new List<string> { "1" };
            var result = StringHelpers.GetRangeString(list);
            Assert.AreEqual(expectedString, result);
        }

        [TestMethod]
        public void RangeString_InvalidValueInList()
        {
            var expectedString = "1-4,10-12";
            var list = new[] { "1", "2", "3", "test","4", "10", "11", "12" };
            var result = StringHelpers.GetRangeString(list);
            Assert.AreEqual(expectedString, result);
        }

        [TestMethod]
        public void GetRangeString()
        {
            var array = new[] { "1", "2", "10", "11", "12" };
            var expectedString = "1-2,10-12";

            var result = StringHelpers.GetRangeString(array);

            Assert.AreEqual(expectedString, result);
        }

        [TestMethod]
        public void SentenceCase()
        {
            var sourceString = "THIS IS A SENTENCE.";

            var expectedString = "This is a sentence.";

            var result = sourceString.ToSentence();

            Assert.AreEqual(expectedString, result);
        }

        [TestMethod]
        public void SentenceCase_MultiSentence()
        {
            var sourceString = "THIS IS A SENTENCE. THIS IS ANOTHER SENTENCE";

            var expectedString = "This is a sentence. This is another sentence";

            var result = sourceString.ToSentence();

            Assert.AreEqual(expectedString, result);
        }

        [TestMethod]
        public void Replace_First_Occurrence()
        {
            string sourceString = "This is {} a test {} string";
            string expectedString = "This is  a test {} string";

            var result = sourceString.ReplaceFirst("{}", "");
            Assert.AreEqual(expectedString, result);
        }

        [TestMethod]
        public void Replace_First_Occurrence_Not_Found()
        {
            string sourceString = "This is {} a test {} string";

            var result = sourceString.ReplaceFirst("#", "");
            Assert.AreEqual(sourceString, result);
        }
    }
}