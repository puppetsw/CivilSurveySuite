using System;
using System.Diagnostics;
using CivilSurveySuite.Shared.Services.Interfaces;

namespace CivilSurveySuite.Shared.Helpers
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
