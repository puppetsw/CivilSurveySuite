using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;
using _3DS_CivilSurveySuiteAcadTests;
using Autodesk.AutoCAD.Runtime;

[assembly: ExtensionApplication(typeof(TestPlugin))]
namespace _3DS_CivilSurveySuiteAcadTests
{
    public class TestPlugin : IExtensionApplication
    {
        public void Initialize()
        {
            // Don't need to do anything here.
        }

        public void Terminate()
        {
            var directoryPlugin = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            if (directoryPlugin == null)
                return;

            var directoryReportUnit = Path.Combine(directoryPlugin, @"ReportUnit");
            Directory.CreateDirectory(directoryReportUnit);
            var fileInputXml = Path.Combine(directoryReportUnit, @"Report-NUnit.xml");
            var fileOutputHtml = Path.Combine(directoryReportUnit, @"Report-NUnit.html");
            var generatorReportUnit = Path.Combine(directoryPlugin, @"ReportUnit", @"ReportUnit.exe");
            //var generatorReportUnit = Path.Combine(directoryPlugin, @"Extent", @"extent.exe");

            CreateHtmlReport(fileInputXml, fileOutputHtml, generatorReportUnit);
            OpenHtmlReport(fileOutputHtml);
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

                var param = new StringBuilder();

                // param.AppendFormat($" -l verbose ");
                // param.AppendFormat($" -i \"{inputFile}\"");
                // param.AppendFormat($" -o \"{outputFile}\"");

                param.AppendFormat($" \"{inputFile}\"");
                param.AppendFormat($" \"{outputFile}\"");
                process.StartInfo.Arguments = param.ToString();

                process.Start();
                process.WaitForExit();
            }
        }
    }
}