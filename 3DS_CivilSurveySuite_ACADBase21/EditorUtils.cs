// Copyright Scott Whitney. All Rights Reserved.
// Reproduction or transmission in whole or in part, any form or by any
// means, electronic, mechanical or otherwise, is prohibited without the
// prior written consent of the copyright owner.

using System;
using System.Collections.Generic;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Internal;
using Autodesk.AutoCAD.Runtime;

namespace _3DS_CivilSurveySuite_ACADBase21
{
    public static class EditorUtils
    {
        public static Point3d? GetBasePoint3d()
        {
            Utils.SetFocusToDwgView();
            var ppo = new PromptPointOptions("\n3DS> Select a base point: ");
            var ppr = AutoCADActive.Editor.GetPoint(ppo);

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

        public static PromptSelectionResult GetEntities<T>(string addMessage, string removeMessage = "")  where T : Entity
        {
            RXClass entityType = RXObject.GetClass(typeof(T));

            TypedValue[] typedValues = { new TypedValue((int)DxfCode.Start, entityType.DxfName) };
            var ss = new SelectionFilter(typedValues);
            var pso = new PromptSelectionOptions
            {
                MessageForAdding = addMessage,
                MessageForRemoval = removeMessage
            };

            return AutoCADActive.Editor.GetSelection(pso, ss);
        }

        public static PromptEntityResult GetEntity(IEnumerable<Type> allowedClasses, string addMessage, string removeMessage = "")
        {
            var peo = new PromptEntityOptions(addMessage);
            peo.SetRejectMessage(removeMessage);

            foreach (Type type in allowedClasses)
            {
                peo.AddAllowedClass(type, true);
            }

            return AutoCADActive.Editor.GetEntity(peo);
        }
        
    }
}