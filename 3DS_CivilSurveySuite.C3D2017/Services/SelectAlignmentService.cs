// Copyright Scott Whitney. All Rights Reserved.
// Reproduction or transmission in whole or in part, any form or by any
// means, electronic, mechanical or otherwise, is prohibited without the
// prior written consent of the copyright owner.

using System.Collections.Generic;
using _3DS_CivilSurveySuite.ACAD2017;
using _3DS_CivilSurveySuite.Model;
using _3DS_CivilSurveySuite.UI.Services;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.Civil.DatabaseServices;

namespace _3DS_CivilSurveySuite.C3D2017.Services
{
    public class SelectAlignmentService : ISelectAlignmentService
    {
        public IEnumerable<CivilAlignment> GetAlignments()
        {
            var list = new List<CivilAlignment>();
            using (var tr = AcadApp.StartLockedTransaction())
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

        public CivilAlignment SelectAlignment()
        {
            if (!EditorUtils.GetEntityOfType<Alignment>(out var objectId, "\n3DS> Select Alignment: "))
                return null;

            CivilAlignment alignment;

            using (var tr = AcadApp.StartLockedTransaction())
            {
                alignment = AlignmentUtils.GetAlignmentByObjectId(tr, objectId).ToCivilAlignment();
                tr.Commit();
            }

            return alignment;
        }
    }
}