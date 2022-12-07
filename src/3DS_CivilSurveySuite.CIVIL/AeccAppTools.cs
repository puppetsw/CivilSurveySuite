using System;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Runtime;

namespace _3DS_CivilSurveySuite.CIVIL
{
    /// <summary>
    /// Utility class for accessing Civil-3D.
    /// </summary>
    public static class AeccAppTools
    {
        /// <summary>
        /// module must be "Land", "Pipe", "Roadway", or "Survey"
        /// </summary>
        /// <param name="module"></param>
        /// <returns>AeccApp string</returns>
        public static dynamic GetAeccApp(string module)
        {
            var rootKey = HostApplicationServices.Current.UserRegistryProductRootKey;
            var hj = Registry.LocalMachine.OpenSubKey(rootKey);
            var c3d = (string)hj.GetValue("Release");
            c3d = c3d.Substring(0, c3d.IndexOf(".", c3d.IndexOf(".", StringComparison.Ordinal) + 1, StringComparison.Ordinal));
            hj.Close();
            c3d = "AeccXUi" + module + ".Aecc" + (module == "Land" ? "" : module) + "Application." + c3d;
            dynamic acadApp = Application.AcadApplication;
            dynamic aeccApp = acadApp.GetInterfaceObject(c3d);
            return aeccApp;
        }
    }
}
