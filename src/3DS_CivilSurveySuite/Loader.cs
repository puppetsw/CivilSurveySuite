// Copyright Scott Whitney. All Rights Reserved.
// Reproduction or transmission in whole or in part, any form or by any
// means, electronic, mechanical or otherwise, is prohibited without the
// prior written consent of the copyright owner.

using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Runtime;

[assembly: ExtensionApplication(typeof(_3DS_CivilSurveySuite.Loader))]
namespace _3DS_CivilSurveySuite
{
    public class Loader : IExtensionApplication
    {
        public void Initialize()
        {
            // Load supporting DLLs.
            string[] supportDlls = { "Microsoft.Xaml.Behaviors", "System.Data.SQLite" };

            foreach (string dll in supportDlls)
            {
                Assembly.Load(dll);
                if (!IsAssemblyLoaded(dll))
                    throw new FileLoadException($"Unable to load {dll}");
            }

            string versionYear = VersionYear();
            if (string.IsNullOrEmpty(versionYear))
                return;

            Assembly.Load($"3DS_CivilSurveySuite.ACAD{versionYear}");

            // Check if we are running Civil3D.
            if (IsCivil3D())
                Assembly.Load($"3DS_CivilSurveySuite.C3D{versionYear}");
        }

        private static string VersionYear()
        {
            // Load plugin dlls
            string registryProductRootKey = HostApplicationServices.Current.UserRegistryProductRootKey;

            // Check which version to load.
            string versionYear;
            if (registryProductRootKey.Contains("\\R19.0\\"))
                versionYear = "";
            else if (registryProductRootKey.Contains("\\R19.1\\"))
                versionYear = "";
            else if (registryProductRootKey.Contains("\\R20.0\\"))
                versionYear = "";
            else if (registryProductRootKey.Contains("\\R20.1\\"))
                versionYear = "";
            else if (registryProductRootKey.Contains("\\R21.0\\"))
                versionYear = "2017";
            else if (registryProductRootKey.Contains("\\R22.0\\"))
                versionYear = "";
            else if (registryProductRootKey.Contains("\\R23.0\\"))
                versionYear = "";
            else if (registryProductRootKey.Contains("\\R23.1\\"))
                versionYear = "";
            else if (registryProductRootKey.Contains("\\R24.0\\"))
                versionYear = "";
            else
                versionYear = "";
            return versionYear;
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
            return assemblies.Any(assem => assem.GetName().Name == assemblyName);
        }
    }
}