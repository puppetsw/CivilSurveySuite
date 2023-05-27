using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CivilSurveySuite.Shared.Helpers
{
    public static class FileHelpers
    {
        /// <summary>
        /// Gets all files from a directory with passed extensions
        /// </summary>
        /// <param name="path"></param>
        /// <param name="extensions"></param>
        /// <returns></returns>
        public static IEnumerable<string> GetFiles(string path, IEnumerable<string> extensions)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException(nameof(path));

            if (!Directory.Exists(path))
                throw new DirectoryNotFoundException();

            string[] files = Directory.GetFiles(path);

            return files.Where(file => extensions.Contains(Path.GetExtension(file))).ToList();
        }

        /// <summary>
        /// Writes data to a file.
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="overWrite"></param>
        /// <param name="data"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public static void WriteFile(string fileName, bool overWrite, string data)
        {
            if (string.IsNullOrEmpty(fileName))
                throw new ArgumentNullException(nameof(fileName));

            if (File.Exists(fileName) && !overWrite)
                return;

            using (var writer = new StreamWriter(fileName))
            {
                writer.Write(data);
            }
        }
    }
}