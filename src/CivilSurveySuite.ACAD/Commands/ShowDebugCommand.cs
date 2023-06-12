using CivilSurveySuite.Common.Services.Interfaces;

namespace CivilSurveySuite.ACAD
{
    public class ShowDebugCommand : IAcadCommand
    {
        public void Execute()
        {
            ILogger logger = Ioc.Default.GetInstance<ILogger>();
            logger.ShowLog();
        }
    }
}