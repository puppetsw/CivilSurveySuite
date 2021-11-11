// Copyright Scott Whitney. All Rights Reserved.
// Reproduction or transmission in whole or in part, any form or by any
// means, electronic, mechanical or otherwise, is prohibited without the
// prior written consent of the copyright owner.

using System;
using System.Collections.Generic;
using _3DS_CivilSurveySuite.ACAD2017;
using _3DS_CivilSurveySuite.UI.Services;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.Civil.DatabaseServices;

namespace _3DS_CivilSurveySuite.C3D2017.Services
{
    public class CogoPointReplaceDuplicateService : ICogoPointReplaceDuplicateService
    {
        public string FindCode { get; set; }

        public int FoundCount { get; set; }

        public string ReplaceCode { get; set; }

        public string DuplicateCode { get; set; }

        public bool ShouldApplyDescriptionKey { get; set; }

        public bool ShouldOverwriteStyle { get; set; }

        public bool ShouldReplaceCode { get; set; }

        public bool ShouldDuplicateCode { get; set; }

        public bool ShouldDuplicateApplyDescriptionKey { get; set; }

        public bool ShouldDuplicateOverwriteStyle { get; set; }

        public string ReplaceSymbol { get; set; }

        public string DuplicateSymbol { get; set; }

        public IEnumerable<string> GetCogoPointSymbols()
        {
            List<string> symbols;
            using (var tr = AcadApp.StartTransaction())
            {
                symbols = new List<string>(StyleUtils.GetCogoPointStylesNames(tr));
                tr.Commit();
            }
            return symbols;
        }

        public void Save()
        {
            //TODO: Save settings so the last used settings are remembered.
        }

        public void ReplaceDuplicate(string findCode, string replaceCode, string duplicateCode, bool shouldReplaceCode,
            bool shouldApplyDescriptionKey, bool shouldOverwriteStyle, bool shouldDuplicateCode,
            bool shouldDuplicateApplyDescriptionKey, bool shouldDuplicateOverwriteStyle, string replaceSymbol,
            string duplicateSymbol)
        {
            using (var tr = AcadApp.StartTransaction())
            {
                var replaceSymbolId = StyleUtils.GetPointStyleByName(tr, replaceSymbol);
                var duplicateSymbolId = StyleUtils.GetPointStyleByName(tr, duplicateSymbol);

                foreach (var id in C3DApp.ActiveDocument.CogoPoints)
                {
                    var cogoPoint = tr.GetObject(id, OpenMode.ForRead) as CogoPoint;

                    if (cogoPoint == null)
                        throw new InvalidOperationException("Unable to get CogoPoint.");

                    // If the code is not the one we are looking for, skip it.
                    if (!cogoPoint.RawDescription.StartsWith(findCode))
                        continue;

                    string oldCode = cogoPoint.RawDescription;

                    if (shouldReplaceCode)
                    {
                        // Open for write.
                        cogoPoint.UpgradeOpen();

                        // Should we replace the code?
                        if (!string.IsNullOrEmpty(replaceCode))
                        {
                            string replacedCode = cogoPoint.RawDescription.Replace(findCode, replaceCode);
                            cogoPoint.RawDescription = replacedCode;
                        }

                        if (shouldApplyDescriptionKey)
                            cogoPoint.ApplyDescriptionKeys();

                        if (shouldOverwriteStyle)
                            cogoPoint.StyleId = replaceSymbolId.ObjectId;

                        cogoPoint.DowngradeOpen();
                    }

                    // Do we need to duplicate?
                    if (shouldDuplicateCode)
                    {
                        var dupPtId = C3DApp.ActiveDocument.CogoPoints.Add(cogoPoint.Location, true);
                        var dupPt = tr.GetObject(dupPtId, OpenMode.ForWrite) as CogoPoint;

                        if (dupPt == null)
                            throw new InvalidOperationException("Could not duplicate point.");

                        if (!string.IsNullOrEmpty(duplicateCode))
                        {
                            string dupCode = oldCode.Replace(findCode, duplicateCode);
                            dupPt.RawDescription = dupCode;
                        }

                        if (shouldDuplicateApplyDescriptionKey)
                            dupPt.ApplyDescriptionKeys();

                        if (shouldDuplicateOverwriteStyle)
                            dupPt.StyleId = duplicateSymbolId.ObjectId;

                        dupPt.DowngradeOpen();
                    }
                }
                tr.Commit();
            }
        }

        public void Find()
        {
            using (var tr = AcadApp.StartTransaction())
            {
                foreach (var id in C3DApp.ActiveDocument.CogoPoints)
                {
                    var cogoPoint = tr.GetObject(id, OpenMode.ForRead) as CogoPoint;

                    if (cogoPoint == null)
                        continue;

                    if (cogoPoint.RawDescription.StartsWith(FindCode))
                    {
                        FoundCount++;
                    }
                }
                tr.Commit();
            }
        }

        public void ReplaceDuplicate()
        {
            using (var tr = AcadApp.StartTransaction())
            {
                var replaceSymbol = StyleUtils.GetPointStyleByName(tr, ReplaceSymbol);
                var duplicateSymbol = StyleUtils.GetPointStyleByName(tr, DuplicateSymbol);

                foreach (var id in C3DApp.ActiveDocument.CogoPoints)
                {
                    var cogoPoint = tr.GetObject(id, OpenMode.ForRead) as CogoPoint;

                    if (cogoPoint == null)
                        throw new InvalidOperationException("Unable to get CogoPoint.");

                    // If the code is not the one we are looking for, skip it.
                    if (!cogoPoint.RawDescription.StartsWith(FindCode))
                        continue;

                    string oldCode = cogoPoint.RawDescription;

                    if (ShouldReplaceCode)
                    {
                        // Open for write.
                        cogoPoint.UpgradeOpen();

                        // Should we replace the code?
                        if (!string.IsNullOrEmpty(ReplaceCode))
                        {
                            string replacedCode = cogoPoint.RawDescription.Replace(FindCode, ReplaceCode);
                            cogoPoint.RawDescription = replacedCode;
                        }

                        if (ShouldOverwriteStyle)
                            cogoPoint.StyleId = replaceSymbol.ObjectId;

                        if (ShouldApplyDescriptionKey)
                            cogoPoint.ApplyDescriptionKeys();

                        cogoPoint.DowngradeOpen();
                    }

                    // Do we need to duplicate?
                    if (ShouldDuplicateCode)
                    {
                        var dupPtId = C3DApp.ActiveDocument.CogoPoints.Add(cogoPoint.Location, true);
                        var dupPt = tr.GetObject(dupPtId, OpenMode.ForWrite) as CogoPoint;

                        if (dupPt == null)
                            throw new InvalidOperationException("Could not duplicate point.");

                        if (!string.IsNullOrEmpty(DuplicateCode))
                        {
                            string dupCode = oldCode.Replace(FindCode, DuplicateCode);
                            dupPt.RawDescription = dupCode;
                        }

                        if (ShouldDuplicateOverwriteStyle)
                            dupPt.StyleId = duplicateSymbol.ObjectId;

                        if (ShouldDuplicateApplyDescriptionKey)
                            dupPt.ApplyDescriptionKeys();

                        dupPt.DowngradeOpen();
                    }
                }
                tr.Commit();
            }
        }
    }
}