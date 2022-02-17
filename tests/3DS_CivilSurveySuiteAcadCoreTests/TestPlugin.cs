using System.Diagnostics;
using System.IO;
using System.Reflection;
using _3DS_CivilSurveySuiteAcadCoreTests;
using Autodesk.AutoCAD.Runtime;

[assembly: ExtensionApplication(typeof(TestPlugin))]
namespace _3DS_CivilSurveySuiteAcadCoreTests
{
    public class TestPlugin : IExtensionApplication
    {
        public void Initialize()
        {
            // Don't need to do anything here.
        }

        public void Terminate()
        {
            string directoryPlugin = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            if (directoryPlugin == null)
                return;

            string directoryReportUnit = Path.Combine(directoryPlugin, @"ReportUnit");
            Directory.CreateDirectory(directoryReportUnit);
            string fileInputXml = Path.Combine(directoryReportUnit, @"TestResult.xml");
            // var fileOutputHtml = Path.Combine(directoryReportUnit, @"Report-NUnit.html");
            string fileOutputHtml = Path.Combine(directoryReportUnit, @"Report-NUnit");
            //var generatorReportUnit = Path.Combine(directoryPlugin, @"ReportUnit", @"ReportUnit.exe");
            string generatorReportUnit = Path.Combine(directoryPlugin, @"Extent", @"extent.exe");

            CreateHtmlReport(fileInputXml, fileOutputHtml, generatorReportUnit);
            OpenHtmlReport(fileOutputHtml + "\\index.html");
        }

        /// <summary>
        /// Opens a HTML report with the default viewer.
        /// </summary>
        /// <param name="fileName"></param>
        private static void OpenHtmlReport(string fileName)
        {
            using (var process = new Process())
            {
                process.StartInfo.UseShellExecute = true;
                process.StartInfo.RedirectStandardOutput = false;
                process.StartInfo.FileName = fileName;
                process.Start();
            }
        }

        /// <summary>
        /// Creates a HTML report based on the NUnit XML report.
        /// </summary>
        /// <param name="inputFile">The NUnit XML file.</param>
        /// <param name="outputFile">The output HTML report file.</param>
        /// <param name="reportUnitPath">Path to the ReportUnit executable.</param>
        private static void CreateHtmlReport(string inputFile, string outputFile, string reportUnitPath)
        {
            if (!File.Exists(inputFile))
                return;

            if (File.Exists(outputFile))
                File.Delete(outputFile);

            using (var process = new Process())
            {
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.CreateNoWindow = true;

                process.StartInfo.FileName = reportUnitPath;

                // var param = new StringBuilder();
                // param.AppendFormat($" \"{inputFile}\"");
                // param.AppendFormat($" \"{outputFile}\"");
                // process.StartInfo.Arguments = param.ToString();
                process.StartInfo.Arguments = $" -l verbose -i \"{inputFile}\" -o \"{outputFile}\"";

                process.Start();
                process.WaitForExit();
            }
        }
    }
}