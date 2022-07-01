using System.Diagnostics;
using System.IO;
using System.Reflection;
using _3DS_CivilSurveySuite.Shared.Services.Interfaces;

namespace _3DS_CivilSurveySuite.ACAD
{
    public class ShowHelpCommand : IAcadCommand
    {
        public void Execute()
        {
            var helpFile = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + Path.DirectorySeparatorChar + "3DSCivilSurveySuite.chm";
            AcadApp.Logger.Info($"Trying to open help file: {helpFile}");
            Process.Start(helpFile);
        }
    }
}