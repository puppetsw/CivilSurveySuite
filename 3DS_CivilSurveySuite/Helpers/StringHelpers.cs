// Copyright Scott Whitney. All Rights Reserved.
// Reproduction or transmission in whole or in part, any form or by any
// means, electronic, mechanical or otherwise, is prohibited without the
// prior written consent of the copyright owner.
// 
// Filename: StringHelpers.cs
// Date:     01/07/2021
// Author:   scott

using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace _3DS_CivilSurveySuite.Helpers
{
    public static class StringHelpers
    {
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
            var digits = new Regex(@"^\D*?((-?(\d+(\.\d+)?))|(-?\.\d+)).*");
            var mx = digits.Match(str);

            return mx.Success ? Convert.ToDouble(mx.Groups[1].Value) : 0;
        }

        public static string RemoveWhitespace(string targetString)
        {
            return string.Concat(targetString.Where(c => !char.IsWhiteSpace(c)));
        }
    }
}