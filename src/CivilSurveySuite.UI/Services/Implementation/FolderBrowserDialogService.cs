using System.Windows.Forms;
using CivilSurveySuite.Common.Services.Interfaces;

namespace CivilSurveySuite.UI.Services.Implementation
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