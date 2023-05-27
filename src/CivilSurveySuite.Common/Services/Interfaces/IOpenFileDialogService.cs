namespace CivilSurveySuite.Shared.Services.Interfaces
{
    public interface IOpenFileDialogService
    {
        bool? ShowDialog();

        string FileName { get; set; }

        string DefaultExt { get; set; }

        string Filter { get; set; }
    }
}