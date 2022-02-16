// Copyright Scott Whitney. All Rights Reserved.
// Reproduction or transmission in whole or in part, any form or by any
// means, electronic, mechanical or otherwise, is prohibited without the
// prior written consent of the copyright owner.

using System;
using System.Collections.Generic;
using System.Globalization;
using _3DS_CivilSurveySuite.ACAD2017;
using _3DS_CivilSurveySuite.UI.Models;
using _3DS_CivilSurveySuite.UI.Services;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.Civil;
using Autodesk.Civil.DatabaseServices;

namespace _3DS_CivilSurveySuite.C3D2017.Services
{
    public class CogoPointEditorService : ICogoPointEditorService
    {
        private const double TOLERANCE = 0.000000001;

        public void Select(CivilPoint civilPoint)
        {
            using (var tr = AcadApp.StartTransaction())
            {
                var cogoPoint = GetCogoPoint(tr, civilPoint);
                AcadApp.Editor.SetImpliedSelection(new[] { cogoPoint.ObjectId });
                tr.Commit();
            }

            AcadApp.Editor.UpdateScreen();
        }

        public void Update(CivilPoint civilPoint)
        {
            using (var tr = AcadApp.StartTransaction())
            {
                var cogoPoint = GetCogoPoint(tr, civilPoint);

                cogoPoint.UpgradeOpen();

                UpdateCivilPoint(ref civilPoint, ref cogoPoint);

                cogoPoint.DowngradeOpen();

                tr.Commit();
            }

            AcadApp.Editor.UpdateScreen();
        }

        public void UpdateSelected(IEnumerable<CivilPoint> civilPoints, string propertyName, string value)
        {
            //Need to use locked transaction as it's being called from a dialog
            using (var tr = AcadApp.StartLockedTransaction())
            {
                foreach (CivilPoint civilPoint in civilPoints)
                {
                    var cogoPoint = GetCogoPoint(tr, civilPoint);
                    cogoPoint.UpgradeOpen();

                    switch (propertyName)
                    {
                        case nameof(CivilPoint.RawDescription):
                            cogoPoint.RawDescription = value;
                            civilPoint.RawDescription = value;
                            break;
                        case nameof(CivilPoint.DescriptionFormat):
                            cogoPoint.DescriptionFormat = value;
                            civilPoint.DescriptionFormat = value;
                            break;
                    }

                    cogoPoint.ApplyDescriptionKeys();
                    cogoPoint.DowngradeOpen();
                }
                tr.Commit();
            }
            AcadApp.Editor.Regen();
            AcadApp.Editor.UpdateScreen();
        }

        public IEnumerable<CivilPoint> GetPoints()
        {
            var list = new List<CivilPoint>();
            var cogoPoints = C3DApp.ActiveDocument.CogoPoints;

            using (var tr = AcadApp.StartTransaction())
            {
                foreach (ObjectId objectId in cogoPoints)
                {
                    try
                    {
                        var cogoPoint = tr.GetObject(objectId, OpenMode.ForRead) as CogoPoint;

                        if (cogoPoint == null)
                            throw new InvalidOperationException("cogoPoint was null.");

                        var civilPoint =
                            new CivilPoint
                            {
                                PointNumber = cogoPoint.PointNumber,
                                Easting = cogoPoint.Easting,
                                Northing = cogoPoint.Northing,
                                Elevation = cogoPoint.Elevation,
                                RawDescription = cogoPoint.RawDescription,
                                DescriptionFormat = cogoPoint.DescriptionFormat,
                                ObjectId = objectId.Handle.ToString(),
                                PointName = cogoPoint.PointName
                            };

                        list.Add(civilPoint);
                    }
                    catch (CivilException)
                    {
                        //BUG: Exception thrown: 'Autodesk.Civil.CivilException' in AeccDbMgd.dll
                        //Autodesk has made it so property throw an exception if the string is empty
                        //It's out of my hands.
                    }
                }

                tr.Commit();
            }
            return list;
        }

        public void ZoomTo(CivilPoint civilPoint)
        {
            using (var tr = AcadApp.StartTransaction())
            {
                EditorUtils.ZoomToEntity(GetCogoPoint(tr, civilPoint));
                tr.Commit();
            }
        }

        private static void UpdateCivilPoint(ref CivilPoint civilPoint, ref CogoPoint cogoPoint)
        {
            if (cogoPoint.PointNumber != civilPoint.PointNumber)
                cogoPoint.PointNumber = civilPoint.PointNumber;

            if (Math.Abs(cogoPoint.Easting - civilPoint.Easting) > TOLERANCE)
                cogoPoint.Easting = civilPoint.Easting;

            if (Math.Abs(cogoPoint.Northing - civilPoint.Northing) > TOLERANCE)
                cogoPoint.Northing = civilPoint.Northing;

            if (Math.Abs(cogoPoint.Elevation - civilPoint.Elevation) > TOLERANCE)
                cogoPoint.Elevation = civilPoint.Elevation;

            if (cogoPoint.RawDescription != civilPoint.RawDescription)
                cogoPoint.RawDescription = civilPoint.RawDescription;

            if (cogoPoint.DescriptionFormat != civilPoint.DescriptionFormat)
                cogoPoint.DescriptionFormat = civilPoint.DescriptionFormat;
        }

        private static CogoPoint GetCogoPoint(Transaction tr, CivilPoint civilPoint)
        {
            Handle h = new Handle(long.Parse(civilPoint.ObjectId, NumberStyles.AllowHexSpecifier));
            AcadApp.ActiveDatabase.TryGetObjectId(h, out var id);//TryGetObjectId method

            return tr.GetObject(id, OpenMode.ForRead) as CogoPoint;
        }
    }
}