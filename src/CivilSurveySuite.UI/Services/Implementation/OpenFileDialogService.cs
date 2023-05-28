using CivilSurveySuite.Common.Services.Interfaces;
using Microsoft.Win32;

namespace CivilSurveySuite.UI.Services.Implementation
{
    public class OpenFileDialogService : IOpenFileDialogService
    {
        public bool? ShowDialog()
        {
            var dialog = new OpenFileDialog
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