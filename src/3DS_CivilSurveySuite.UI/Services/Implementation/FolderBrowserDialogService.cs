// Copyright Scott Whitney. All Rights Reserved.
// Reproduction or transmission in whole or in part, any form or by any
// means, electronic, mechanical or otherwise, is prohibited without the
// prior written consent of the copyright owner.

using System.Windows.Forms;

namespace _3DS_CivilSurveySuite.UI.Services.Implementation
{
    public class FolderBrowserDialogService : IFolderBrowserDialogService
    {
        public bool? ShowDialog()
        {
            var dialog = new FolderBrowserDialog
            {
                ShowNewFolderButton = true,
                Description = Description,
            };

            var result = dialog.ShowDialog();

            if (result == DialogResult.OK)
            {
                SelectedPath = dialog.SelectedPath;
            }
            else
            {
                return false;
            }

            return true;
        }

        public string SelectedPath { get; set; }

        public string Description { get; set; }
    }
}