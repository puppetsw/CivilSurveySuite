// Copyright Scott Whitney. All Rights Reserved.
// Reproduction or transmission in whole or in part, any form or by any
// means, electronic, mechanical or otherwise, is prohibited without the
// prior written consent of the copyright owner.

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

        public ObservableCollection<ImageData> Images
        {
            [DebuggerStepThrough]
            get;
            [DebuggerStepThrough]
            set;
        }

        public RelayCommand LoadImagesCommand => new RelayCommand(LoadImages, () => true);


        public ImageManagerViewModel(IFolderBrowserDialogService folderBrowserDialogService)
        {
            _folderBrowserDialogService = folderBrowserDialogService;

            Images = new ObservableCollection<ImageData>();
        }

        private async void LoadImages()
        {
            if (_folderBrowserDialogService.ShowDialog() != true)
                return;

            var exts = new List<string> { ".jpg", ".jpeg", ".png", ".bmp" };
            var files = FileHelpers.GetFiles(_folderBrowserDialogService.SelectedPath, exts);

            // Add files to Images on a background thread.
            foreach (var file in files)
            {
                Images.Add(new ImageData(file));
            }
        }


    }
}