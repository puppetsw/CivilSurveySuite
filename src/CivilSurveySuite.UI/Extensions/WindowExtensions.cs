using System;
using System.Runtime.InteropServices;
using System.Windows;

namespace CivilSurveySuite.UI.Extensions
{
    internal static class WindowExtensions
    {
        // from winuser.h
        private const int GWL_STYLE = -16;
        private const int WS_MAXIMIZEBOX = 0x10000;
        private const int WS_MINIMIZEBOX = 0x20000;

        [DllImport("user32.dll")]
        private static extern int GetWindowLong(IntPtr hwnd, int index);

        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hwnd, int index, int value);

        /// <summary>
        /// Hides the minimize and maximize buttons.
        /// </summary>
        /// <param name="window">The window.</param>
        /// <remarks>Usage:
        /// <code>
        /// this.SourceInitialized += (x, y) =>
        /// {
        ///    this.HideMinimizeAndMaximizeButtons();
        /// };
        /// </code>
        /// </remarks>
        internal static void HideMinimizeAndMaximizeButtons(this Window window)
        {
            IntPtr hwnd = new System.Windows.Interop.WindowInteropHelper(window).Handle;
            var currentStyle = GetWindowLong(hwnd, GWL_STYLE);

            SetWindowLong(hwnd, GWL_STYLE, (currentStyle & ~WS_MAXIMIZEBOX & ~WS_MINIMIZEBOX));
        }
    }
}
