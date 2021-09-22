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
    public static class ProfileUtils
    {
        /// <summary>
        /// Gets the profiles in alignment.
        /// </summary>
        /// <param name="alignment">The alignment.</param>
        /// <returns>IEnumerable&lt;ObjectId&gt;.</returns>
        [Obsolete("This method is obsolete. Just use Alignment.GetProfileIds")]
        public static IEnumerable<ObjectId> GetProfilesInAlignment(Alignment alignment)
        {
            return alignment.GetProfileIds().ToArray();
        }


        /// <summary>
        /// Gets a collection of <see cref="CivilProfile"/> from a <see cref="CivilAlignment"/>.
        /// </summary>
        /// <param name="civilAlignment"></param>
        /// <returns>IEnumerable&lt;CivilProfile&gt;.</returns>
        public static IEnumerable<CivilProfile> GetCivilProfiles(CivilAlignment civilAlignment)
        {
            if (civilAlignment == null)
                throw new ArgumentNullException(nameof(civilAlignment));

            var profiles = new List<CivilProfile>();

            using (var tr = AcadApp.StartTransaction())
            {
                var alignment = AlignmentUtils.GetAlignmentByObjectId(tr, civilAlignment.ObjectId.ToObjectId());
                //HACK: May be faster to get by name? I'll have to test.
                //UNDONE: 22/9 getting by objectId is approximately 1 second faster.
                //var alignment = AlignmentUtils.GetAlignmentByName(tr, civilAlignment.Name);

                foreach (ObjectId objectId in alignment.GetProfileIds())
                {
                    var prof = tr.GetObject(objectId, OpenMode.ForRead) as Profile;

                    if (prof == null)
                        continue;

                    var civilProf = new CivilProfile
                    {
                        Name = prof.Name,
                        Description = prof.Description,
                        ObjectId = objectId.Handle.ToString()
                    };

                    profiles.Add(civilProf);
                }

                tr.Commit();
            }

            return profiles;
        }
    }
}
