using System.Threading.Tasks;

namespace CivilSurveySuite.Shared.Services.Interfaces
{
    public interface ILogWriter
    {
        Task WriteLineToLogAsync(string text);

        void WriteLineToLog(string text);

        string GetCurrentFileName();
    }
}
