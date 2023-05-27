using System.Collections.Generic;
using CivilSurveySuite.Shared.Models;

namespace CivilSurveySuite.Shared.Services.Interfaces
{
    public interface ICogoPointService
    {
        void ZoomTo(CivilPoint civilPoint);

        void Select(CivilPoint civilPoint);

        void Update(CivilPoint civilPoint);

        void UpdateSelected(IEnumerable<CivilPoint> civilPoints, string propertyName, string value);

        void MoveLabels(double x, double y);

        IEnumerable<CivilPoint> GetPoints();

        IEnumerable<string> GetCogoPointSymbols();
    }
}