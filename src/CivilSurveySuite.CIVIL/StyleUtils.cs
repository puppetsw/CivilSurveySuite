// Copyright Scott Whitney. All Rights Reserved.
// Reproduction or transmission in whole or in part, any form or by any
// means, electronic, mechanical or otherwise, is prohibited without the
// prior written consent of the copyright owner.

using System.Collections.Generic;
using System.Linq;
using CivilSurveySuite.ACAD;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.Civil.DatabaseServices;
using Autodesk.Civil.DatabaseServices.Styles;

namespace CivilSurveySuite.CIVIL
{
    public static class StyleUtils
    {
        /// <summary>
        /// Swap the style of the CogoPoint.
        /// </summary>
        /// <param name="cogoPoint"></param>
        /// <param name="pointStyle"></param>
        /// <param name="applyDescriptionKey"></param>
        public static void ChangeStyle(CogoPoint cogoPoint, PointStyle pointStyle, bool applyDescriptionKey = false)
        {
            cogoPoint.StyleId = pointStyle.Id;

            if (applyDescriptionKey)
                cogoPoint.ApplyDescriptionKeys();

            // AcadApp.Editor.UpdateScreen(); //UNDONE: Check if this is needed.
        }

        /// <summary>
        /// Gets the point style by it's object id.
        /// </summary>
        /// <param name="tr">The active transaction.</param>
        /// <param name="objectId">The <see cref="ObjectId"/> of the style.</param>
        /// <returns>The PointStyle object.</returns>
        public static PointStyle GetPointStyle(Transaction tr, string objectId)
        {
            var pointStyleId = objectId.ToObjectId();

            if (pointStyleId == ObjectId.Null)
                return null;

            return tr.GetObject(pointStyleId, OpenMode.ForRead) as PointStyle;
        }

        public static PointStyle GetPointStyleByName(Transaction tr, string name)
        {
            return C3DApp.ActiveDocument.Styles.PointStyles
                .Select(id => tr.GetObject(id, OpenMode.ForRead))
                .OfType<PointStyle>()
                .FirstOrDefault(pointStyle => pointStyle.Name == name);
        }

        /// <summary>
        /// Gets a list of point style names from the current drawing.
        /// </summary>
        /// <param name="tr">The active transaction.</param>
        /// <returns>A enumerable list containing the style names.</returns>
        public static IEnumerable<string> GetCogoPointStylesNames(Transaction tr)
        {
            return C3DApp.ActiveDocument.Styles.PointStyles
                .Select(id => tr.GetObject(id, OpenMode.ForRead)).OfType<PointStyle>()
                .Select(style => style.Name).OrderBy(name => name).ToList();
        }

        /// <summary>
        /// Gets a list of point label styles names from the current drawing.
        /// </summary>
        /// <param name="tr">The active transaction.</param>
        /// <returns>A enumerable list containing the label style names.</returns>
        public static IEnumerable<string> GetCogoPointLabelStyleNames(Transaction tr)
        {
            return C3DApp.ActiveDocument.Styles.LabelStyles.PointLabelStyles.LabelStyles
                .Select(id => tr.GetObject(id, OpenMode.ForRead)).OfType<LabelStyle>()
                .Select(style => style.Name).OrderBy(name => name).ToList();
        }
    }
}