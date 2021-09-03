using _3DS_CivilSurveySuite.Model;
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
    }
}
