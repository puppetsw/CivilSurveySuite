using System;
using System.Reflection;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.Civil.DatabaseServices;
using Autodesk.Civil.DatabaseServices.Styles;

namespace _3DS_CivilSurveySuite.C3D2017
{
    public static class FeatureLineUtils
    {
        public static ObjectId AddFeatureFromPoly(this Site site, Polyline poly, FeatureLineStyle style)
        {
            object acadObject = site.AcadObject;
            object[] args =
            {
                poly.AcadObject,
                style.AcadObject
            };

            object target = acadObject.GetType().InvokeMember("FeatureLines", BindingFlags.GetProperty, null, acadObject, null);
            return Autodesk.AutoCAD.DatabaseServices.DBObject.FromAcadObject(target.GetType().InvokeMember("AddFromPolylineEx", BindingFlags.InvokeMethod, null, target, args));
        }

        public static void FlattenFeatureLine(FeatureLine featureLine)
        {
            throw new NotSupportedException();
        }
    }
}
