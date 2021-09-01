// Copyright Scott Whitney. All Rights Reserved.
// Reproduction or transmission in whole or in part, any form or by any
// means, electronic, mechanical or otherwise, is prohibited without the
// prior written consent of the copyright owner.

using _3DS_CivilSurveySuite.Model;
using Autodesk.Civil.DatabaseServices;

namespace _3DS_CivilSurveySuite.C3D2017
{
    public static class SurfaceExtensions
    {
        public static CivilSurface ToCivilSurface(this TinSurface surface)
        {
            return new CivilSurface
            {
                ObjectId = surface.ObjectId.Handle.ToString(),
                Name = surface.Name,
                Description = surface.Description
            };
        }

        //public static List<CivilSurface> ToListOfCivilSurfaces(this IEnumerable<TinSurface> surfaces)
        //{
        //    return surfaces.Select(surface => surface.ToCivilSurface()).ToList();
        //}

        //public static List<TinSurface> ToListOfTinSurfaces(this IEnumerable<CivilSurface> surfaces, Transaction tr)
        //{
        //    return surfaces.Select(surface => surface.ToSurface(tr)).ToList();
        //}

        //public static TinSurface ToSurface(this CivilSurface surface, Transaction tr)
        //{
        //    Handle h = new Handle(long.Parse(surface.ObjectId, NumberStyles.AllowHexSpecifier));
        //    ObjectId id = ObjectId.Null;
        //    AcadApp.ActiveDatabase.TryGetObjectId(h, out id);//TryGetObjectId method

        //    return tr.GetObject(id, OpenMode.ForRead) as TinSurface;
        //}
    }
}