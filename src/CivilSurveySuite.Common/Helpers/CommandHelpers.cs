using System;
using System.Diagnostics;
using CivilSurveySuite.Common.Services.Interfaces;

namespace CivilSurveySuite.Common.Helpers
{
    public static class CommandHelpers
    {
        public static void ExecuteCommand<T>(ILogger logger = null) where T : IAcadCommand
        {
            try
            {
                var cmd = Activator.CreateInstance<T>();
                cmd.Execute();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                logger?.Error(ex, ex.Message);
            }
        }
    }
}
