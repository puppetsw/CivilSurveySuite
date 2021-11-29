// Copyright Scott Whitney. All Rights Reserved.
// Reproduction or transmission in whole or in part, any form or by any
// means, electronic, mechanical or otherwise, is prohibited without the
// prior written consent of the copyright owner.

using System.IO;

namespace _3DS_CivilSurveySuite.Core
{
    /// <summary>
    /// Class to handle reading and writing from CSV files.
    /// </summary>
    public static class CsvHelpers
    {
        /// <summary>
        /// Reads a CSV file and returns a string array of the lines in the file.
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static string[,] ReadCsv(string filePath)
        {
            var lines = File.ReadAllLines(filePath);
            var result = new string[lines.Length, lines[0].Split(',').Length];
            for (var i = 0; i < lines.Length; i++)
            {
                var values = lines[i].Split(',');
                for (var j = 0; j < values.Length; j++)
                {
                    result[i, j] = values[j];
                }
            }
            return result;
        }

        /// <summary>
        /// Writes a string array to a CSV file.
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="data"></param>
        public static void WriteCsv(string filePath, string[,] data)
        {
            var lines = new string[data.GetLength(0)];
            for (var i = 0; i < data.GetLength(0); i++)
            {
                lines[i] = string.Join(",", data[i, 0].Length);
            }
            File.WriteAllLines(filePath, lines);
        }
    }
}