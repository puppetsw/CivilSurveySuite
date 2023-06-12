using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Autodesk.AutoCAD.Runtime;

[assembly: ExtensionApplication(typeof(CivilSurveySuite.Loader))]
namespace CivilSurveySuite
{
    public class Loader : IExtensionApplication
    {
        private const string ACAD_DLL = "CivilSurveySuite.ACAD";
        private const string CIVIL_DLL = "CivilSurveySuite.CIVIL";
        private readonly string[] _supportDlls =
        {
            "Microsoft.Xaml.Behaviors"
        };

        public void Initialize()
        {
            LoadSupportAssemblies();
            Assembly.Load(ACAD_DLL);
            if (IsCivil3D())
            {
                Assembly.Load(CIVIL_DLL);
            }
        }

        public void Terminate() { }

        private void LoadSupportAssemblies()
        {
            foreach (string dll in _supportDlls)
            {
                Assembly.Load(dll);
                if (!IsAssemblyLoaded(dll))
                {
                    throw new FileLoadException($"Unable to load {dll}");
                }
            }
        }

        private static bool IsCivil3D()
        {
            return SystemObjects.DynamicLinker.GetLoadedModules().Contains("AecBase.dbx".ToLower());
        }

        private static bool IsAssemblyLoaded(string assemblyName)
        {
            var currentDomain = AppDomain.CurrentDomain;
            var assemblies = currentDomain.GetAssemblies();
            return assemblies.Any(assembly => assembly.GetName().Name == assemblyName);
        }
    }
}