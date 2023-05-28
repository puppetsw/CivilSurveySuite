using System;
using System.Collections.Generic;
using CivilSurveySuite.Common.Models;

namespace CivilSurveySuite.Common.Services.Interfaces
{
    /// <summary>
    /// Interface ITraverseService
    /// </summary>
    public interface ITraverseService
    {
        void DrawLines(IEnumerable<TraverseObject> traverse);

        void DrawTransientLines(IEnumerable<TraverseObject> traverse);

        void SetBasePoint();

        void ClearGraphics();

        void ZoomTo(IEnumerable<TraverseObject> traverse);

        [Obsolete("This method is obsolete. Use SelectLines() instead.")]
        AngleDistance SelectLine();

        IEnumerable<TraverseObject> SelectLines();

        IEnumerable<TraverseObject> GetTraverseData(string fileName);

        void WriteTraverseData(string fileName, IEnumerable<TraverseObject> traverseData);

    }
}