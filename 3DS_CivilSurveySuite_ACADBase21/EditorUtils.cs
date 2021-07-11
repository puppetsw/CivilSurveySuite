// Copyright Scott Whitney. All Rights Reserved.
// Reproduction or transmission in whole or in part, any form or by any
// means, electronic, mechanical or otherwise, is prohibited without the
// prior written consent of the copyright owner.

using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Internal;

namespace _3DS_CivilSurveySuite_ACADBase21
{
    public class EditorUtils
    {
        public static Point3d? GetBasePoint3d()
        {
            Utils.SetFocusToDwgView();
            var ppo = new PromptPointOptions("\n3DS> Select a base point: ");
            var ppr = AutoCADApplicationManager.Editor.GetPoint(ppo);

            if (ppr.Status != PromptStatus.OK)
            {
                return null;
            }
            
            return ppr.Value;
        }

        public static Point2d? GetBasePoint2d()
        {
            var point = GetBasePoint3d();
            return point != null ? (Point2d?) new Point2d(point.Value.X, point.Value.Y) : null;
        }


        
    }
}