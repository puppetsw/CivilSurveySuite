// Copyright Scott Whitney. All Rights Reserved.
// Reproduction or transmission in whole or in part, any form or by any
// means, electronic, mechanical or otherwise, is prohibited without the
// prior written consent of the copyright owner.

using System;
using System.IO;
using System.Windows.Media.Imaging;

namespace _3DS_CivilSurveySuite.Model
{
    /// <summary>
    /// Simple Image class to hold the image path and if it is selected.
    /// </summary>
    public class ImageData
    {
        public string FilePath { get; set; }

        public bool IsSelected { get; set; }

        public BitmapImage Image { get; private set; }

        public ImageData(string filePath)
        {
            FilePath = filePath;
            IsSelected = false;

            if (!File.Exists(FilePath))
                throw new FileNotFoundException();

            // Load BitmapImage from filepath
            Image = new BitmapImage(new Uri(FilePath));
        }
    }
}