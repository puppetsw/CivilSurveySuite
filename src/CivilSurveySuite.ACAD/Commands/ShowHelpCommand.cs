using System.Diagnostics;
using System.IO;
using System.Reflection;
using CivilSurveySuite.Common;
using CivilSurveySuite.Common.Services.Interfaces;

namespace CivilSurveySuite.ACAD
{
    public class ShowHelpCommand : IAcadCommand
    {
        public void Execute()
        {
            var helpFile = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + Path.DirectorySeparatorChar + Constants.HELP_FILE_NAME;
            AcadApp.Logger.Info($"Trying to open help file: {helpFile}");
            Process.Start(helpFile);
        }
    }
}