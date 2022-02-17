// Copyright Scott Whitney. All Rights Reserved.
// Reproduction or transmission in whole or in part, any form or by any
// means, electronic, mechanical or otherwise, is prohibited without the
// prior written consent of the copyright owner.

using System;
using System.Collections.Generic;
using System.Linq;
using _3DS_CivilSurveySuite.ACAD2017;
using _3DS_CivilSurveySuite.UI.Models;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.Civil.DatabaseServices;

namespace _3DS_CivilSurveySuite.C3D2017
{
    public static class AlignmentUtils
    {

        /// <summary>
        /// Gets an <see cref="Alignment"/> object by name.
        /// </summary>
        /// <param name="tr">The tr.</param>
        /// <param name="alignmentName">Name of the alignment.</param>
        /// <returns>Alignment.</returns>
        /// <exception cref="ArgumentNullException">tr</exception>
        /// <exception cref="ArgumentException">alignmentName was null or empty - alignmentName</exception>
        public static Alignment GetAlignmentByName(Transaction tr, string alignmentName)
        {
            if (tr == null)
                throw new ArgumentNullException(nameof(tr));

            if (string.IsNullOrEmpty(alignmentName))
                throw new ArgumentException(@"alignmentName was null or empty", nameof(alignmentName));

            var alignmentIds = C3DApp.ActiveDocument.GetAlignmentIds();

            foreach (ObjectId alignmentId in alignmentIds)
            {
                var alignment = tr.GetObject(alignmentId, OpenMode.ForRead) as Alignment;

                if (alignment == null)
                    continue;

                if (alignment.Name == alignmentName)
                {
                    return alignment;
                }
            }
            return null;
        }

        /// <summary>
        /// Gets the alignment by it's <see cref="ObjectId"/>.
        /// </summary>
        /// <param name="tr">The tr.</param>
        /// <param name="objectId">The object identifier.</param>
        /// <returns>Alignment.</returns>
        /// <exception cref="ArgumentNullException">tr</exception>
        /// <exception cref="ArgumentNullException">objectId</exception>
        public static Alignment GetAlignmentByObjectId(Transaction tr, ObjectId objectId)
        {
            if (tr == null)
                throw new ArgumentNullException(nameof(tr));

            if (objectId.IsNull)
                throw new ArgumentNullException(nameof(objectId));

            return tr.GetObject(objectId, OpenMode.ForRead) as Alignment;
        }

        /// <summary>
        /// Converts a <see cref="Alignment"/> to a <see cref="CivilAlignment"/>.
        /// </summary>
        /// <param name="alignment">The alignment to convert.</param>
        /// <returns>A <see cref="CivilAlignment"/> representing the <see cref="Alignment"/>.</returns>
        public static CivilAlignment ToCivilAlignment(this Alignment alignment)
        {
            return new CivilAlignment
            {
                ObjectId = alignment.ObjectId.Handle.ToString(),
                Name = alignment.Name,
                Description = alignment.Description,
                StationStart = alignment.StartingStation,
                StationEnd = alignment.EndingStation,
                SiteName = alignment.SiteName
            };
        }

        /// <summary>
        /// Extension method that converts a IEnumerable of <see cref="Alignment"/> objects to a IEnumerable of
        /// <see cref="CivilAlignment"/> objects.
        /// </summary>
        /// <param name="alignments">The IEnumerable of <see cref="Alignment"/>s to convert.</param>
        /// <returns>A IEnumerable of <see cref="CivilAlignment"/>.</returns>
        public static IEnumerable<CivilAlignment> ToListOfCivilAlignments(this IEnumerable<Alignment> alignments)
        {
            return alignments.Select(alignment => alignment.ToCivilAlignment()).ToList();
        }

        /// <summary>
        /// Gets the station offset information from an alignment at the given x and y coordinates.
        /// </summary>
        /// <param name="alignment">The alignment.</param>
        /// <param name="x">The x coordinate.</param>
        /// <param name="y">The y coordinate.</param>
        /// <returns>A <see cref="StationOffset"/> object representing the alignment information.</returns>
        /// <remarks><see cref="StationOffset"/> was used instead of a tuple as 4.5 doesn't have it inbuilt.</remarks>
        public static StationOffset GetStationOffset(Alignment alignment, double x, double y)
        {
            double station = 0;
            double offset = 0;
            alignment.StationOffset(x, y, ref station, ref offset);
            return new StationOffset { Station = station, Offset = offset };
        }

