using System.Threading.Tasks;
using _3DS_CivilSurveySuite.Shared.Services.Interfaces;
using _3DS_CivilSurveySuite.UI.Logger;
using NUnit.Framework;

namespace _3DS_CivilSurveySuiteTests
{
    [TestFixture]
    public class LoggerTests
    {
        [Test]
        public async Task CreateLogFileAndWriteTest()
        {
            ILogWriter logWriter = new LogWriter();
            await logWriter.WriteLineToLogAsync("Test logging");
        }
    }
}
