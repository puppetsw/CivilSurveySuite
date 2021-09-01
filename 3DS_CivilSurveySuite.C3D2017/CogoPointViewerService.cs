// Copyright Scott Whitney. All Rights Reserved.
// Reproduction or transmission in whole or in part, any form or by any
// means, electronic, mechanical or otherwise, is prohibited without the
// prior written consent of the copyright owner.

using System;
using System.Collections.Generic;
using System.Globalization;
using _3DS_CivilSurveySuite.ACAD2017;
using _3DS_CivilSurveySuite.Model;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.Civil;
using Autodesk.Civil.DatabaseServices;

namespace _3DS_CivilSurveySuite.C3D2017
{
    public class CogoPointViewerService : ICogoPointViewerService
    {
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

                        var civilPoint =
                            new CivilPoint
                            {
                                PointNumber = cogoPoint.PointNumber,
                                Easting = cogoPoint.Easting,
                                Northing = cogoPoint.Northing,
                                Elevation = cogoPoint.Elevation,
                                RawDescription = cogoPoint.RawDescription,
                                DescriptionFormat = cogoPoint.DescriptionFormat,
                                ObjectIdHandle = objectId.Handle.ToString(),
                                PointName = cogoPoint.PointName
                            };

                        list.Add(civilPoint);
                    }
                    catch (CivilException e)
                    {
                        //BUG: Exception thrown: 'Autodesk.Civil.CivilException' in AeccDbMgd.dll
                        //Autodesk has made it so property throw an exception if the string is empty
                        /*
                         *    try
                              {
                                AeccDbCogoPoint* impObj = this.GetImpObj();
                                AeccDbCogoPoint* aeccDbCogoPointPtr = (IntPtr) impObj == IntPtr.Zero ? (AeccDbCogoPoint*) 0L : (AeccDbCogoPoint*) ((IntPtr) impObj + 24L);
                                AeccAtom aeccAtom2;
                                AeccAtom* attributeAeccAtom = AttributeHelper.getAttributeAeccAtom(&aeccAtom2, 167776259U, (IAeccAttributeBin*) aeccDbCogoPointPtr);
                                try
                                {
                                  __memcpy(ref aeccAtom1, (IntPtr) attributeAeccAtom, 4);
                                }
                                __fault
                                {
                                  \u003CModule\u003E.___CxxCallUnwindDtor((__FnPtr<void (void*)>) __methodptr(AeccAtom\u002E\u007Bdtor\u007D), (void*) &aeccAtom2);
                                }
                              }
                              catch (CivilException ex)
                              {
                                return string.Empty;
                              }
                         *
                         */
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

            if (cogoPoint.Easting != civilPoint.Easting)
                cogoPoint.Easting = civilPoint.Easting;

            if (cogoPoint.Northing != civilPoint.Northing)
                cogoPoint.Northing = civilPoint.Northing;

            if (cogoPoint.Elevation != civilPoint.Elevation)
                cogoPoint.Elevation = civilPoint.Elevation;

            if (cogoPoint.RawDescription != civilPoint.RawDescription)
                cogoPoint.RawDescription = civilPoint.RawDescription;

            if (cogoPoint.DescriptionFormat != civilPoint.DescriptionFormat)
                cogoPoint.DescriptionFormat = civilPoint.DescriptionFormat;
        }

        private static CogoPoint GetCogoPoint(Transaction tr, CivilPoint civilPoint)
        {
            Handle h = new Handle(long.Parse(civilPoint.ObjectIdHandle, NumberStyles.AllowHexSpecifier));
            ObjectId id = ObjectId.Null;   
            AcadApp.ActiveDatabase.TryGetObjectId(h, out id);//TryGetObjectId method

            return tr.GetObject(id, OpenMode.ForRead) as CogoPoint;
        }

    }
}