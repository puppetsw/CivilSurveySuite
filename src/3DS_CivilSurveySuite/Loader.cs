// Copyright Scott Whitney. All Rights Reserved.
// Reproduction or transmission in whole or in part, any form or by any
// means, electronic, mechanical or otherwise, is prohibited without the
// prior written consent of the copyright owner.

using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Autodesk.AutoCAD.Runtime;

[assembly: ExtensionApplication(typeof(_3DS_CivilSurveySuite.Loader))]
namespace _3DS_CivilSurveySuite
{
    public class Loader : IExtensionApplication
    {
        public void Initialize()
        {
            // Load supporting DLLs.
            string[] supportDlls = { "Microsoft.Xaml.Behaviors" };

            foreach (string dll in supportDlls)
            {
                Assembly.Load(dll);
                if (!IsAssemblyLoaded(dll))
                {
                    throw new FileLoadException($"Unable to load {dll}");
                }
            }

            Assembly.Load($"3DS_CivilSurveySuite.ACAD");

            // Check if we are running Civil3D.
            if (IsCivil3D())
            {
                Assembly.Load($"3DS_CivilSurveySuite.CIVIL");
            }
        }

        public void Terminate()
        {
            // Do nothing.
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