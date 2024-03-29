﻿namespace CivilSurveySuite.Common.Services.Interfaces
{
    public interface ICogoPointReplaceDuplicateService
    {
        string FindCode { get; set; }

        int FoundCount{ get; set; }

        string ReplaceCodeText { get; set; }

        string DuplicateCodeText { get; set; }

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

        void Find();
    }
}
