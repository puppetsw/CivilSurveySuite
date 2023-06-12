using System.Collections.Generic;

namespace CivilSurveySuite.Common.Services.Interfaces
{
    public interface IRasterImageService
    {
        void InsertRasterImage(IEnumerable<string> fileNames, double imageWidth, double imageHeight,
            double padding, int rowLimit);
    }
}