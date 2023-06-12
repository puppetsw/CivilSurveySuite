using System.Collections.Generic;
using Autodesk.AutoCAD.Geometry;
using CivilSurveySuite.Common.Services.Interfaces;

namespace CivilSurveySuite.ACAD.Services
{
    public class RasterImageService : IRasterImageService
    {
        public void InsertRasterImage(IEnumerable<string> fileNames, double imageWidth, double imageHeight,
            double padding, int rowLimit)
        {
            if (!EditorUtils.TryGetPoint("\nSelect starting point for raster image insertion ", out Point3d point))
                return;

            RasterUtils.AttachRasterImages(fileNames, point, imageWidth, imageHeight, padding, rowLimit);
            AcadApp.Editor.UpdateScreen();
        }
    }
}