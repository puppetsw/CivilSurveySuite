// Copyright Scott Whitney. All Rights Reserved.
// Reproduction or transmission in whole or in part, any form or by any
// means, electronic, mechanical or otherwise, is prohibited without the
// prior written consent of the copyright owner.

using System.Collections.Generic;
using _3DS_CivilSurveySuite.ACAD2017;
using _3DS_CivilSurveySuite.Model;
using _3DS_CivilSurveySuite.UI.Services;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.Civil.DatabaseServices;

namespace _3DS_CivilSurveySuite.C3D2017.Services
{
    public class SurfaceSelectService : ISurfaceSelectService
    {
        public IEnumerable<CivilSurface> GetSurfaces()
        {
            var list = new List<CivilSurface>();
            using (var tr = AcadApp.StartLockedTransaction())
            {
                var surfaceIds = C3DApp.ActiveDocument.GetSurfaceIds();

                foreach (ObjectId surfaceId in surfaceIds)
                {
                    var surface = tr.GetObject(surfaceId, OpenMode.ForRead) as TinSurface;
                    list.Add(surface.ToCivilSurface());
                }

                tr.Commit();
            }

            return list;
        }

        public CivilSurface SelectSurface()
        {
            if (!EditorUtils.GetEntityOfType<TinSurface>(out var objectId, "\n3DS> Select Surface: "))
                return null;

            CivilSurface surface;

            using (var tr = AcadApp.StartLockedTransaction())
            {
                surface = SurfaceUtils.GetSurfaceByObjectId(tr, objectId).ToCivilSurface();
                tr.Commit();
            }

            return surface;
        }
    }
}