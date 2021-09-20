// Copyright Scott Whitney. All Rights Reserved.
// Reproduction or transmission in whole or in part, any form or by any
// means, electronic, mechanical or otherwise, is prohibited without the
// prior written consent of the copyright owner.

using System.Collections.Generic;
using _3DS_CivilSurveySuite.Model;
using _3DS_CivilSurveySuite.UI.Services;

namespace _3DS_CivilSurveySuite.C3D2017.Services
{
    public class SelectAlignmentService : ISelectAlignmentService
    {
        public IEnumerable<CivilAlignment> GetAlignments()
        {
            return AlignmentUtils.GetAlignments().ToListOfCivilAlignments();
        }

        public CivilAlignment SelectAlignment()
        {
            return AlignmentUtils.SelectCivilAlignment();
        }
    }
}