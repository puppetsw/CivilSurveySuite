// Copyright Scott Whitney. All Rights Reserved.
// Reproduction or transmission in whole or in part, any form or by any
// means, electronic, mechanical or otherwise, is prohibited without the
// prior written consent of the copyright owner.

using System;
using System.Collections.Generic;
using System.Linq;
using _3DS_CivilSurveySuite.Core;
using _3DS_CivilSurveySuite.Model;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Internal;
using Autodesk.AutoCAD.Runtime;

namespace _3DS_CivilSurveySuite.ACAD2017
{
    /// <summary>
    /// Editor utilities class for handling common editor functions.
    /// </summary>
    /// <remarks>Scott Whitney, 23/11/2021.</remarks>
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
            pdo.Keywords.Add(Keywords.PICK);
            pdo.AppendKeywordsToMessage = true;

            var cancelled = false;
            do
            {
                var pdoResult = AcadApp.Editor.GetDouble(pdo);

                if (pdoResult.Status == PromptStatus.Keyword)
                {
                    if (pdoResult.StringResult != Keywords.PICK)
                        break;

                    if (PickAngle(out angle, pickMessage, basePoint))
                        break;
                }
                else if (pdoResult.Status == PromptStatus.OK)
                {
                    angle = new Angle(pdoResult.Value);
                    cancelled = true;
                }
                else
                {
                    cancelled = true;
                }
            } while (!cancelled);

