// Copyright Scott Whitney. All Rights Reserved.
// Reproduction or transmission in whole or in part, any form or by any
// means, electronic, mechanical or otherwise, is prohibited without the
// prior written consent of the copyright owner.

using System.Collections.Generic;

namespace _3DS_CivilSurveySuite.Model
{
    /// <summary>
    /// Interface IViewerService
    /// </summary>
    /// TODO Edit XML Comment Template for IViewerService
    public interface IViewerService
    {
        /// <summary>
        /// Shows the window.
        /// </summary>
        /// TODO Edit XML Comment Template for ShowWindow
        void ShowWindow();

        /// <summary>
        /// Adds the graphics.
        /// </summary>
        /// <param name="points">The points.</param>
        /// TODO Edit XML Comment Template for AddGraphics
        void AddGraphics(IReadOnlyList<Point> points);
    }
}