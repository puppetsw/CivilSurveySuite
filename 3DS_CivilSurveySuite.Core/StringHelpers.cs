// Copyright Scott Whitney. All Rights Reserved.
// Reproduction or transmission in whole or in part, any form or by any
// means, electronic, mechanical or otherwise, is prohibited without the
// prior written consent of the copyright owner.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace _3DS_CivilSurveySuite.Core
{
    public static class StringHelpers
    {
        public static string RemoveAlphaCharacters(string targetString)
        {
            var numbers = new[] { '0', '1', '2', '3', '4', '5', '6', '7', '8', '9' };
            var chars = new[] { '.', };

            return new string(targetString
                .Where(x => numbers.Contains(x) || chars.Contains(x))
                .ToArray()).Trim(chars);
        }

        public static double ExtractDoubleFromString(string targetString)
        {
            var digits = new Regex(@"^\D*?((-?(\d+(\.\d+)?))|(-?\.\d+)).*");
            var mx = digits.Match(targetString);

            return mx.Success ? Convert.ToDouble(mx.Groups[1].Value) : 0;
        }

        public static string RemoveWhitespace(string targetString)
        {
            return string.Concat(targetString.Where(c => !char.IsWhiteSpace(c)));
        }

        public static bool IsNumeric(this string text)
        {
            return double.TryParse(text, out double _);
        }

        public static string GetRangeString(IEnumerable<string> strings)
        {
            var cleanedString = strings.Where(IsNumeric).ToArray();
            int[] arr = Array.ConvertAll(cleanedString, int.Parse);

            // Return if array is null or contains less than 2 items
            if (!arr.Any()) 
                return string.Empty;

            if (arr.Length == 1) 
                return arr[0].ToString();

            Array.Sort(arr);

            var rangeString = new StringBuilder();
            bool isRange = false;

            for (int i = 0; i < arr.Length; i++)
            {
                while (i < arr.Length - 1 && arr[i] + 1 == arr[i + 1])
                {
                    if (!isRange) rangeString.Append($"{arr[i]}");
                    isRange = true;
                    i++;
                }

                if (isRange)
                {
                    rangeString.Append("-");
                    isRange = false;
                }

                rangeString.Append($"{arr[i]},");
            }

            return rangeString.ToString().TrimEnd(',');
        }

        public static string ToSentence(this string sourceString)
        {
            // start by converting entire string to lower case
            var lowerCase = sourceString.ToLower();
            // matches the first sentence of a string, as well as subsequent sentences
            var r = new Regex(@"(^[a-z])|\.\s+(.)", RegexOptions.ExplicitCapture);
            // MatchEvaluator delegate defines replacement of sentence starts to uppercase
            return r.Replace(lowerCase, s => s.Value.ToUpper());
        }
    }
}