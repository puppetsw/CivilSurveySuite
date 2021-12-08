// Copyright Scott Whitney. All Rights Reserved.
// Reproduction or transmission in whole or in part, any form or by any
// means, electronic, mechanical or otherwise, is prohibited without the
// prior written consent of the copyright owner.

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Windows.Controls;
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

        public ObservableCollection<ImageData> Images
        {
            [DebuggerStepThrough]
            get;
            [DebuggerStepThrough]
            set;
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

        public ImageManagerViewModel(IFolderBrowserDialogService folderBrowserDialogService, IRasterImageService rasterImageService)
        {
            _folderBrowserDialogService = folderBrowserDialogService;
            _rasterImageService = rasterImageService;

            Images = new ObservableCollection<ImageData>();
        }

        private void InsertImages()
        {
            _rasterImageService.InsertRasterImage(GetSelectedImages(), 0.5, 0);
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
        }

        private IEnumerable<string> GetSelectedImages()
        {
            return from image in Images where image.IsSelected select image.FileName;
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