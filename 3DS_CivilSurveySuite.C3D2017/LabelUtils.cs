// Copyright Scott Whitney. All Rights Reserved.
// Reproduction or transmission in whole or in part, any form or by any
// means, electronic, mechanical or otherwise, is prohibited without the
// prior written consent of the copyright owner.

using System;
using _3DS_CivilSurveySuite.ACAD2017;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.Civil.DatabaseServices.Styles;

namespace _3DS_CivilSurveySuite.C3D2017
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
        public static int GetLabelWidth(Transaction tr, LabelStyle labelStyle)
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

            // Ignore for dragged state?
            // if (labelStyle.Properties.DraggedStateComponents.MaxTextWidth.Value > maxWidth)
            //     maxWidth = labelStyle.Properties.DraggedStateComponents.MaxTextWidth.Value;

            return Convert.ToInt32(maxWidth);
        }
    }
}