﻿// Copyright Scott Whitney. All Rights Reserved.
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

            // Return fast if array is null or contains less than 2 items
            if (!arr.Any()) return string.Empty;
            if (arr.Length == 1) return arr[0].ToString();

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


        [Obsolete("This method is obsolete. Use GetRangeString(IEnumerable<string>) instead. To be removed.", true)]
        public static string RangeString(List<string> pointNumberList)
        {
            string left = "";
            string currentPointNumber = "";
            bool flag = false;
            SortNumericAlpha(ref pointNumberList);

            foreach (string str in pointNumberList)
            {
                if (!string.IsNullOrEmpty(currentPointNumber))
                {
                    if (IsNumeric(str))
                    {
                        if (Convert.ToInt32(str) == checked(Convert.ToInt32(currentPointNumber) + 1))
                        {
                            flag = true;
                        }
                        else
                        {
                            if (flag)
                                left = left + "-" + currentPointNumber;
                            left = left + "," + str;
                            flag = false;
                        }
                    }
                    else
                    {
                        left = left + "," + str;
                        flag = false;
                    }
                }

                currentPointNumber = str;

                // If it's the first one, set Left to str.
                if (string.IsNullOrEmpty(left))
                {
                    left = str;
                }
            }


            if (flag)
                left = left + "-" + currentPointNumber;
            return left;
        }

        public static void SortNumericAlpha(ref List<string> listToBeSorted)
        {
            if (listToBeSorted.Count <= 1)
                return;

            SortedDictionary<string, int> sortedDictionary = new SortedDictionary<string, int>();
            int num1 = listToBeSorted.Count - 1;
            int index = 0;
            while (index <= num1)
            {
                string str1 = string.Empty;
                string str2 = string.Empty;

                bool flag = false;
                string str3 = listToBeSorted[index].ToUpper() + " ";

                int num2 = str3.Length - 1;
                int startIndex = 0;
                while (startIndex <= num2)
                {
                    string str4 = str3.Substring(startIndex, 1);
                    if (IsNumeric(str4))
                    {
                        str2 += str4;
                        flag = true;
                    }
                    else
                    {
                        if (flag)
                        {
                            string str5 = new string('0', 10) + str2;
                            str1 += str5.Substring(checked (str5.Length - 8));
                            flag = false;
                            str2 = "";
                        }
                        str1 += str4;
                    }
                    startIndex++; 
                }
                string key = str1.Trim();
                if (!sortedDictionary.ContainsKey(key))
                    sortedDictionary.Add(key, index);
                checked { ++index; }
            }
            List<string> stringList = new List<string>();
            
            foreach (KeyValuePair<string, int> keyValuePair in sortedDictionary)
                stringList.Add(listToBeSorted[keyValuePair.Value]);
            
            listToBeSorted = stringList;
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