using System.Diagnostics;
using CivilSurveySuite.Common.Services.Interfaces;

namespace CivilSurveySuite.ACAD.Services
{
    public class ProcessService : IProcessService
    {
        public void Start(string fileName)
        {
            _ = Process.Start(fileName);
        }
    }
}