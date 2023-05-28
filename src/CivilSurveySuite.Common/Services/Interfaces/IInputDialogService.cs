using CivilSurveySuite.Common.Models;

namespace CivilSurveySuite.Common.Services.Interfaces
{
    public interface IInputDialogService
    {
        bool? DialogResult { get; set; }

        string ResultString { get; set; }

        bool? ShowDialog();

        void AssignOptions(InputServiceOptions options);
    }
}