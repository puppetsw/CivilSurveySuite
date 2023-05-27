using System.Diagnostics;
using CivilSurveySuite.Shared.Services.Interfaces;

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