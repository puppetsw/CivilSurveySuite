// Copyright Scott Whitney. All Rights Reserved.
// Reproduction or transmission in whole or in part, any form or by any
// means, electronic, mechanical or otherwise, is prohibited without the
// prior written consent of the copyright owner.

using System;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.Civil.DatabaseServices;

namespace _3DS_CivilSurveySuite.C3D2017
{
    public static class AlignmentUtils
    {
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


        public static Alignment GetAlignmentByObjectId(Transaction tr, ObjectId objectId)
        {
            if (tr == null)
                throw new ArgumentNullException(nameof(tr));

            if (objectId.IsNull)
                throw new ArgumentNullException(nameof(objectId));

            return tr.GetObject(objectId, OpenMode.ForRead) as Alignment;
        }
    }
}