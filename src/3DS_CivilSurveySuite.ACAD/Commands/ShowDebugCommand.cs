using _3DS_CivilSurveySuite.Shared.Services.Interfaces;

namespace _3DS_CivilSurveySuite.ACAD.Commands
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