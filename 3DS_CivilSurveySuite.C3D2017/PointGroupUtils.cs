using System;
using System.Collections.Generic;
using _3DS_CivilSurveySuite.ACAD2017;
using _3DS_CivilSurveySuite.Core;
using _3DS_CivilSurveySuite.Model;
using Autodesk.AutoCAD.DatabaseServices;
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


        public static CivilPointGroup SelectCivilPointGroup()
        {
            if (!EditorUtils.GetEntityOfType<CogoPoint>(out var objectId, "\n3DS> Select CogoPoint to obtain Point Group: "))
                return null;

            CivilPointGroup pointGroup;

            using (var tr = AcadApp.StartTransaction())
            {
                var cogoPoint = tr.GetObject(objectId, OpenMode.ForRead) as CogoPoint;
                var pgId = cogoPoint.PrimaryPointGroupId;

                var pg = GetPointGroupByObjectId(tr, pgId);
                pointGroup = pg.ToCivilPointGroup();

                tr.Commit();
            }

            return pointGroup;
        }


        public static PointGroup GetPointGroupByObjectId(Transaction tr, ObjectId pointGroupObjectId)
        {
            if (tr == null)
                throw new ArgumentNullException(nameof(tr));

            if (pointGroupObjectId.IsNull)
                throw new ArgumentNullException(nameof(pointGroupObjectId));

            return tr.GetObject(pointGroupObjectId, OpenMode.ForRead) as PointGroup;
        }

        public static PointGroup GetPointGroupByName(Transaction tr, string groupName)
        {
            if (tr == null)
                throw new ArgumentNullException(nameof(tr));

            if (string.IsNullOrEmpty(groupName))
                throw new ArgumentException(nameof(groupName));

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



        public static IEnumerable<PointGroup> GetPointGroups()
        {
            var list = new List<PointGroup>();
            using (var tr = AcadApp.StartLockedTransaction())
            {
                foreach (ObjectId pointGroupId in C3DApp.ActiveDocument.PointGroups)
                {
                    var pointGroup = tr.GetObject(pointGroupId, OpenMode.ForRead) as PointGroup;
                    list.Add(pointGroup);
                }

                tr.Commit();
            }
            return list;
        }

        public static IEnumerable<CivilPointGroup> GetCivilPointGroups()
        {
            var list = new List<CivilPointGroup>();
            using (var tr = AcadApp.StartTransaction())
            {
                foreach (ObjectId pointGroupId in C3DApp.ActiveDocument.PointGroups)
                {
                    var pointGroup = tr.GetObject(pointGroupId, OpenMode.ForRead) as PointGroup;
                    list.Add(pointGroup.ToCivilPointGroup());
                }
                tr.Commit();
            }
            return list;
        }


    }
}
