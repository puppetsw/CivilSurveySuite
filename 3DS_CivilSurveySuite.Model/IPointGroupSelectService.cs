// Copyright Scott Whitney. All Rights Reserved.
// Reproduction or transmission in whole or in part, any form or by any
// means, electronic, mechanical or otherwise, is prohibited without the
// prior written consent of the copyright owner.

using System.Collections.Generic;

namespace _3DS_CivilSurveySuite.Model
{
    public interface IPointGroupSelectService
    {
        CivilPointGroup PointGroup { get; set; }

        CivilPointGroup GetPointGroup();

        IEnumerable<CivilPointGroup> GetPointGroups();
    }
}