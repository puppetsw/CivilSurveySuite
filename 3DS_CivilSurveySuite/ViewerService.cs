// Copyright Scott Whitney. All Rights Reserved.
// Reproduction or transmission in whole or in part, any form or by any
// means, electronic, mechanical or otherwise, is prohibited without the
// prior written consent of the copyright owner.

using System;
using System.Collections.Generic;
using _3DS_CivilSurveySuite.Model;
using _3DS_CivilSurveySuite.UI.UserControls;
using Application = Autodesk.AutoCAD.ApplicationServices.Core.Application;

namespace _3DS_CivilSurveySuite
{
    public class ViewerService : IViewerService, IDisposable
    {
        private TraverseViewer _viewer;

        public ViewerService()
        {
            _viewer = new TraverseViewer();
            _viewer.Closing += (sender, args) => { args.Cancel = true; _viewer.Hide(); };
        }

        /// <inheritdoc />
        public void ShowWindow()
        {
            Application.ShowModelessWindow(_viewer);
        }

        /// <inheritdoc />
        public void AddGraphics(IReadOnlyList<Point> points)
        {
            _viewer.Draw(points);
        }

        /// <inheritdoc />
        public void Dispose()
        {
            _viewer = null;
        }
    }
}