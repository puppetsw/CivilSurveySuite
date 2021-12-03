using System.Collections.Generic;
using System.IO;
using System.Reflection;
using _3DS_CivilSurveySuiteAcadTests;
using Autodesk.AutoCAD.Runtime;
using NUnitLite;

[assembly: CommandClass(typeof(TestRunner))]
namespace _3DS_CivilSurveySuiteAcadTests
{
    public static class TestRunner
    {
        [CommandMethod("RunTests", CommandFlags.Session)]
        public static void RunTests()
        {
            var directoryPlugin = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            if (directoryPlugin == null)
                return;

            var directoryReportUnit = Path.Combine(directoryPlugin, @"ReportUnit");
            Directory.CreateDirectory(directoryReportUnit);
            var fileInputXml = Path.Combine(directoryReportUnit, @"Report-NUnit.xml");

            var nunitArgs = new List<string>
            {
                "--trace=verbose"
                ,"--result=" + fileInputXml
            }.ToArray();

            new AutoRun().Execute(nunitArgs);
        }
    }
}