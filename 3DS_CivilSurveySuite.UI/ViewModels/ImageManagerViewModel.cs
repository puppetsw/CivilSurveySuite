// Copyright Scott Whitney. All Rights Reserved.
// Reproduction or transmission in whole or in part, any form or by any
// means, electronic, mechanical or otherwise, is prohibited without the
// prior written consent of the copyright owner.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using _3DS_CivilSurveySuite.Core;
using _3DS_CivilSurveySuite.Model;
using _3DS_CivilSurveySuite.UI.Services;

namespace _3DS_CivilSurveySuite.UI.ViewModels
{
    /// <summary>
    /// ViewModel for ImageManagerView.xaml
    /// </summary>
    public class ImageManagerViewModel : ViewModelBase
    {
        private readonly IFolderBrowserDialogService _folderBrowserDialogService;
        private readonly IRasterImageService _rasterImageService;

        private double _imageWidth;
        private double _imageHeight;
        private double _imagePadding;
        private bool _lockAspectRatio;
        private int _rowLimit;

        private const double DEFAULT_RATIO = 0.6667;

        public ObservableCollection<ImageData> Images
        {
            [DebuggerStepThrough]
            get;
            [DebuggerStepThrough]
            set;
        }

        public bool LockAspectRatio
        {
            [DebuggerStepThrough]
            get => _lockAspectRatio;
            [DebuggerStepThrough]
            set
            {
                _lockAspectRatio = value;
                NotifyPropertyChanged();
            }
        }

        public double ImageWidth
        {
            [DebuggerStepThrough]
            get => _imageWidth;
            [DebuggerStepThrough]
            set
            {
                _imageWidth = value;
                NotifyPropertyChanged();
                NotifyPropertyChanged(nameof(ImageHeight)); // ImageHeight depends on ImageWidth
            }
        }

        public double ImageHeight
        {
            [DebuggerStepThrough]
            get
            {
                if (!LockAspectRatio)
                    return _imageHeight;

                // If the aspect ratio is locked, then the height is calculated from the width
                // Use the default ratio if no images loaded.
                if (Images.Count < 1)
                {
                    _imageHeight = Math.Round(ImageWidth * DEFAULT_RATIO, 1);
                    return _imageHeight;
                }

                double currentRatio = Images[0].Ratio;
                _imageHeight = Math.Round(ImageWidth * currentRatio, 1);
                return _imageHeight;
            }
            [DebuggerStepThrough]
            set
            {
                _imageHeight = value;
                NotifyPropertyChanged();
            }
        }

        public double ImagePadding
        {
            [DebuggerStepThrough]
            get => _imagePadding;
            [DebuggerStepThrough]
            set
            {
                _imagePadding = value;
                NotifyPropertyChanged();
            }

        }

        public int RowLimit
        {
            [DebuggerStepThrough]
            get => _rowLimit;
            [DebuggerStepThrough]
            set
            {
                _rowLimit = value;
                NotifyPropertyChanged();
            }
        }

        public RelayCommand LoadImagesCommand
        {
            [DebuggerStepThrough]
            get => new RelayCommand(LoadImages, () => true);
        }

        public RelayCommand InsertImagesCommand
        {
            [DebuggerStepThrough]
            get => new RelayCommand(InsertImages, () => true);
        }

        public RelayCommand SelectAllCommand
        {
            [DebuggerStepThrough]
            get => new RelayCommand(SelectAll, () => true);
        }

        public RelayCommand SelectNoneCommand
        {
            [DebuggerStepThrough]
            get => new RelayCommand(SelectNone, () => true);
        }

        public bool HasImages
        {
            [DebuggerStepThrough]
            get => Images.Count >= 1;
        }

        public ImageManagerViewModel(IFolderBrowserDialogService folderBrowserDialogService,
                                     IRasterImageService rasterImageService)
        {
            _folderBrowserDialogService = folderBrowserDialogService;
            _rasterImageService = rasterImageService;

            Images = new ObservableCollection<ImageData>();

            // Load default values.
            LockAspectRatio = true;
            ImageWidth = 1.5;
            ImagePadding = 0.5;
            RowLimit = 6;
        }

        private void InsertImages()
        {
            // Bit of a hack getting the first image ratio.
            _rasterImageService.InsertRasterImage(GetSelectedImages(), ImageWidth, ImageHeight, ImagePadding, RowLimit);
        }

        private void LoadImages()
        {
            if (_folderBrowserDialogService.ShowDialog() != true)
                return;

            var exts = new List<string> { ".jpg", ".jpeg", ".png", ".bmp" };
            IEnumerable<string> files = FileHelpers.GetFiles(_folderBrowserDialogService.SelectedPath, exts);

            // Add files to Images on a background thread.
            foreach (string file in files)
            {
                // Create new image object and select it by default.
                var imageData = new ImageData(file) { IsSelected = true };
                Images.Add(imageData);
            }

            NotifyPropertyChanged(nameof(HasImages)); // Is this needed?
        }

        private IEnumerable<string> GetSelectedImages()
        {
            var selectedImages = new List<string>();
            foreach (var imageData in Images)
            {
                if (imageData.IsSelected)
                    selectedImages.Add(imageData.FileName);
            }
            return selectedImages;
        }

        private void SelectAll()
        {
            foreach (var image in Images)
            {
                image.IsSelected = true;
            }
        }

        private void SelectNone()
        {
            foreach (var image in Images)
            {
                image.IsSelected = false;
            }
        }
    }
}