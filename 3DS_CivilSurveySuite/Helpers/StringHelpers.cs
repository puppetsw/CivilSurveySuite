using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace _3DS_CivilSurveySuite.Helpers
{
    public class StringHelpers
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="bearingWithSymbols"></param>
        /// <returns></returns>
        public static string StripDMSSymbols(string bearingWithSymbols)
        {
            //check if we have symbols?
            //TODO: what if only one symbol?
            string cleanedString = ReplaceFirst(bearingWithSymbols, "°", ".");
            cleanedString = RemoveAlphaCharacters(cleanedString);

            return cleanedString;
        }

        public static string ReplaceFirst(string text, string search, string replace)
        {
            int pos = text.IndexOf(search);
            if (pos < 0)
            {
                return text;
            }
            return text.Substring(0, pos) + replace + text.Substring(pos + search.Length);
        }

        public static string RemoveAlphaCharacters(string source)
        {
            var numbers = new[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
            var chars = new[] { '.', };

            return new string(source
                    .Where(x => numbers.Contains(x) || chars.Contains(x))
                    .ToArray()).Trim(chars);
        }

        public static double ExtractDoubleFromString(string str)
        {
            Regex digits = new Regex(@"^\D*?((-?(\d+(\.\d+)?))|(-?\.\d+)).*");
            Match mx = digits.Match(str);
            //Console.WriteLine("Input {0} - Digits {1} {2}", str, mx.Success, mx.Groups);

            return mx.Success ? Convert.ToDouble(mx.Groups[1].Value) : 0;
        }
    }
}
