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
            throw new System.NotImplementedException();
        }

        public void Update(CivilPoint civilPoint)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<CivilPoint> GetPoints()
        {
            var list = new List<CivilPoint>();
            using (var tr = AcadApp.StartTransaction())
            {
                foreach (ObjectId objectId in C3DApp.ActiveDocument.CogoPoints)
                {
                    var cogoPoint = tr.GetObject(objectId, OpenMode.ForRead) as CogoPoint;
                    var civilPoint = new CivilPoint(cogoPoint.PointNumber, cogoPoint.Easting, cogoPoint.Northing, cogoPoint.Elevation, cogoPoint.RawDescription, cogoPoint.FullDescription, cogoPoint.ObjectId.Handle.ToString(), "", cogoPoint.PointName);

                    list.Add(civilPoint);
                }
                tr.Commit();
            }
            return list;
        }

        public void ZoomTo(CivilPoint civilPoint)
        {
            EditorUtils.ZoomToEntity(GetCogoPoint(civilPoint));
        }






        private static CogoPoint GetCogoPoint(CivilPoint civilPoint)
        {
            CogoPoint cogoPoint;
            using (var tr = AcadApp.StartTransaction())
            {
                Handle h = new Handle(long.Parse(civilPoint.ObjectIdHandle, NumberStyles.AllowHexSpecifier));   
                ObjectId id = ObjectId.Null;   
                AcadApp.ActiveDatabase.TryGetObjectId(h, out id);//TryGetObjectId method

                cogoPoint = tr.GetObject(id, OpenMode.ForRead) as CogoPoint;
                tr.Commit();
            }

            return cogoPoint;
        }

    }
}