// Copyright Scott Whitney. All Rights Reserved.
// Reproduction or transmission in whole or in part, any form or by any
// means, electronic, mechanical or otherwise, is prohibited without the
// prior written consent of the copyright owner.

using System;
using System.Collections.Generic;

namespace _3DS_CivilSurveySuite.UI.Services
{
    public interface ICogoPointReplaceDuplicateService
    {
        string FindCode { get; set; }

        int FoundCount{ get; set; }

        string ReplaceCode { get; set; }

        string DuplicateCode { get; set; }

        bool ShouldApplyDescriptionKey { get; set; }

        bool ShouldOverwriteStyle { get; set; }

        bool ShouldReplaceCode { get; set; }

        bool ShouldDuplicateCode { get; set; }

        bool ShouldDuplicateApplyDescriptionKey { get; set; }

        bool ShouldDuplicateOverwriteStyle { get; set; }

        string ReplaceSymbol { get; set; }

        string DuplicateSymbol { get; set; }

        void Save();

        void ReplaceDuplicate();

        [Obsolete("This method is obsolete. Use ReplaceDuplicate()")]
        void ReplaceDuplicate(string findCode, string replaceCode, string duplicateCode, bool shouldReplaceCode,
            bool shouldApplyDescriptionKey, bool shouldOverwriteStyle, bool shouldDuplicateCode,
            bool shouldDuplicateApplyDescriptionKey, bool shouldDuplicateOverwriteStyle,
            string replaceSymbol, string duplicateSymbol);

        void Find();

        IEnumerable<string> GetCogoPointSymbols();
    }
}