// Copyright Scott Whitney. All Rights Reserved.
// Reproduction or transmission in whole or in part, any form or by any
// means, electronic, mechanical or otherwise, is prohibited without the
// prior written consent of the copyright owner.

using System;
using System.Collections.Generic;
using _3DS_CivilSurveySuite.Core;
using _3DS_CivilSurveySuite.Model;
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

        /// <summary>
        /// Gets a base <see cref="Point3d"/> from user input.
        /// </summary>
        /// <param name="basePoint">The base point output.</param>
        /// <param name="message">The message to display to the user in the command line.</param>
        /// <returns><c>true</c> if base point successfully got, <c>false</c> otherwise.</returns>
        public static bool GetBasePoint3d(out Point3d basePoint, string message)
        {
            Utils.SetFocusToDwgView();
            var ppo = new PromptPointOptions(message);
            var ppr = AutoCADActive.Editor.GetPoint(ppo);
            basePoint = Point3d.Origin;

            if (ppr.Status != PromptStatus.OK)
                return false;

            basePoint = ppr.Value;
            return true;
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

        public static PromptNestedEntityResult GetNestedEntity(string message)
        {
            var pneo = new PromptNestedEntityOptions(message) { AllowNone = false };
            return AutoCADActive.Editor.GetNestedEntity(pneo);
        }

        public static bool GetEntity(out ObjectId objectId, IEnumerable<Type> allowedClasses, string addMessage, string removeMessage = "")
        {
            var peo = new PromptEntityOptions(addMessage);
            peo.SetRejectMessage(removeMessage);

            objectId = ObjectId.Null;

            foreach (Type type in allowedClasses)
                peo.AddAllowedClass(type, true);

            PromptEntityResult entity = AutoCADActive.Editor.GetEntity(peo);

            if (entity.Status != PromptStatus.OK)
                return false;

            objectId = entity.ObjectId;
            return true;
        }

        public static bool GetEntity(out ObjectId objectId, out Point3d pickedPoint, IEnumerable<Type> allowedClasses, string addMessage, string removeMessage = "")
        {
            var peo = new PromptEntityOptions(addMessage);
            peo.SetRejectMessage(removeMessage);

            objectId = ObjectId.Null;
            pickedPoint = Point3d.Origin;

            foreach (Type type in allowedClasses)
                peo.AddAllowedClass(type, true);

            PromptEntityResult entity = AutoCADActive.Editor.GetEntity(peo);

            if (entity.Status != PromptStatus.OK)
                return false;

            pickedPoint = entity.PickedPoint;
            objectId = entity.ObjectId;
            return true;
        }

        /// <summary>
        /// Gets a <see cref="Angle"/> from user input.
        /// </summary>
        /// <param name="message">The message to display to the user.</param>
        /// <param name="basePoint">Optional base point parameter. If null, will prompt
        /// the user to select a base point.</param>
        /// <returns>Angle. Null if cancelled or empty.</returns>
        /// <remarks>Ignores AutoCADs ANGDIR, and ANGBASE variables. AutoCAD also
        /// returns the radians in a counter-clockwise direction. We need to use
        /// the AngleToClockwise extension method to correct this.</remarks>
        public static Angle GetAngle(string message, Point3d? basePoint = null)
        {
            if (basePoint == null)
                basePoint = GetBasePoint3d();

            // If base point is still null, return null.
            if (basePoint == null) 
                return null;

            var pao = new PromptAngleOptions(message) { UseBasePoint = true, BasePoint = basePoint.Value, UseAngleBase = false };

            PromptDoubleResult pdrAngle = AutoCADActive.ActiveDocument.Editor.GetAngle(pao);
            
            if (pdrAngle.Status != PromptStatus.OK)
                return null;

            return MathHelpers.RadiansToAngle(pdrAngle.Value).ToClockwise();
        }

        /// <summary>
        /// Gets a <see cref="Angle"/> from user input.
        /// </summary>
        /// <param name="angle">The angle.</param>
        /// <param name="message">The message to display to the user.</param>
        /// <param name="basePoint">Optional base point parameter. If null, will prompt
        /// the user to select a base point.</param>
        /// <returns>Angle. Null if cancelled or empty.</returns>
        /// <remarks>Ignores AutoCADs ANGDIR, and ANGBASE variables. AutoCAD also
        /// returns the radians in a counter-clockwise direction. We need to use
        /// the AngleToClockwise extension method to correct this.</remarks>
        public static bool GetAngle(out Angle angle, string message, Point3d basePoint)
        {
            angle = null;

            var pao = new PromptAngleOptions(message) { UseBasePoint = true, BasePoint = basePoint, UseAngleBase = false };

            PromptDoubleResult pdrAngle = AutoCADActive.ActiveDocument.Editor.GetAngle(pao);
            
            if (pdrAngle.Status != PromptStatus.OK)
                return false;

            angle = MathHelpers.RadiansToAngle(pdrAngle.Value).ToClockwise();

            return true;
        }

        /// <summary>
        /// Gets the distance from the user input.
        /// </summary>
        /// <param name="message">The message to display to the user.</param>
        /// <param name="basePoint">Optional base point parameter. If null, will prompt
        /// the user to select a base point.</param>
        /// <returns>A double value containing the distance.</returns>
        /// <remarks>If the action is cancelled returns a <see cref="double.NaN"/> which
        /// can be checked with double.IsNaN(distance).</remarks>
        public static double GetDistance(string message, Point3d? basePoint = null)
        {
            if (basePoint == null)
                basePoint = GetBasePoint3d();

            // If base point is still null, return null.
            if (basePoint == null) 
                return double.NaN;

            var pdo = new PromptDistanceOptions(message) { BasePoint = basePoint.Value, Only2d = true, UseDashedLine = true };

            PromptDoubleResult pdrDistance = AutoCADActive.ActiveDocument.Editor.GetDistance(pdo);

            if (pdrDistance.Status != PromptStatus.OK)
                return double.NaN;

            return pdrDistance.Value;
        }

        /// <summary>
        /// Gets the distance from the user input.
        /// </summary>
        /// <param name="distance">A double containing the output distance.</param>
        /// <param name="message">The message to display to the user.</param>
        /// <param name="basePoint">Optional base point parameter. If null, will prompt
        /// the user to select a base point.</param>
        /// <returns><c>true</c> if got distance successfully, <c>false</c> otherwise.</returns>
        public static bool GetDistance(out double distance, string message, Point3d? basePoint = null)
        {
            if (basePoint == null)
                basePoint = GetBasePoint3d();

            distance = double.NaN;

            // If base point is still null, return null.
            if (basePoint == null) 
                return false;

            var pdo = new PromptDistanceOptions(message) { BasePoint = basePoint.Value, Only2d = true, UseDashedLine = true };

            PromptDoubleResult pdrDistance = AutoCADActive.ActiveDocument.Editor.GetDistance(pdo);

            if (pdrDistance.Status != PromptStatus.OK)
                return false;

            distance = pdrDistance.Value;

            return true;
        }
    }
}