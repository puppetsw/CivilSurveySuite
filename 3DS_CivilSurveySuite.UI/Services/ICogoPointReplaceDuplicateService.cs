// Copyright Scott Whitney. All Rights Reserved.
// Reproduction or transmission in whole or in part, any form or by any
// means, electronic, mechanical or otherwise, is prohibited without the
// prior written consent of the copyright owner.

using System.Collections.Generic;

namespace _3DS_CivilSurveySuite.UI.Services
{
    public interface ICogoPointReplaceDuplicateService
    {
        IEnumerable<string> GetCogoPointSymbols();

        string TreeReplaceSymbol { get; set; }

        string TrunkReplaceSymbol { get; set; }

        string TreeCode { get; set; }

        int TrunkParameter { get; set; }

        int SpreadParameter { get; set; }

        void Save();

        void ReplaceAndDuplicateSymbols();
    }
}