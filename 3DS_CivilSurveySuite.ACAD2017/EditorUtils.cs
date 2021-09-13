// Copyright Scott Whitney. All Rights Reserved.
// Reproduction or transmission in whole or in part, any form or by any
// means, electronic, mechanical or otherwise, is prohibited without the
// prior written consent of the copyright owner.

using System;
using System.Collections.Generic;
using _3DS_CivilSurveySuite.Core;
using _3DS_CivilSurveySuite.Model;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Internal;
using Autodesk.AutoCAD.Runtime;

namespace _3DS_CivilSurveySuite.ACAD2017
{
    public static class EditorUtils
    {
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
        [Obsolete("This method is obsolete. Use GetAngle (bool) instead. To be removed.", true)]
        public static Angle GetAngle(string message, Point3d? basePoint = null)
        {
            if (basePoint == null)
                basePoint = GetBasePoint3d();

            // If base point is still null, return null.
            if (basePoint == null)
                return null;

            var pao = new PromptAngleOptions(message) { UseBasePoint = true, BasePoint = basePoint.Value, UseAngleBase = false };

            PromptDoubleResult pdrAngle = AcadApp.ActiveDocument.Editor.GetAngle(pao);

            if (pdrAngle.Status != PromptStatus.OK)
                return null;

            return AngleHelpers.RadiansToAngle(pdrAngle.Value).ToClockwise();
        }

        /// <summary>
        /// Gets a <see cref="Angle"/> from user input.
        /// </summary>
        /// <param name="angle">The angle.</param>
        /// <param name="message">The message to display to the user.</param>
        /// <param name="basePoint">Optional base point parameter. If null, will prompt
        /// the user to select a base point.</param>
        /// <param name="pickMessage"></param>
        /// <returns>Angle. Null if cancelled or empty.</returns>
        /// <remarks>Ignores AutoCADs ANGDIR, and ANGBASE variables. AutoCAD also
        /// returns the radians in a counter-clockwise direction. We need to use
        /// the AngleToClockwise extension method to correct this.</remarks>
        public static bool GetAngle(out Angle angle, string message, Point3d basePoint, string pickMessage = "")
        {
            angle = null;

            var pdo = new PromptDoubleOptions(message);
            pdo.Keywords.Add(Keywords.Pick);
            pdo.AppendKeywordsToMessage = true;

            var cancelled = false;
            do
            {
                PromptDoubleResult pdoResult = AcadApp.Editor.GetDouble(pdo);
                switch (pdoResult.Status)
                {
                    case PromptStatus.Keyword:
                        if (pdoResult.StringResult == Keywords.Pick)
                        {
                            var pao = new PromptAngleOptions(pickMessage) { UseBasePoint = true, BasePoint = basePoint, UseAngleBase = false };
                            var innerCancelled = false;
                            do
                            {
                                PromptDoubleResult pdrAngle = AcadApp.ActiveDocument.Editor.GetAngle(pao);

                                switch (pdrAngle.Status)
                                {
                                    case PromptStatus.OK:
                                        angle = AngleHelpers.RadiansToAngle(pdrAngle.Value).ToClockwise();
                                        cancelled = true;
                                        innerCancelled = true;
                                        break;
                                    case PromptStatus.Cancel:
                                        innerCancelled = true;
                                        break;
                                }
                            } while (!innerCancelled);
                        }

                        break;
                    case PromptStatus.OK:
                        angle = new Angle(pdoResult.Value);
                        cancelled = true;
                        break;
                    case PromptStatus.Cancel:
                        cancelled = true;
                        break;
                }
            } while (!cancelled);

            return true;
        }

        //TODO: Remove this method.
        [Obsolete("This method is obsolete. Use GetPoint()", false)]
        public static Point2d? GetBasePoint2d()
        {
            var point = GetBasePoint3d();
            return point != null ? (Point2d?)new Point2d(point.Value.X, point.Value.Y) : null;
        }