        /// <summary>
        /// Gets the station offset information from an alignment at the given x and y coordinates.
        /// </summary>
        /// <param name="tr">Active <see cref="Transaction"/>.</param>
        /// <param name="civilAlignment">The alignment.</param>
        /// <param name="x">The x coordinate.</param>
        /// <param name="y">The y coordinate.</param>
        /// <returns>A <see cref="StationOffset"/> object representing the alignment information.</returns>
        /// <remarks><see cref="StationOffset"/> was used instead of a tuple as 4.5 doesn't have it inbuilt.</remarks>
        public static StationOffset GetStationOffset(Transaction tr, CivilAlignment civilAlignment, double x, double y)
        {
            double station = 0;
            double offset = 0;

            try
            {
                Alignment alignment = GetAlignmentByName(tr, civilAlignment.Name);
                alignment.StationOffset(x, y, ref station, ref offset);
            }
            catch
            {
                station = -9999.999;
                offset = -9999.999;
            }

            return new StationOffset { Station = station, Offset = offset };
        }

        /// <summary>
        /// Gets a IEnumerable of <see cref="Alignment"/> from the current document.
        /// </summary>
        /// <returns>A IEnumerable of <see cref="Alignment"/>.</returns>
        public static IEnumerable<Alignment> GetAlignments()
        {
            var list = new List<Alignment>();
            using (var tr = AcadApp.StartTransaction())
            {
                foreach (ObjectId alignmentId in C3DApp.ActiveDocument.GetAlignmentIds())
                {
                    var alignment = tr.GetObject(alignmentId, OpenMode.ForRead) as Alignment;
                    list.Add(alignment);
                }
                tr.Commit();
            }
            return list;
        }

        /// <summary>
        /// Gets a IEnumerable of <see cref="CivilAlignment"/>s from the current drawing.
        /// </summary>
        /// <returns>A IEnumerable of <see cref="CivilAlignment"/>s representing the <see cref="Alignment"/>s.</returns>
        public static IEnumerable<CivilAlignment> GetCivilAlignments()
        {
            var list = new List<CivilAlignment>();
            using (var tr = AcadApp.StartTransaction())
            {
                foreach (ObjectId alignmentId in C3DApp.ActiveDocument.GetAlignmentIds())
                {
                    var alignment = tr.GetObject(alignmentId, OpenMode.ForRead) as Alignment;
                    list.Add(alignment.ToCivilAlignment());
                }
                tr.Commit();
            }
            return list;
        }

        /// <summary>
        /// Gets alignments linked to a site.
        /// </summary>
        /// <param name="site">The name of the site to search against.</param>
        /// <returns><see cref="IEnumerable{CivilAlignment}">IEnumerable&lt;CivilAlignment&gt;</see></returns>
        public static IEnumerable<CivilAlignment> GetCivilAlignmentsInCivilSite(CivilSite site)
        {
            var list = new List<CivilAlignment>();
            using (var tr = AcadApp.StartTransaction())
            {
                IEnumerable<CivilAlignment> alignments = GetCivilAlignments();

                list.AddRange(site.Equals(CivilSite.NoneSite)
                    ? alignments
                    : alignments.Where(civilAlignment => civilAlignment.SiteName == site.Name));

                tr.Commit();
            }
            return list;
        }

        /// <summary>
        /// Prompts the user to select an alignment in the drawing space.
        /// </summary>
        /// <returns>A <see cref="CivilAlignment"/> representing the <see cref="Alignment"/>.</returns>
        public static CivilAlignment SelectCivilAlignment()
        {
            if (!EditorUtils.TryGetEntityOfType<Alignment>("\n3DS> Select Alignment: ",
                    "\n3DS> Select Alignmnet: ", out var objectId))
                return null;

            CivilAlignment alignment;

            using (var tr = AcadApp.StartLockedTransaction())
            {
                alignment = GetAlignmentByObjectId(tr, objectId).ToCivilAlignment();
                tr.Commit();
            }

            return alignment;
        }
    }
}