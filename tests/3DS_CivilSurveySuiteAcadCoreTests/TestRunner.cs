using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Autodesk.AutoCAD.Runtime;
using CivilSurveySuiteAcadCoreTests;
using NUnitLite;

[assembly: CommandClass(typeof(TestRunner))]
namespace CivilSurveySuiteAcadCoreTests
{
    public static class TestRunner
    {
        [CommandMethod("RunTests", CommandFlags.Session)]
        public static void RunTests()
        {
            string directoryPlugin = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            if (directoryPlugin == null)
                return;

            string directoryReportUnit = Path.Combine(directoryPlugin, @"ReportUnit");
            Directory.CreateDirectory(directoryReportUnit);
            string fileInputXml = Path.Combine(directoryReportUnit, @"TestResult.xml");

            string[] nunitArgs = new List<string>
            {
                "--trace=verbose"
                ,"--result=" + fileInputXml
            }.ToArray();

            new AutoRun().Execute(nunitArgs);
        }
    }
}