            return true;
        }

        /// <summary>
        /// Lets the user pick or enter an angle.
        /// </summary>
        /// <param name="angle">The picked or entered angle.</param>
        /// <param name="pickMessage">The pick message to display to the user.</param>
        /// <param name="basePoint">Basepoint to start the pick from.</param>
        /// <param name="useBasePoint">If true use the basepoint.</param>
        /// <param name="useAngleBase">Use drawing angle base in ACAD.</param>
        /// <returns></returns>
        private static bool PickAngle(out Angle angle, string pickMessage, Point3d basePoint, bool useBasePoint = true, bool useAngleBase = false)
        {
            var pao = new PromptAngleOptions(pickMessage)
            {
                UseBasePoint = useBasePoint,
                BasePoint = basePoint,
                UseAngleBase = useAngleBase
            };
            angle = null;
            while (true)
            {
                var pdrAngle = AcadApp.ActiveDocument.Editor.GetAngle(pao);

                switch (pdrAngle.Status)
                {
                    case PromptStatus.OK:
                        angle = AngleHelpers.RadiansToAngle(pdrAngle.Value).ToClockwise();
                        return true;
                    case PromptStatus.Cancel:
                        return false;
                }
            }
        }

        [Obsolete("This method is obsolete. Use GetPoint()", false)]
        public static Point2d? GetBasePoint2d()
        {
            var point = GetBasePoint3d();
            return point != null ? (Point2d?)new Point2d(point.Value.X, point.Value.Y) : null;
        }

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

            var pdrDistance = AcadApp.ActiveDocument.Editor.GetDistance(pdo);

            if (pdrDistance.Status != PromptStatus.OK)
                return false;

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

            var pdrDistance = AcadApp.ActiveDocument.Editor.GetDistance(pdo);

            if (pdrDistance.Status != PromptStatus.OK)
                return false;

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
        /// <param name="allowZero"></param>
        /// <returns><c>true</c> if a double was successfully entered, <c>false</c> otherwise.</returns>
        public static bool GetDouble(out double value, string message, bool useDefaultValue = false, double defaultValue = 0, bool allowZero = true)
        {
            value = double.MinValue;

            var pdo = new PromptDoubleOptions(message);

            if (useDefaultValue)
                pdo.DefaultValue = defaultValue;

            pdo.AllowZero = allowZero;

            var pdr = AcadApp.Editor.GetDouble(pdo);

            if (pdr.Status != PromptStatus.OK)
                return false;

            value = pdr.Value;
            return true;
        }

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

        /// <summary>
        /// Gets a selection of type T with option for keywords.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="objectIds">The object ids.</param>
        /// <param name="addMessage">The add message.</param>
        /// <param name="removeMessage">The remove message.</param>
        /// <param name="keyword"></param>
        /// <param name="keywords"></param>
        /// <param name="defaultKeyword"></param>
        /// <returns><c>true</c> if successfully got a selection, <c>false</c> otherwise.</returns>
        public static bool GetSelectionOfType<T>(out ObjectIdCollection objectIds, string addMessage, string removeMessage, out string keyword, string[] keywords, string defaultKeyword = "")
        {
            var entityType = RXObject.GetClass(typeof(T));
            TypedValue[] typedValues = { new TypedValue((int)DxfCode.Start, entityType.DxfName) };
            return GetSelection(out objectIds, typedValues, addMessage, removeMessage, out keyword, keywords, defaultKeyword);
        }

        public static bool GetSelectionOfType<T1, T2>(out ObjectIdCollection objectIds, string addMessage, string removeMessage = "")
            where T1 : Entity where T2 : Entity
        {
            var entityType1 = RXObject.GetClass(typeof(T1));
            var entityType2 = RXObject.GetClass(typeof(T2));

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
            var entityType1 = RXObject.GetClass(typeof(T1));
            var entityType2 = RXObject.GetClass(typeof(T2));
            var entityType3 = RXObject.GetClass(typeof(T3));

            var dxfNames = $"{entityType1.DxfName},{entityType2.DxfName},{entityType3.DxfName}";

            TypedValue[] typedValues =
            {
                new TypedValue((int)DxfCode.Start, dxfNames),
            };

            return GetSelection(out objectIds, typedValues, addMessage, removeMessage);
        }

        /// <summary>
        /// Gets the an entity of type <see cref="T"/>.
        /// </summary>
        /// <typeparam name="T">Entity type.</typeparam>
        /// <param name="objectId">The object identifier.</param>
        /// <param name="addMessage">The add message.</param>
        /// <param name="removeMessage">The remove message.</param>
        /// <param name="exactMatch">Set to true if you want the type to be an exact match.</param>
        /// <returns><c>True</c> if an entity was selected, <c>false</c> otherwise.</returns>
        public static bool GetEntityOfType<T>(out ObjectId objectId, string addMessage, string removeMessage = "", bool exactMatch = false)
        {
            //TODO: Add keyword support.
            var peo = new PromptEntityOptions(addMessage);
            peo.SetRejectMessage(removeMessage);

            objectId = ObjectId.Null;

            peo.AddAllowedClass(typeof(T), exactMatch);

            var entity = AcadApp.Editor.GetEntity(peo);

            if (entity.Status != PromptStatus.OK)
                return false;

            objectId = entity.ObjectId;
            return true;
        }

        public static bool GetEntityOfType<T>(out ObjectId objectId, out Point3d pickedPoint, string addMessage, string removeMessage = "", bool exactMatch = false)
        {
            var peo = new PromptEntityOptions(addMessage);
            peo.SetRejectMessage(removeMessage);

            objectId = ObjectId.Null;
            pickedPoint = Point3d.Origin;

            peo.AddAllowedClass(typeof(T), exactMatch);

            var entity = AcadApp.Editor.GetEntity(peo);

            if (entity.Status != PromptStatus.OK)
                return false;

            pickedPoint = entity.PickedPoint;
            objectId = entity.ObjectId;
            return true;
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

            var entity = AcadApp.Editor.GetEntity(peo);

            if (entity.Status != PromptStatus.OK)
                return false;

            objectId = entity.ObjectId;
            return true;
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
        /// <param name="objectId"></param>
        /// <param name="pickedPoint"></param>
        /// <param name="allowedClasses"></param>
        /// <param name="addMessage"></param>
        /// <param name="removeMessage"></param>
        /// <returns></returns>
        public static bool GetEntity(out ObjectId objectId, out Point3d pickedPoint, IEnumerable<Type> allowedClasses, string addMessage, string removeMessage = "")
        {
            var peo = new PromptEntityOptions(addMessage);
            peo.SetRejectMessage(removeMessage);

            objectId = ObjectId.Null;
            pickedPoint = Point3d.Origin;

            foreach (var type in allowedClasses)
                peo.AddAllowedClass(type, true);

            var entity = AcadApp.Editor.GetEntity(peo);

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
        /// Gets a selection set. Selection set uses <param name="typedValues">TypedValues[]</param>
        /// to determine the selection filter.
        /// </summary>
        /// <param name="objectIds">The objectids of the selected entities.</param>
        /// <param name="typedValues">The typed values to filter by.</param>
        /// <param name="addMessage">The add message.</param>
        /// <param name="removeMessage">The remove message.</param>
        /// <param name="keyword">The keyword.</param>
        /// <param name="keywords">The keywords.</param>
        /// <param name="defaultKeyword"></param>
        /// <returns><c>true</c> if selection set was successful, <c>false</c> otherwise.</returns>
        public static bool GetSelection(out ObjectIdCollection objectIds, TypedValue[] typedValues, string addMessage,
            string removeMessage, out string keyword, string[] keywords, string defaultKeyword = "")
        {
            if (typedValues == null)
            {
                throw new ArgumentNullException(nameof(typedValues));
            }

            keyword = string.Empty;
            objectIds = new ObjectIdCollection();
            var filter = new SelectionFilter(typedValues);
            var pso = new PromptSelectionOptions
            {
                MessageForAdding = addMessage,
                MessageForRemoval = removeMessage
            };

            if (keywords != null)
            {
                pso.KeywordInput += OnKeywordInput;

                foreach (string word in keywords)
                {
                    if (!string.IsNullOrEmpty(word))
                    {
                        pso.Keywords.Add(word);
                    }
                }

                if (!string.IsNullOrEmpty(defaultKeyword) && keywords.Contains(defaultKeyword))
                {
                    pso.Keywords.Default = defaultKeyword;
                }

                //Build messages
                string kws = pso.Keywords.GetDisplayString(false);
                pso.MessageForAdding = addMessage + kws;
                pso.MessageForRemoval = removeMessage + kws;
            }

            try
            {
                var psr = AcadApp.Editor.GetSelection(pso, filter);
                if (psr.Status != PromptStatus.OK)
                {
                    return false;
                }

                foreach (SelectedObject selectedObject in psr.Value)
                {
                    objectIds.Add(selectedObject.ObjectId);
                }
            }
            catch (Autodesk.AutoCAD.Runtime.Exception e)
            {
                //Keyword pressed?
                if (e.ErrorStatus == ErrorStatus.OK)
                {
                    keyword = e.Message;
                    pso.KeywordInput -= OnKeywordInput; //unhook event handler.
                }
            }
            return true;
        }

        /// <summary>
        /// Handles the <see cref="E:KeywordInput" /> event.
        /// </summary>
        /// <param name="o">The sender object.</param>
        /// <param name="e">The <see cref="SelectionTextInputEventArgs"/> instance containing the event data.</param>
        /// <exception cref="Autodesk.AutoCAD.Runtime.Exception"></exception>
        private static void OnKeywordInput(object o, SelectionTextInputEventArgs e)
        {
            throw new Autodesk.AutoCAD.Runtime.Exception(ErrorStatus.OK, e.Input);
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
        /// <param name="useDefaultValue">Whether or not to use the default value.</param>
        /// <param name="defaultValue">The default value to use.</param>
        /// <param name="allowSpaces">Whether or not to allow spaces as input.</param>
        /// <returns><c>True</c> if got a string successfully, <c>false</c> otherwise.</returns>
        public static bool GetString(out string input, string message, bool useDefaultValue = false, string defaultValue = "", bool allowSpaces = false)
        {
            input = string.Empty;
            var pso = new PromptStringOptions(message) { AllowSpaces = allowSpaces };

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

        public static void ZoomToWindow(Point3d minPoint, Point3d maxPoint)
        {
            using (var tr = AcadApp.StartTransaction())
            using (var view = AcadApp.Editor.GetCurrentView())
            {
                double ratio = view.Width / view.Height;
                double width = maxPoint.X - minPoint.X;
                double height = maxPoint.Y - minPoint.Y;
                if (width > (height * ratio))
                    height = width / ratio;

                var center = new Point2d((maxPoint.X + minPoint.X) / 2.0, (maxPoint.Y + minPoint.Y) / 2.0);

                view.Width = width;
                view.Height = height;
                view.CenterPoint = center;

                AcadApp.Editor.SetCurrentView(view);
                tr.Commit();
            }
        }
    }
}