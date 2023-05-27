namespace CivilSurveySuite.Shared.Services.Interfaces
{
    /// <summary>
    /// Interface IDialogService
    /// </summary>
    public interface IDialogService<T>
    {
        bool? DialogResult { get; set; }

        T ResultObject { get; set; }

        bool? ShowDialog();
    }
}