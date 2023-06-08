using System;
using System.Collections.Generic;
using System.Linq;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.Civil.DatabaseServices;
using CivilSurveySuite.ACAD;
using CivilSurveySuite.Common.Helpers;
using CivilSurveySuite.Common.Models;

namespace CivilSurveySuite.CIVIL
{
    public static class PointGroupUtils
    {
        /// <summary>
        /// Quickly add/remove points by description match, elevation range, number range, etc.
        /// </summary>
        public static void AddPointToPointGroupBy()
        {
            throw new NotSupportedException();
        }

        public static void RemovePointFromPointGroupBy()
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Prompts the user to select a <see cref="CogoPoint"/> and gets it's primary <see cref="PointGroup"/>.
        /// </summary>
        /// <returns>A <see cref="CivilPointGroup"/> object representing the <see cref="PointGroup"/>.</returns>
        /// <exception cref="InvalidOperationException">the <see cref="ObjectId"/> did not
        /// return a valid <see cref="CogoPoint"/></exception>
        public static CivilPointGroup SelectCivilPointGroup()
        {
            if (!EditorUtils.TryGetEntityOfType<CogoPoint>("\nSelect CogoPoint to obtain Point Group: ",
                    "\nSelect CogoPoint: ", out var objectId))
                return null;

            CivilPointGroup pointGroup;

            using (var tr = AcadApp.StartTransaction())
            {
                var cogoPoint = tr.GetObject(objectId, OpenMode.ForRead) as CogoPoint;

                if (cogoPoint == null)
                    throw new InvalidOperationException("cogoPoint was null.");

                var pgId = cogoPoint.PrimaryPointGroupId;

                var pg = GetPointGroupByObjectId(tr, pgId);
                pointGroup = pg.ToCivilPointGroup();

                tr.Commit();
            }

            return pointGroup;
        }

        /// <summary>
        /// Gets a <see cref="PointGroup"/> by it's <see cref="ObjectId"/>."/>
        /// </summary>
        /// <param name="tr">The active transaction.</param>
        /// <param name="pointGroupObjectId">The <see cref="ObjectId"/> of the <see cref="PointGroup"/>.</param>
        /// <returns>A <see cref="PointGroup"/> object or null if not found.</returns>
        /// <exception cref="ArgumentNullException">if the transaction is null or the ObjectId is null.</exception>
        public static PointGroup GetPointGroupByObjectId(Transaction tr, ObjectId pointGroupObjectId)
        {
            if (tr == null)
                throw new ArgumentNullException(nameof(tr));

            if (pointGroupObjectId.IsNull)
                throw new ArgumentNullException(nameof(pointGroupObjectId));

            return tr.GetObject(pointGroupObjectId, OpenMode.ForRead) as PointGroup;
        }

        /// <summary>
        /// Gets a <see cref="PointGroup"/> by it's name.
        /// </summary>
        /// <param name="tr">The active transaction.</param>
        /// <param name="groupName">The name of the point group.</param>
        /// <returns>A <see cref="PointGroup"/> object if found, otherwise null.</returns>
        /// <exception cref="ArgumentNullException">the transaction was null.</exception>
        /// <exception cref="ArgumentException">the groupName was empty or null.</exception>
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

        /// <summary>
        /// Gets a string representing the <see cref="PointGroup"/>s point number range.
        /// </summary>
        /// <param name="tr">The active transaction.</param>
        /// <param name="groupName">The name of the <see cref="PointGroup"/>.</param>
        /// <returns>A string representing the point number range.</returns>
        /// <exception cref="InvalidOperationException">the cogoPoint was null.</exception>
        public static string GroupRange(Transaction tr, string groupName)
        {
            var pointNumberList = new List<string>();

            var pointGroup = GetPointGroupByName(tr, groupName);

            if (pointGroup == null)
                return "No points in drawing.";

            if (pointGroup.PointsCount == 0)
                return $"No points in {groupName}";

            var pointNumbers = pointGroup.GetPointNumbers();

            int index = 0;
            while (index < pointNumbers.Length)
            {
                uint num = pointNumbers[index];
                ObjectId pointByPointNumber = C3DApp.ActiveDocument.CogoPoints.GetPointByPointNumber(num);
                CogoPoint cogoPoint = pointByPointNumber.GetObject(OpenMode.ForRead) as CogoPoint;

                if (cogoPoint == null)
                    throw new InvalidOperationException("cogoPoint was null.");

                if (cogoPoint.PointNumber > 0U)
                    pointNumberList.Add(cogoPoint.PointNumber.ToString());

                index++;
            }

            return StringHelpers.GetRangeString(pointNumberList);
        }

        /// <summary>
        /// Gets a list of <see cref="PointGroup"/>s.
        /// </summary>
        /// <returns>A IEnumerable of <see cref="PointGroup"/> of the PointGroups in the active drawing.</returns>
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

        /// <summary>
        /// Gets a list of <see cref="CivilPointGroup"/>s.
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Converts a <see cref="PointGroup"/> to a <see cref="CivilPointGroup"/>.
        /// </summary>
        /// <param name="pointGroup">The PointGroup to convert.</param>
        /// <returns>A <see cref="CivilPointGroup"/> representing the PointGroup.</returns>
        public static CivilPointGroup ToCivilPointGroup(this PointGroup pointGroup)
        {
            return new CivilPointGroup
            {
                ObjectId = pointGroup.ObjectId.Handle.ToString(),
                Name = pointGroup.Name,
                Description = pointGroup.Description
            };
        }

        /// <summary>
        /// Converts a list of PointGroups to a list of CivilPointGroups.
        /// </summary>
        /// <param name="pointGroups">The list of PointGroups.</param>
        /// <returns>A list of CivilPointGroups.</returns>
        public static IEnumerable<CivilPointGroup> ToListOfCivilPointGroups(this IEnumerable<PointGroup> pointGroups)
        {
            return pointGroups.Select(pointGroup => pointGroup.ToCivilPointGroup()).ToList();
        }

        public static PointGroup ToPointGroup(this CivilPointGroup civilPointGroup, Transaction tr)
        {
            return PointGroupUtils.GetPointGroupByName(tr, civilPointGroup.Name);
        }
    }
}