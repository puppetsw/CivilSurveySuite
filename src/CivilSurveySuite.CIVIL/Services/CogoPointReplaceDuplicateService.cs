// Copyright Scott Whitney. All Rights Reserved.
// Reproduction or transmission in whole or in part, any form or by any
// means, electronic, mechanical or otherwise, is prohibited without the
// prior written consent of the copyright owner.

using System;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.Civil.DatabaseServices;
using CivilSurveySuite.ACAD;
using CivilSurveySuite.Common.Services.Interfaces;
using DBObject = Autodesk.AutoCAD.DatabaseServices.DBObject;

namespace CivilSurveySuite.CIVIL.Services
{
    public class CogoPointReplaceDuplicateService : ICogoPointReplaceDuplicateService
    {
        public string FindCode { get; set; }

        public int FoundCount { get; set; }

        public string ReplaceCodeText { get; set; }

        public string DuplicateCodeText { get; set; }

        public bool ShouldApplyDescriptionKey { get; set; }

        public bool ShouldOverwriteStyle { get; set; }

        public bool ShouldReplaceCode { get; set; }

        public bool ShouldDuplicateCode { get; set; }

        public bool ShouldDuplicateApplyDescriptionKey { get; set; }

        public bool ShouldDuplicateOverwriteStyle { get; set; }

        public string ReplaceSymbol { get; set; }

        public string DuplicateSymbol { get; set; }

        public void Save()
        {
            //TODO: Save settings so the last used settings are remembered.
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
                        ReplaceCode(ref cogoPoint, replaceSymbol);
                    }

                    // Do we need to duplicate?
                    if (ShouldDuplicateCode)
                    {
                        DuplicateCode(tr, ref cogoPoint, duplicateSymbol, oldCode);
                    }
                }
                tr.Commit();
            }
        }

        private void ReplaceCode(ref CogoPoint cogoPoint, DBObject replaceSymbol)
        {
            // Open for write.
            cogoPoint.UpgradeOpen();

            // Should we replace the code?
            if (!string.IsNullOrEmpty(ReplaceCodeText))
            {
                string replacedCode = cogoPoint.RawDescription.Replace(FindCode, ReplaceCodeText);
                cogoPoint.RawDescription = replacedCode;
            }

            if (ShouldOverwriteStyle)
                cogoPoint.StyleId = replaceSymbol.ObjectId;

            if (ShouldApplyDescriptionKey)
                cogoPoint.ApplyDescriptionKeys();

            cogoPoint.DowngradeOpen();
        }

        private void DuplicateCode(Transaction tr, ref CogoPoint cogoPoint, DBObject duplicateSymbol, string oldCode)
        {
            var dupPtId = C3DApp.ActiveDocument.CogoPoints.Add(cogoPoint.Location, true);
            var dupPt = tr.GetObject(dupPtId, OpenMode.ForWrite) as CogoPoint;

            if (dupPt == null)
                throw new InvalidOperationException("Could not duplicate point.");

            if (!string.IsNullOrEmpty(DuplicateCodeText))
            {
                string dupCode = oldCode.Replace(FindCode, DuplicateCodeText);
                dupPt.RawDescription = dupCode;
            }

            if (ShouldDuplicateOverwriteStyle)
                dupPt.StyleId = duplicateSymbol.ObjectId;

            if (ShouldDuplicateApplyDescriptionKey)
                dupPt.ApplyDescriptionKeys();

            dupPt.DowngradeOpen();
        }
    }
}