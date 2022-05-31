using System.Threading.Tasks;

namespace _3DS_CivilSurveySuite.Shared.Services.Interfaces
{
    public interface ILogWriter
    {
        Task WriteLineToLogAsync(string text);

        void WriteLineToLog(string text);

        string GetCurrentFileName();
    }
}
