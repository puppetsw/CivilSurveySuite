using System;
using System.Collections.Generic;
using _3DS_CivilSurveySuite.Core;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.Civil.ApplicationServices;
using Autodesk.Civil.DatabaseServices;

namespace _3DS_CivilSurveySuite.C3D2017
{
    public static class PointGroupUtils
    {
        /// <summary>
        /// Quickly add/remove points by description match, elevation range, number range, etc.
        /// </summary>
        static void AddPointToPointGroupBy()
        { }

        static void RemovePointFromPointGroupBy()
        { }

        public static PointGroup GetPointGroupByName(Transaction tr, string groupName)
        {
            foreach (ObjectId objectId in C3DApp.ActiveDocument.PointGroups)
            {
                var pointGroup = tr.GetObject(objectId, OpenMode.ForRead) as PointGroup;
                if (pointGroup != null && string.Equals(pointGroup.Name, groupName, StringComparison.CurrentCultureIgnoreCase))
                {
                    return pointGroup;
                }
            }

            return null;
        }

        public static string GroupRange(Transaction tr, string groupName)
        {
            var pointNumberList = new List<string>();

            var pointGroup = GetPointGroupByName(tr, groupName);

            if (pointGroup.PointsCount == 0)
                return $"3DS> No points in {groupName}";

            var pointNumbers = GetPointGroupByName(tr, groupName).GetPointNumbers();

            int index = 0;
            while (index < pointNumbers.Length)
            {
                uint num = pointNumbers[index];
                ObjectId pointByPointNumber = C3DApp.ActiveDocument.CogoPoints.GetPointByPointNumber(num);
                CogoPoint cogoPoint = pointByPointNumber.GetObject(OpenMode.ForRead) as CogoPoint;

                if (cogoPoint.PointNumber > 0U)
                    pointNumberList.Add(cogoPoint.PointNumber.ToString());

                index++;
            }

            return StringHelpers.GetRangeString(pointNumberList);
        }
    }
}
