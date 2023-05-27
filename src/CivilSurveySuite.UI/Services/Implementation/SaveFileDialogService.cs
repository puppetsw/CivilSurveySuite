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