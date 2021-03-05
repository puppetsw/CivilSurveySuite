using System.Linq;

namespace _3DS_CivilSurveySuite.Traverse
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

        private static string ReplaceFirst(string text, string search, string replace)
        {
            int pos = text.IndexOf(search);
            if (pos < 0)
            {
                return text;
            }
            return text.Substring(0, pos) + replace + text.Substring(pos + search.Length);
        }

        private static string RemoveAlphaCharacters(string source)
        {
            var numbers = new[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
            var chars = new[] { '.', };

            return new string(source
                    .Where(x => numbers.Contains(x) || chars.Contains(x))
                    .ToArray()).Trim(chars);
        }
    }
}
