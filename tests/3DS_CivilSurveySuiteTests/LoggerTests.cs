﻿using System.Threading.Tasks;
using CivilSurveySuite.Shared.Services.Interfaces;
using CivilSurveySuite.UI.Logger;
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
