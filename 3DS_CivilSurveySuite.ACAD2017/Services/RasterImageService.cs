﻿// Copyright Scott Whitney. All Rights Reserved.
// Reproduction or transmission in whole or in part, any form or by any
// means, electronic, mechanical or otherwise, is prohibited without the
// prior written consent of the copyright owner.

using System.Collections.Generic;
using _3DS_CivilSurveySuite.UI.Services;
using Autodesk.AutoCAD.Geometry;

namespace _3DS_CivilSurveySuite.ACAD2017.Services
{
    public class RasterImageService : IRasterImageService
    {
        public void InsertRasterImage(IEnumerable<string> fileNames, double imageWidth, double imageHeight,
            double padding, int rowLimit)
        {
            if (!EditorUtils.GetPoint(out Point3d point, "\n3DS> Select starting point for raster image insertion "))
                return;

            RasterUtils.AttachRasterImages(fileNames, point, imageWidth, imageHeight, padding, rowLimit);
            AcadApp.Editor.UpdateScreen();
        }
    }
}