﻿using System.Collections.Generic;
using System.IO;
using System.Reflection;
using _3DS_CivilSurveySuiteAcadCoreTests;
using Autodesk.AutoCAD.Runtime;
using NUnitLite;

[assembly: CommandClass(typeof(TestRunner))]
namespace _3DS_CivilSurveySuiteAcadCoreTests
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