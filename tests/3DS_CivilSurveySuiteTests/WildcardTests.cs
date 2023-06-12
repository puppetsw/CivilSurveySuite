using System.Text.RegularExpressions;
using NUnit.Framework;

namespace CivilSurveySuiteTests
{
    [TestFixture]
    public class WildcardTests
    {
        [Test]
        public void Test_Wildcard_Regex1()
        {
            var deskey = "FP#*";
            var pattern = "\\A" + deskey.Replace("#", "\\d\\d?\\d?").Replace("*", ".*");

            string[] rawDesTrue = { "FP1 CONCRETE", "FP1S CONCRETE", "FP11S CONCRETE" };
            for (int i = 0; i < rawDesTrue.Length; i++)
            {
                Assert.AreEqual(true, Regex.Match(rawDesTrue[i], pattern).Success);
            }

            string[] rawDesFalse = { "FPH1 CONCRETE", "FPH1S CONCRETE", "CONCRETE FP11S CONCRETE" };
            for (int i = 0; i < rawDesFalse.Length; i++)
            {
                Assert.AreEqual(false, Regex.Match(rawDesFalse[i], pattern).Success);
            }

        }

        [Test]
        public void Test_Wildcard_Capture_Group()
        {
            var deskey = "FP#*";
            var pattern = "^" + deskey.Replace("#", "(\\d\\d?\\d?)").Replace("*", ".*");

            var expectedPattern = "^FP(\\d\\d?\\d?).*";
            Assert.AreEqual(expectedPattern, pattern); //check pattern match

            var rawDes = "FP12 CONCRETE";
            var expectedNumber = "12";

            var match = Regex.Match(rawDes, pattern, RegexOptions.IgnoreCase);

            Assert.AreEqual(expectedNumber, match.Groups[1].Value);

        }
    }
}