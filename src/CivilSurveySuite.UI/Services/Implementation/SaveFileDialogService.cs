﻿// Copyright Scott Whitney. All Rights Reserved.
// Reproduction or transmission in whole or in part, any form or by any
// means, electronic, mechanical or otherwise, is prohibited without the
// prior written consent of the copyright owner.

using CivilSurveySuite.Shared.Services.Interfaces;
using Microsoft.Win32;

namespace CivilSurveySuite.UI.Services.Implementation
{
    public class SaveFileDialogService : ISaveFileDialogService
    {
        public bool? ShowDialog()
        {
            var dialog = new SaveFileDialog
            {
                DefaultExt = DefaultExt,
                Filter = Filter
            };

            var result = dialog.ShowDialog();

            if (result == true)
            {
                FileName = dialog.FileName;
            }

            return result;
        }

        public string FileName { get; set; }

        public string DefaultExt { get; set; }

        public string Filter { get; set; }
    }
}