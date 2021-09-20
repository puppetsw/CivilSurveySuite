// Copyright Scott Whitney. All Rights Reserved.
// Reproduction or transmission in whole or in part, any form or by any
// means, electronic, mechanical or otherwise, is prohibited without the
// prior written consent of the copyright owner.

using System;
using System.Collections.Generic;
using _3DS_CivilSurveySuite.ACAD2017;
using _3DS_CivilSurveySuite.Model;
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


        public static StationOffset GetStatoinOffset(Alignment alignment, double x, double y)
        {
            double station = 0;
            double offset = 0;
            alignment.StationOffset(x, y, ref station, ref offset);
            return new StationOffset { Station = station, Offset = offset };
        }

        public static StationOffset GetStatoinOffset(CivilAlignment civilAlignment, double x, double y)
        {
            double station = 0;
            double offset = 0;

            using (var tr = AcadApp.StartTransaction())
            {
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
                tr.Commit();
            }
            return new StationOffset { Station = station, Offset = offset };
        }


        public static double GetStationAtXY(Alignment alignment, double x, double y)
        {
            double station = 0;
            double offset = 0;
            alignment.StationOffset(x, y, ref station, ref offset);
            return station;
        }

        public static double GetOffsetAtXY(Alignment alignment, double x, double y)
        {
            double station = 0;
            double offset = 0;
            alignment.StationOffset(x, y, ref station, ref offset);
            return offset;
        }


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

        public static CivilAlignment SelectCivilAlignment()
        {
            if (!EditorUtils.GetEntityOfType<Alignment>(out var objectId, "\n3DS> Select Alignment: "))
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