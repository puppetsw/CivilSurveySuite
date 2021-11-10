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

// TODO: Can we generalise this to be more generic?

namespace _3DS_CivilSurveySuite.C3D2017.Services
{
    public class CogoPointReplaceDuplicateService : ICogoPointReplaceDuplicateService
    {
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

        // public string GetSettingTreeReplaceSymbol()
        // {
        //     return Properties.Settings.Default.Tree_Replace_Base_Symbol;
        // }

        public string TreeReplaceSymbol
        {
            get => Properties.Settings.Default.Tree_Replace_Base_Symbol;
            set
            {
                Properties.Settings.Default.Tree_Replace_Base_Symbol = value;
                Properties.Settings.Default.Save();
            }
        }

        // public string GetSettingTrunkReplaceSymbol()
        // {
        //     return Properties.Settings.Default.Tree_Replace_Trunk_Symbol;
        // }

        public string TrunkReplaceSymbol
        {
            get => Properties.Settings.Default.Tree_Replace_Trunk_Symbol;
            set => Properties.Settings.Default.Tree_Replace_Trunk_Symbol = value;
        }

        public string TreeCode
        {
            get => Properties.Settings.Default.Tree_Code;
            set => Properties.Settings.Default.Tree_Code = value;
        }

        public int TrunkParameter
        {
            get => Properties.Settings.Default.Tree_Trunk_Parameter;
            set => Properties.Settings.Default.Tree_Trunk_Parameter = value;
        }

        public int SpreadParameter
        {
            get => Properties.Settings.Default.Tree_Spread_Parameter;
            set => Properties.Settings.Default.Tree_Spread_Parameter = value;
        }

        public void Save()
        {
            Properties.Settings.Default.Save();
        }

        public void ReplaceAndDuplicateSymbols()
        {
            using (var tr = AcadApp.StartTransaction())
            {
                var trunk = StyleUtils.GetPointStyleByName(tr, TrunkReplaceSymbol);
                var tree = StyleUtils.GetPointStyleByName(tr, TreeReplaceSymbol);

                foreach (var objectId in C3DApp.ActiveDocument.CogoPoints)
                {
                    var cogoPoint = tr.GetObject(objectId, OpenMode.ForRead) as CogoPoint;

                    if (cogoPoint == null)
                        continue;

                    if (cogoPoint.RawDescription.StartsWith(TreeCode))
                    {
                        // get parameters
                        string[] parameters = cogoPoint.RawDescription.Split(' ');

                        var trunkPointId = C3DApp.ActiveDocument.CogoPoints.Add(cogoPoint.Location, true);
                        var trunkPoint = trunkPointId.GetObject(OpenMode.ForWrite) as CogoPoint;

                        trunkPoint.StyleId = trunk.ObjectId;
                        trunkPoint.ScaleXY = Convert.ToDouble(parameters[TrunkParameter]);

                        trunkPoint.DowngradeOpen();

                        cogoPoint.StyleId = tree.ObjectId;
                        cogoPoint.ScaleXY = Convert.ToDouble(parameters[SpreadParameter]);
                    }
                }

                tr.Commit();
            }
        }

        // public CogoPointReplaceDuplicateService()
        // {
            // TreeCode = Properties.Settings.Default.Tree_Code;
            // TrunkParameter = Properties.Settings.Default.Tree_Trunk_Parameter;
            // SpreadParameter = Properties.Settings.Default.Tree_Spread_Parameter;
        // }
    }
}