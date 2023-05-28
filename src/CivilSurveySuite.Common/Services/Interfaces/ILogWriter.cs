using System.Threading.Tasks;

namespace CivilSurveySuite.Common.Services.Interfaces
{
    public interface ILogWriter
    {
        Task WriteLineToLogAsync(string text);

        void WriteLineToLog(string text);

        string GetCurrentFileName();
    }
}