        //TODO: Remove this method.
        [Obsolete("This method is obsolete. Use GetPoint()", false)]
        public static Point3d? GetBasePoint3d()
        {
            Utils.SetFocusToDwgView();
            var ppo = new PromptPointOptions("\n3DS> Select a base point: ");
            var ppr = AcadApp.Editor.GetPoint(ppo);

            if (ppr.Status != PromptStatus.OK)
            {
                return null;
            }

            return ppr.Value;
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
        [Obsolete("This method is obsolete. Use GetDistance (bool) instead. To be removed.", true)]
        public static double GetDistance(string message, Point3d? basePoint = null)
        {
            if (basePoint == null)
                basePoint = GetBasePoint3d();

            // If base point is still null, return null.
            if (basePoint == null)
                return double.NaN;

            var pdo = new PromptDistanceOptions(message) { BasePoint = basePoint.Value, Only2d = true, UseDashedLine = true };

            PromptDoubleResult pdrDistance = AcadApp.ActiveDocument.Editor.GetDistance(pdo);

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
        public static bool GetDistance(out double distance, string message, Point3d basePoint)
        {
            distance = double.NaN;
            var pdo = new PromptDistanceOptions(message)
            {
                BasePoint = basePoint,
                UseBasePoint = true,
                Only2d = true,
                UseDashedLine = true,
                AllowNone = true
            };

            PromptDoubleResult pdrDistance = AcadApp.ActiveDocument.Editor.GetDistance(pdo);

            if (pdrDistance.Status != PromptStatus.OK)
                return false;

            //UNDONE: If nothing entered, 0 distance.
            //if (string.IsNullOrEmpty(pdrDistance.StringResult) && pdrDistance.Value == 0)
            //    return false;

            distance = pdrDistance.Value;

            return true;
        }

        /// <summary>
        /// Gets the distance from the user input.
        /// </summary>
        /// <param name="distance">A double containing the output distance.</param>
        /// <param name="message">The message to display to the user.</param>
        /// <returns><c>true</c> if got distance successfully, <c>false</c> otherwise.</returns>
        public static bool GetDistance(out double distance, string message)
        {
            distance = double.NaN;

            var pdo = new PromptDistanceOptions(message)
            {
                Only2d = true,
                UseDashedLine = true,
                AllowNone = true
            };

            PromptDoubleResult pdrDistance = AcadApp.ActiveDocument.Editor.GetDistance(pdo);

            if (pdrDistance.Status != PromptStatus.OK)
                return false;

            //if (string.IsNullOrEmpty(pdrDistance.StringResult) && pdrDistance.Value == 0)
            //    return false;

            distance = pdrDistance.Value;

            return true;
        }

        /// <summary>
        /// Gets a double value from user input.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <param name="message">The message.</param>
        /// <param name="useDefaultValue">if set to <c>true</c> [use default value].</param>
        /// <param name="defaultValue">The default value.</param>
        /// <returns><c>true</c> if a double was successfully entered, <c>false</c> otherwise.</returns>
        public static bool GetDouble(out double value, string message, bool useDefaultValue = false, double defaultValue = 0)
        {
            value = double.MinValue;

            var pdo = new PromptDoubleOptions(message);

            if (useDefaultValue)
                pdo.DefaultValue = defaultValue;

            var pdr = AcadApp.Editor.GetDouble(pdo);

            if (pdr.Status != PromptStatus.OK)
                return false;

            value = pdr.Value;
            return true;
        }


        //TODO: Replace with bool method.
        [Obsolete("This method is obsolete. Use GetEntityOfType<T>(out ObjectId, String, String)", false)]
        public static PromptSelectionResult GetEntities<T>(string addMessage, string removeMessage = "") where T : Entity
        {
            RXClass entityType = RXObject.GetClass(typeof(T));

            TypedValue[] typedValues = { new TypedValue((int)DxfCode.Start, entityType.DxfName) };
            var ss = new SelectionFilter(typedValues);
            var pso = new PromptSelectionOptions
            {
                MessageForAdding = addMessage,
                MessageForRemoval = removeMessage
            };

            return AcadApp.Editor.GetSelection(pso, ss);
        }

        /// <summary>
        /// Gets the type of the entities of.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objectIds">The object ids.</param>
        /// <param name="addMessage">The add message.</param>
        /// <param name="removeMessage">The remove message.</param>
        /// <returns><c>true</c> if successfully got a selection, <c>false</c> otherwise.</returns>
        public static bool GetSelectionOfType<T>(out ObjectIdCollection objectIds, string addMessage, string removeMessage = "") where T : Entity
        {
            RXClass entityType = RXObject.GetClass(typeof(T));

            objectIds = new ObjectIdCollection();

            TypedValue[] typedValues = { new TypedValue((int)DxfCode.Start, entityType.DxfName) };
            var ss = new SelectionFilter(typedValues);
            var pso = new PromptSelectionOptions
            {
                MessageForAdding = addMessage,
                MessageForRemoval = removeMessage
            };

            var result = AcadApp.Editor.GetSelection(pso, ss);

            if (result.Status != PromptStatus.OK)
                return false;

            objectIds = new ObjectIdCollection(result.Value.GetObjectIds());

            return true;
        }

        public static bool GetSelectionOfType<T1, T2>(out ObjectIdCollection objectIds, string addMessage, string removeMessage = "")
            where T1 : Entity where T2 : Entity
        {
            RXClass entityType1 = RXObject.GetClass(typeof(T1));
            RXClass entityType2 = RXObject.GetClass(typeof(T2));

            var dxfNames = $"{entityType1.DxfName},{entityType2.DxfName}";

            TypedValue[] typedValues =
            {
                new TypedValue((int)DxfCode.Start, dxfNames),
            };

            return GetSelection(out objectIds, typedValues, addMessage, removeMessage);
        }

        public static bool GetSelectionOfType<T1, T2, T3>(out ObjectIdCollection objectIds, string addMessage, string removeMessage = "")
            where T1 : Entity where T2 : Entity where T3 : Entity
        {
            RXClass entityType1 = RXObject.GetClass(typeof(T1));
            RXClass entityType2 = RXObject.GetClass(typeof(T2));
            RXClass entityType3 = RXObject.GetClass(typeof(T3));

            var dxfNames = $"{entityType1.DxfName},{entityType2.DxfName},{entityType3.DxfName}";

            TypedValue[] typedValues =
            {
                new TypedValue((int)DxfCode.Start, dxfNames),
            };

            return GetSelection(out objectIds, typedValues, addMessage, removeMessage);
        }

        [Obsolete("This method is obsolete. Use GetEntity(ObjectId, IEnumerable<Type>, String, String)", false)]
        public static PromptEntityResult GetEntity(IEnumerable<Type> allowedClasses, string addMessage, string removeMessage = "")
        {
            var peo = new PromptEntityOptions(addMessage);
            peo.SetRejectMessage(removeMessage);

            foreach (Type type in allowedClasses)
            {
                peo.AddAllowedClass(type, true);
            }

            return AcadApp.Editor.GetEntity(peo);
        }

        /// <summary>
        /// Gets an entity's <see cref="ObjectId"/>. Selection is restricted by the <param name="allowedClasses">allowedClasses</param>
        /// </summary>
        /// <param name="objectId">The object identifier.</param>
        /// <param name="allowedClasses">The allowed classes.</param>
        /// <param name="addMessage">The add message.</param>
        /// <param name="removeMessage">The remove message.</param>
        /// <returns><c>true</c> if got the <see cref="ObjectId"/> successfully, <c>false</c> otherwise.</returns>
        public static bool GetEntity(out ObjectId objectId, IEnumerable<Type> allowedClasses, string addMessage, string removeMessage = "")
        {
            var peo = new PromptEntityOptions(addMessage);
            peo.SetRejectMessage(removeMessage);

            objectId = ObjectId.Null;

            foreach (Type type in allowedClasses)
                peo.AddAllowedClass(type, true);

            PromptEntityResult entity = AcadApp.Editor.GetEntity(peo);

            if (entity.Status != PromptStatus.OK)
                return false;

            objectId = entity.ObjectId;
            return true;
        }

        /// <summary>
        /// Gets the an entity of type <see cref="T"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objectId">The object identifier.</param>
        /// <param name="addMessage">The add message.</param>
        /// <param name="removeMessage">The remove message.</param>
        /// <param name="exactMatch">Set to true if you want the type to be an exact match.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        public static bool GetEntityOfType<T>(out ObjectId objectId, string addMessage, string removeMessage = "", bool exactMatch = false)
        {
            var peo = new PromptEntityOptions(addMessage);
            peo.SetRejectMessage(removeMessage);

            objectId = ObjectId.Null;

            peo.AddAllowedClass(typeof(T), exactMatch);

            PromptEntityResult entity = AcadApp.Editor.GetEntity(peo);

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

            PromptEntityResult entity = AcadApp.Editor.GetEntity(peo);

            if (entity.Status != PromptStatus.OK)
                return false;

            pickedPoint = entity.PickedPoint;
            objectId = entity.ObjectId;
            return true;
        }

        /// <summary>
        /// Gets a selection set. Selection set uses <param name="typedValues">TypedValues[]</param>
        /// to determine the selection filter.
        /// </summary>
        /// <param name="objectIds">List of <see cref="ObjectId"/> objects that the selection gets.</param>
        /// <param name="typedValues">The typed values to filter by</param>
        /// <param name="addMessage">The add message.</param>
        /// <param name="removeMessage">The remove message.</param>
        /// <returns><c>true</c> if selection set was successful, <c>false</c> otherwise.</returns>
        /// <remarks>
        /// <code>
        /// TypedValue[] acTypValAr = new TypedValue[1];
        /// acTypValAr.SetValue(new TypedValue((int)DxfCode.Start, "CIRCLE"), 0);
        /// </code>
        /// </remarks>
        public static bool GetSelection(out ObjectIdCollection objectIds, TypedValue[] typedValues, string addMessage, string removeMessage = "")
        {
            objectIds = new ObjectIdCollection();

            var filter = new SelectionFilter(typedValues);
            
            var pso = new PromptSelectionOptions
            {
                MessageForAdding = addMessage,
                MessageForRemoval = removeMessage
            };

            var psr = AcadApp.Editor.GetSelection(pso, filter);

            if (psr.Status != PromptStatus.OK)
                return false;

            foreach (SelectedObject selectedObject in psr.Value)
            {
                objectIds.Add(selectedObject.ObjectId);
            }

            return true;
        }

        /// <summary>
        /// Gets a integer from user input.
        /// </summary>
        /// <param name="input">The input.</param>
        /// <param name="message">The message.</param>
        /// <param name="useDefaultValue"></param>
        /// <param name="defaultValue"></param>
        /// <returns><c>true</c> if a integer was input successfully, <c>false</c> otherwise.</returns>
        public static bool GetInt(out int input, string message, bool useDefaultValue = false, int defaultValue = 0)
        {
            input = int.MinValue;

            var pio = new PromptIntegerOptions(message);

            if (useDefaultValue)
                pio.DefaultValue = defaultValue;

            var pir = AcadApp.Editor.GetInteger(pio);

            if (pir.Status != PromptStatus.OK)
                return false;

            input = pir.Value;
            return true;
        }

        [Obsolete("This method is obsolete and will not be used. To be removed.", true)]
        public static PromptNestedEntityResult GetNestedEntity(string message)
        {
            var pneo = new PromptNestedEntityOptions(message) { AllowNone = false };
            return AcadApp.Editor.GetNestedEntity(pneo);
        }

        /// <summary>
        /// Gets the nested entity.
        /// </summary>
        /// <param name="entityResult">The entity result.</param>
        /// <param name="message">The message.</param>
        /// <returns><c>true</c> if successfully got an entity, <c>false</c> otherwise.</returns>
        public static bool GetNestedEntity(out PromptNestedEntityResult entityResult, string message)
        {
            var pneo = new PromptNestedEntityOptions(message) { AllowNone = false };
            entityResult = AcadApp.Editor.GetNestedEntity(pneo);
            return entityResult.Status == PromptStatus.OK;
        }

        /// <summary>
        /// Gets a base <see cref="Point3d"/> from user input.
        /// </summary>
        /// <param name="basePoint">The base point output.</param>
        /// <param name="message">The message to display to the user in the command line.</param>
        /// <returns><c>true</c> if base point successfully got, <c>false</c> otherwise.</returns>
        public static bool GetPoint(out Point3d basePoint, string message)
        {
            //Utils.SetFocusToDwgView(); //UNDONE: Not sure why this was needed?
            var ppo = new PromptPointOptions(message)
            {
                AllowNone = true
            };

            var ppr = AcadApp.Editor.GetPoint(ppo);
            basePoint = Point3d.Origin;

            if (ppr.Status != PromptStatus.OK || ppr.StringResult == string.Empty)
                return false;

            basePoint = ppr.Value;
            return true;
        }

        /// <summary>
        /// Gets a base <see cref="Point2d"/> from user input.
        /// </summary>
        /// <param name="basePoint">The base point output.</param>
        /// <param name="message">The message to display to the user in the command line.</param>
        /// <returns><c>true</c> if base point successfully got, <c>false</c> otherwise.</returns>
        public static bool GetPoint(out Point2d basePoint, string message)
        {
            Utils.SetFocusToDwgView();
            var ppo = new PromptPointOptions(message);
            var ppr = AcadApp.Editor.GetPoint(ppo);
            basePoint = Point2d.Origin;

            if (ppr.Status != PromptStatus.OK)
                return false;

            basePoint = ppr.Value.ToPoint2d();
            return true;
        }

        /// <summary>
        /// Gets a string from user input.
        /// </summary>
        /// <param name="input">The typed input string.</param>   
        /// <param name="message">The message to display to the user.</param>
        /// <param name="useDefaultValue"></param>
        /// <param name="defaultValue"></param>
        /// <returns><c>true</c> if got a string successfully, <c>false</c> otherwise.</returns>
        public static bool GetString(out string input, string message, bool useDefaultValue = false, string defaultValue = "")
        {
            input = string.Empty;
            var pso = new PromptStringOptions(message) { AllowSpaces = false };

            if (useDefaultValue)
                pso.DefaultValue = defaultValue;

            var psr = AcadApp.ActiveDocument.Editor.GetString(pso);

            if (psr.Status != PromptStatus.OK)
                return false;

            input = psr.StringResult;
            return true;
        }

        public static bool IsType(ObjectId objectId, Type type)
        {
            RXClass classType = RXObject.GetClass(type);
            return objectId.ObjectClass.IsDerivedFrom(classType);
        }

        public static bool IsType<T>(this ObjectId objectId)
        {
            RXClass classType = RXObject.GetClass(typeof(T));
            return objectId.ObjectClass.IsDerivedFrom(classType);
        }

        public static bool IsType(this ObjectId objectId, IEnumerable<Type> types)
        {
            foreach (Type type in types)
            {
                if (IsType(objectId, type))
                    return true;
            }

            return false;
        }

        public static void ZoomExtents()
        {
            ZoomToWindow(AcadApp.ActiveDatabase.Extmin, AcadApp.ActiveDatabase.Extmax);
        }

        public static void ZoomToEntity(Entity entity)
        {
            ZoomToWindow(entity.GeometricExtents.MinPoint, entity.GeometricExtents.MaxPoint);
        }

        public static void ZoomToWindow(Point3d minPoint, Point3d maxPoint, double padPer = 0.0)
        {
            using (ViewTableRecord view = AcadApp.Editor.GetCurrentView())
            {
                view.Width = maxPoint.X - minPoint.X;
                view.Height = maxPoint.Y - minPoint.Y;
                view.CenterPoint = new Point2d(minPoint.X + view.Width / 2, minPoint.Y + view.Height / 2);

                AcadApp.Editor.SetCurrentView(view);
            }
        }

        public static void Zoom(Point3d pMin, Point3d pMax, Point3d pCenter, double dFactor)
        {
            // Get the current document and database
            Document acDoc = AcadApp.ActiveDocument;
            Database acCurDb = acDoc.Database;

            int nCurVport = Convert.ToInt32(Application.GetSystemVariable("CVPORT"));

            // Get the extents of the current space no points 
            // or only a center point is provided
            // Check to see if Model space is current
            if (acCurDb.TileMode == true)
            {
                if (pMin.Equals(new Point3d()) == true &&
                    pMax.Equals(new Point3d()) == true)
                {
                    pMin = acCurDb.Extmin;
                    pMax = acCurDb.Extmax;
                }
            }
            else
            {
                // Check to see if Paper space is current
                if (nCurVport == 1)
                {
                    // Get the extents of Paper space
                    if (pMin.Equals(new Point3d()) == true &&
                        pMax.Equals(new Point3d()) == true)
                    {
                        pMin = acCurDb.Pextmin;
                        pMax = acCurDb.Pextmax;
                    }
                }
                else
                {
                    // Get the extents of Model space
                    if (pMin.Equals(new Point3d()) == true &&
                        pMax.Equals(new Point3d()) == true)
                    {
                        pMin = acCurDb.Extmin;
                        pMax = acCurDb.Extmax;
                    }
                }
            }

            // Start a transaction
            using (Transaction acTrans = acCurDb.TransactionManager.StartTransaction())
            {
                // Get the current view
                using (ViewTableRecord acView = acDoc.Editor.GetCurrentView())
                {
                    Extents3d eExtents;

                    // Translate WCS coordinates to DCS
                    Matrix3d matWCS2DCS;
                    matWCS2DCS = Matrix3d.PlaneToWorld(acView.ViewDirection);
                    matWCS2DCS = Matrix3d.Displacement(acView.Target - Point3d.Origin) * matWCS2DCS;
                    matWCS2DCS = Matrix3d.Rotation(-acView.ViewTwist,
                        acView.ViewDirection,
                        acView.Target) * matWCS2DCS;

                    // If a center point is specified, define the min and max 
                    // point of the extents
                    // for Center and Scale modes
                    if (pCenter.DistanceTo(Point3d.Origin) != 0)
                    {
                        pMin = new Point3d(pCenter.X - (acView.Width / 2),
                            pCenter.Y - (acView.Height / 2), 0);

                        pMax = new Point3d((acView.Width / 2) + pCenter.X,
                            (acView.Height / 2) + pCenter.Y, 0);
                    }

                    // Create an extents object using a line
                    using (Line acLine = new Line(pMin, pMax))
                    {
                        eExtents = new Extents3d(acLine.Bounds.Value.MinPoint,
                            acLine.Bounds.Value.MaxPoint);
                    }

                    // Calculate the ratio between the width and height of the current view
                    double dViewRatio;
                    dViewRatio = (acView.Width / acView.Height);

                    // Tranform the extents of the view
                    matWCS2DCS = matWCS2DCS.Inverse();
                    eExtents.TransformBy(matWCS2DCS);

                    double dWidth;
                    double dHeight;
                    Point2d pNewCentPt;

                    // Check to see if a center point was provided (Center and Scale modes)
                    if (pCenter.DistanceTo(Point3d.Origin) != 0)
                    {
                        dWidth = acView.Width;
                        dHeight = acView.Height;

                        if (dFactor == 0)
                        {
                            pCenter = pCenter.TransformBy(matWCS2DCS);
                        }

                        pNewCentPt = new Point2d(pCenter.X, pCenter.Y);
                    }
                    else // Working in Window, Extents and Limits mode
                    {
                        // Calculate the new width and height of the current view
                        dWidth = eExtents.MaxPoint.X - eExtents.MinPoint.X;
                        dHeight = eExtents.MaxPoint.Y - eExtents.MinPoint.Y;

                        // Get the center of the view
                        pNewCentPt = new Point2d(((eExtents.MaxPoint.X + eExtents.MinPoint.X) * 0.5),
                            ((eExtents.MaxPoint.Y + eExtents.MinPoint.Y) * 0.5));
                    }

                    // Check to see if the new width fits in current window
                    if (dWidth > (dHeight * dViewRatio)) dHeight = dWidth / dViewRatio;

                    // Resize and scale the view
                    if (dFactor != 0)
                    {
                        acView.Height = dHeight * dFactor;
                        acView.Width = dWidth * dFactor;
                    }

                    // Set the center of the view
                    acView.CenterPoint = pNewCentPt;

                    // Set the current view
                    acDoc.Editor.SetCurrentView(acView);
                }

                // Commit the changes
                acTrans.Commit();
            }
        }


        public static string BuildTypedValueString()
        {
            //TODO: Implement this for easier typed values?



            throw new NotImplementedException();
        }



    }
}