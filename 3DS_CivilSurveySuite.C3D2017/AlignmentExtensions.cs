// Copyright Scott Whitney. All Rights Reserved.
// Reproduction or transmission in whole or in part, any form or by any
// means, electronic, mechanical or otherwise, is prohibited without the
// prior written consent of the copyright owner.

using _3DS_CivilSurveySuite.Model;
using Autodesk.Civil.DatabaseServices;

namespace _3DS_CivilSurveySuite.C3D2017
{
    public static class AlignmentExtensions
    {
        public static CivilAlignment ToCivilAlignment(this Alignment surface)
        {
            return new CivilAlignment
            {
                ObjectId = surface.ObjectId.Handle.ToString(),
                Name = surface.Name,
                Description = surface.Description
            };
        }
    }
}