using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace _3DS_CivilSurveySuiteTests
{
    [TestClass]
    public class MathStringTests
    {
        [TestMethod]
        public void TestMathString()
        {
            //sample strings
            //100.00 + 100.00
            //100.00+100.00
            //100+100

            string mathInput = "100.00 ++ 100.00asdkljaskjd@#$#!";

            char[] charArray = mathInput.ToCharArray();
            charArray = Array.FindAll<char>(charArray, (c => (char.IsDigit(c) || c == '-' || c == '+' || c == '.')));
            var str = new string(charArray); //convert character array back to string

            string[] numbersArray = str.Split(new string[] { "+", "-" }, StringSplitOptions.RemoveEmptyEntries);

            if (mathInput.Contains("+"))
                return;
            else if (mathInput.Contains("-"))
                return;


        }

        public static string RemoveWhitespace(string targetString)
        {
            return string.Concat(targetString.Where(c => !char.IsWhiteSpace(c)));
        }
    }
}
