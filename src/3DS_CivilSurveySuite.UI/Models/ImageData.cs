// Copyright Scott Whitney. All Rights Reserved.
// Reproduction or transmission in whole or in part, any form or by any
// means, electronic, mechanical or otherwise, is prohibited without the
// prior written consent of the copyright owner.

using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows.Media.Imaging;

namespace _3DS_CivilSurveySuite.UI.Models
{
    /// <summary>
    /// Simple Image class to hold the image path and if it is selected.
    /// </summary>
    public class ImageData : INotifyPropertyChanged
    {
        private bool _isSelected;

        public string FileName { get; }

        public string Name { get; }

        public bool IsSelected
        {
            get => _isSelected;
            [DebuggerStepThrough]
            set
            {
                _isSelected = value;
                OnPropertyChanged();
            }
        }

        public BitmapFrame Image { get; }

        public double Width { get; }

        public double Height { get; }

        public double Ratio { get; }

        public ImageData(string fileName)
        {
            FileName = fileName;
            IsSelected = false;

            if (!File.Exists(FileName))
                throw new FileNotFoundException();

            Name = Path.GetFileNameWithoutExtension(fileName);
            Image = BitmapFrame.Create(new Uri(FileName));

            Width = Image.PixelWidth;
            Height = Image.PixelHeight;
            Ratio = Height / Width;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}