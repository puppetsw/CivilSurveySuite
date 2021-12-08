// Copyright Scott Whitney. All Rights Reserved.
// Reproduction or transmission in whole or in part, any form or by any
// means, electronic, mechanical or otherwise, is prohibited without the
// prior written consent of the copyright owner.

using System;
using System.Collections.Generic;
using System.IO;

namespace _3DS_CivilSurveySuite.Core
{
    public static class FileHelpers
    {
        /// <summary>
        /// Gets all files from a directory with passed extensions
        /// </summary>
        /// <param name="path"></param>
        /// <param name="extensions"></param>
        /// <returns></returns>
        public static IEnumerable<string> GetFiles(string path, IList<string> extensions)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException(nameof(path));

            if (!Directory.Exists(path))
                throw new DirectoryNotFoundException();

            var images = new List<string>();
            string[] files = Directory.GetFiles(path);

            foreach (string file in files)
            {
                if (extensions.Contains(Path.GetExtension(file)))
                    images.Add(file);
            }
            return images;
        }
    }
}