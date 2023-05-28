using System;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.Civil.DatabaseServices;
using Autodesk.Civil.DatabaseServices.Styles;
using CivilSurveySuite.ACAD;
using CivilSurveySuite.Shared.Models;

namespace CivilSurveySuite.CIVIL
{
    public static class LabelUtils
    {
        /// <summary>
        /// Gets the angle of the first text component in the style.
        /// </summary>
        /// <returns>A double representing angle of the label.</returns>
        public static double GetFirstComponentAngle(LabelStyle style)
        {
            foreach (ObjectId componentId in style.GetComponents(LabelStyleComponentType.Text))
            {
                var component = componentId.GetObject(OpenMode.ForRead) as LabelStyleComponent;

                if (component == null)
                    throw new InvalidOperationException("component was null.");

                if (component.GetType() == typeof(LabelStyleTextComponent))
                {
                    var textComponent = component as LabelStyleTextComponent;

                    if (textComponent == null)
                        throw new InvalidOperationException("textComponent was null.");

                    return textComponent.Text.Angle.Value;
                }
            }
            return 0;
        }

        /// <summary>
        /// Gets the overall total height of the <see cref="LabelStyle"/>
        /// based on the height of each text component.
        /// </summary>
        /// <param name="style">The <see cref="LabelStyle"/>.</param>
        /// <returns>A double value representing the total height of the <see cref="LabelStyle"/>.</returns>
        /// <exception cref="InvalidOperationException"></exception>
        public static double GetHeight(LabelStyle style)
        {
            double calculatedHeight = 0;

            foreach (ObjectId componentId in style.GetComponents(LabelStyleComponentType.Text))
            {
                var component = componentId.GetObject(OpenMode.ForRead) as LabelStyleComponent;

                if (component == null)
                    throw new InvalidOperationException("component was null.");

                if (component.GetType() != typeof(LabelStyleTextComponent))
                    continue;

                var textComponent = component as LabelStyleTextComponent;

                if (textComponent == null)
                    throw new InvalidOperationException("textComponent was null.");

                calculatedHeight += textComponent.Text.Height.Value;
                double currentScale = 1000 / SystemVariables.CANNOSCALEVALUE;
                calculatedHeight *= currentScale;
            }
            return calculatedHeight;
        }

        /// <summary>
        /// Turns label mark on or off.
        /// </summary>
        /// <param name="labelStyle"></param>
        /// <param name="tr"></param>
        /// <param name="value"></param>
        public static void LabelMask(this LabelStyle labelStyle, Transaction tr, bool value)
        {
            // Set dragged state mask.
            labelStyle.Properties.DraggedStateComponents.UseBackgroundMask.Value = value;

            try
            {
                // LabelStyleTextComponent
                foreach (ObjectId textComponentId in labelStyle.GetComponents(LabelStyleComponentType.Text))
                {
                    var textComponent = tr.GetObject(textComponentId, OpenMode.ForWrite) as LabelStyleTextComponent;

                    if (textComponent == null)
                        throw new InvalidOperationException("textComponent was null.");

                    textComponent.Border.BackgroundMask.Value = value;
                }
            }
            catch (ArgumentException)
            {
            }

            try
            {
                // ReferenceText
                foreach (ObjectId referenceTextId in labelStyle.GetComponents(LabelStyleComponentType.ReferenceText))
                {
                    var referenceText = tr.GetObject(referenceTextId, OpenMode.ForWrite) as LabelStyleReferenceTextComponent;

                    if (referenceText == null)
                        throw new InvalidOperationException("referenceText was null.");

                    referenceText.Border.BackgroundMask.Value = value;
                }
            }
            catch (ArgumentException)
            {
            }

            try
            {
                // ForEachText
                foreach (ObjectId foreachTextId in labelStyle.GetComponents(LabelStyleComponentType.TextForEach))
                {
                    var foreachText = tr.GetObject(foreachTextId, OpenMode.ForWrite) as LabelStyleTextForEachComponent;

                    if (foreachText == null)
                        throw new InvalidOperationException("foreachText was null.");

                    foreachText.Border.BackgroundMask.Value = value;
                }
            }
            catch (ArgumentException)
            {
            }
        }

        /// <summary>
        /// Checks each component in the label style and returns the maximum width of the text.
        /// </summary>
        /// <param name="tr">The active transaction.</param>
        /// <param name="labelStyle">The label style.</param>
        /// <returns>A int value containing the maximum width of the label style.</returns>
        public static int GetWidth(Transaction tr, LabelStyle labelStyle)
        {
            var maxWidth = 0.0;
            foreach (ObjectId componentId in labelStyle.GetComponents(LabelStyleComponentType.Text))
            {
                var component = tr.GetObject(componentId, OpenMode.ForRead) as LabelStyleComponent;

                if (component == null)
                    continue;

                if (component.GetType() == typeof(LabelStyleTextComponent))
                {
                    var textComponent = component as LabelStyleTextComponent;

                    if (textComponent == null)
                        continue;

                    double value = Math.Round(textComponent.Text.MaxWidth.Value * 1000, 3);

                    if (value > maxWidth)
                        maxWidth = value;
                }
            }

            return Convert.ToInt32(maxWidth);
        }

        /// <summary>
        /// Gets the first component of type T in the LabelStyle.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="style">The LabelStyle.</param>
        /// <param name="componentType">Matching LabelStyleComponentType</param>
        /// <returns>ObjectId</returns>
        public static ObjectId GetFirstComponentIdOfLabelStyle<T>(LabelStyle style, LabelStyleComponentType componentType)
            where T : LabelStyleComponent
        {
            foreach (ObjectId componentId in style.GetComponents(componentType))
            {
                var component = componentId.GetObject(OpenMode.ForRead) as T;

                if (component == null)
                {
                    throw new InvalidOperationException("component was null.");
                }

                if (component.GetType() == typeof(T))
                {
                    return component.ObjectId;
                }
            }

            return ObjectId.Null;
        }

        /// <summary>
        /// Command to override Text of a CogoPoint LabelStyle.
        /// </summary>
        public static void OverrideTextLabel()
        {
            if (!EditorUtils.TryGetEntityOfType<CogoPoint>("\nSelect CogoPoints label to override: ",
                    "\nPlease select CogoPoints only.", out var objectId))
            {
                return;
            }

            var overrideText = AcadApp.ShowInputDialog(new InputServiceOptions("Override Text", "Please enter the overriding text:", "OK"));

            if (string.IsNullOrEmpty(overrideText))
            {
                return;
            }

            using (var tr = AcadApp.StartTransaction())
            {
                var cogoPoint = (CogoPoint)tr.GetObject(objectId, OpenMode.ForRead);
                var labelStyle = (LabelStyle)tr.GetObject(cogoPoint.LabelStyleId, OpenMode.ForRead);
                var component = GetFirstComponentIdOfLabelStyle<LabelStyleTextComponent>(labelStyle, LabelStyleComponentType.Text);

                cogoPoint.UpgradeOpen();
                cogoPoint.SetLabelTextComponentOverride(component, overrideText);
                cogoPoint.DowngradeOpen();
                tr.Commit();
            }
        }
    }
}
