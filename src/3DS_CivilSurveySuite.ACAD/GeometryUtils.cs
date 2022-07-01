﻿using Autodesk.AutoCAD.Geometry;

namespace _3DS_CivilSurveySuite.ACAD
{
    public static class GeometryUtils
    {
        public static Plane PlaneXY { get; } = new Plane(new Point3d(0.0, 0.0, 0.0), Vector3d.ZAxis);
    }
}
