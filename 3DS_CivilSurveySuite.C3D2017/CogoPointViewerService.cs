// Copyright Scott Whitney. All Rights Reserved.
// Reproduction or transmission in whole or in part, any form or by any
// means, electronic, mechanical or otherwise, is prohibited without the
// prior written consent of the copyright owner.

using System.Collections.Generic;
using System.Globalization;
using _3DS_CivilSurveySuite.ACAD2017;
using _3DS_CivilSurveySuite.Model;
using Autodesk.AutoCAD.DatabaseServices;
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

                cogoPoint.DowngradeOpen();

                tr.Commit();
            }

            AcadApp.Editor.UpdateScreen();
        }

        public IEnumerable<CivilPoint> GetPoints()
        {
            var list = new List<CivilPoint>();
            using (var tr = AcadApp.StartTransaction())
            {
                foreach (ObjectId objectId in C3DApp.ActiveDocument.CogoPoints)
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
                            ObjectIdHandle = cogoPoint.ObjectId.Handle.ToString(),
                            PointName = cogoPoint.PointName
                        };

                    list.Add(civilPoint);
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






        private static CogoPoint GetCogoPoint(Transaction tr, CivilPoint civilPoint)
        {
            Handle h = new Handle(long.Parse(civilPoint.ObjectIdHandle, NumberStyles.AllowHexSpecifier));   
            ObjectId id = ObjectId.Null;   
            AcadApp.ActiveDatabase.TryGetObjectId(h, out id);//TryGetObjectId method

            return tr.GetObject(id, OpenMode.ForRead) as CogoPoint;
        }

    }
}