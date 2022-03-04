// Copyright Scott Whitney. All Rights Reserved.
// Reproduction or transmission in whole or in part, any form or by any
// means, electronic, mechanical or otherwise, is prohibited without the
// prior written consent of the copyright owner.

using System;
using System.Collections.Generic;
using _3DS_CivilSurveySuite.UI.Models;

namespace _3DS_CivilSurveySuite.UI.Services
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