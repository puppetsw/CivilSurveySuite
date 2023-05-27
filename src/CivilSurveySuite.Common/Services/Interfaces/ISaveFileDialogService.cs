namespace CivilSurveySuite.Shared.Services.Interfaces
{
    public interface ISaveFileDialogService
    {
        bool? ShowDialog();

        string FileName { get; set; }

        string DefaultExt { get; set; }

        string Filter { get; set; }
    }
}