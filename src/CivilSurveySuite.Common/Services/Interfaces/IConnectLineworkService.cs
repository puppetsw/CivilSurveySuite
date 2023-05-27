// Copyright Scott Whitney. All Rights Reserved.
// Reproduction or transmission in whole or in part, any form or by any
// means, electronic, mechanical or otherwise, is prohibited without the
// prior written consent of the copyright owner.

using System.Collections.Generic;
using System.Threading.Tasks;
using CivilSurveySuite.Shared.Models;

namespace CivilSurveySuite.Shared.Services.Interfaces
{
    public interface IConnectLineworkService
    {
        string DescriptionKeyFile { get; set; }

        double MidOrdinate { get; set; }

        Task ConnectCogoPoints(IReadOnlyList<DescriptionKey> descriptionKeys);
    }
}
