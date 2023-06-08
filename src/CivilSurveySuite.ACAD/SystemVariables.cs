using System;
using System.Runtime.CompilerServices;
using Autodesk.AutoCAD.ApplicationServices.Core;
using Autodesk.AutoCAD.Geometry;

namespace CivilSurveySuite.ACAD
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

        /// <summary>
        /// Gets current viewport size in pixels (X and Y).
        /// </summary>
        public static Point2d SCREENSIZE => GetSystemVariable<Point2d>();

        /// <summary>
        /// Gets the height of the view displayed in the current viewport, measured in drawing units.
        /// </summary>
        public static double VIEWSIZE => GetSystemVariable<double>();

        /// <summary>
        /// Gets or sets the display precision for linear units and coordinates.
        /// </summary>
        /// <value>The luprec.</value>
        public static short LUPREC
        {
            get => GetSystemVariable<short>();
            set => SetSystemVariable(value);
        }

        /// <summary>
        /// Gets or sets the name of the current annotation scale for the current space.
        /// </summary>
        /// <remarks>
        /// You can only enter a named scale that exists in the drawing's named scale list.
        /// </remarks>
        public static double CANNOSCALEVALUE
        {
            get => GetSystemVariable<double>();
            set => SetSystemVariable(value);
        }

        /// <summary>
        /// Gets the customization file name, including the path for the file name.
        /// </summary>
        public static string MENUNAME => GetSystemVariable<string>();
    }
}