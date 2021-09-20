using System.Collections.Generic;
using System.Linq;
using _3DS_CivilSurveySuite.Model;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.Civil.DatabaseServices;

namespace _3DS_CivilSurveySuite.C3D2017
{
    public static class PointGroupExtensions
    {

        public static CivilPointGroup ToCivilPointGroup(this PointGroup pointGroup)
        {
            return new CivilPointGroup
            {
                ObjectId = pointGroup.ObjectId.Handle.ToString(),
                Name = pointGroup.Name,
                Description = pointGroup.Description
            };
        }


        public static IEnumerable<CivilPointGroup> ToListOfCivilPointGroups(this IEnumerable<PointGroup> pointGroups)
        {
            return pointGroups.Select(pointGroup => pointGroup.ToCivilPointGroup()).ToList();
        }


        public static PointGroup ToPointGroup(this CivilPointGroup civilPointGroup, Transaction tr)
        {
            return PointGroupUtils.GetPointGroupByName(tr, civilPointGroup.Name);
        }

    }
}
