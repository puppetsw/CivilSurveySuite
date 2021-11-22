// Copyright Scott Whitney. All Rights Reserved.
// Reproduction or transmission in whole or in part, any form or by any
// means, electronic, mechanical or otherwise, is prohibited without the
// prior written consent of the copyright owner.

using System;
using System.Runtime.CompilerServices;
using Autodesk.AutoCAD.ApplicationServices.Core;
using Autodesk.AutoCAD.Geometry;

namespace _3DS_CivilSurveySuite.ACAD2017
{
    /// <summary>
    /// Class for handling AutoCAD system variables.
    /// </summary>
    public static class SystemVariables
    {
        private static T GetSystemVariable<T>([CallerMemberName] string variableName = "")
        {
            if (string.IsNullOrEmpty(variableName))
            {
                throw new ArgumentNullException(nameof(variableName));
            }

            return (T)Application.GetSystemVariable(variableName);
        }

        private static void SetSystemVariable<T>(T value, [CallerMemberName]string variableName = "")
        {
            if (string.IsNullOrEmpty(variableName))
            {
                throw new ArgumentNullException(nameof(variableName));
            }

            Application.SetSystemVariable(variableName, value);
        }

        // ReSharper disable InconsistentNaming
        public static Point2d SCREENSIZE
        {
            get => GetSystemVariable<Point2d>();
            set => SetSystemVariable(value);
        }

        public static double VIEWSIZE
        {
            get => GetSystemVariable<double>();
            set => SetSystemVariable(value);
        }

        public static short LUPREC
        {
            get => GetSystemVariable<short>();
            set => SetSystemVariable(value);
        }

        public static double CANNOSCALEVALUE
        {
            get => GetSystemVariable<double>();
            set => SetSystemVariable(value);
        }
        // ReSharper restore InconsistentNaming
    }
}