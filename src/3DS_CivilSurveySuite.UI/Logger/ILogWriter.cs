using System.Threading.Tasks;

namespace _3DS_CivilSurveySuite.UI.Logger
{
    public interface ILogWriter
    {
        Task WriteLineToLogAsync(string text);

        void WriteLineToLog(string text);
    }
}
