using System.Collections.Generic;

namespace CivilSurveySuite.Shared.Services.Interfaces
{
    public interface IRasterImageService
    {
        void InsertRasterImage(IEnumerable<string> fileNames, double imageWidth, double imageHeight,
            double padding, int rowLimit);
    }
}