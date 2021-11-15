﻿// Copyright Scott Whitney. All Rights Reserved.
// Reproduction or transmission in whole or in part, any form or by any
// means, electronic, mechanical or otherwise, is prohibited without the
// prior written consent of the copyright owner.

using System;
using _3DS_CivilSurveySuite.ACAD2017;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.Civil.DatabaseServices;
using Autodesk.Civil.DatabaseServices.Styles;

namespace _3DS_CivilSurveySuite.C3D2017
{
    public static class LabelUtils
    {
        /// <summary>
        /// Gets the angle of the first text component in the style.
        /// </summary>
        /// <returns>A double representing angle of the label.</returns>
        public static double GetLabelStyleComponentAngle(LabelStyle style)
        {
            foreach (ObjectId componentId in style.GetComponents(LabelStyleComponentType.Text))
            {
                var component = componentId.GetObject(OpenMode.ForRead) as LabelStyleComponent;

                if (component.GetType() == typeof(LabelStyleTextComponent))
                {
                    var textComponent = component as LabelStyleTextComponent;
                    return textComponent.Text.Angle.Value;
                }
            }
            return 0;
        }

        //BUG: What if there is more than 1 labelstyle component?
        public static double CalculateLabelHeight(LabelStyle style)
        {
            // Calculate the height of the label based on the text height of each component.
            double calculatedHeight = 0;

            foreach (ObjectId componentId in style.GetComponents(LabelStyleComponentType.Text))
            {
                var component = componentId.GetObject(OpenMode.ForRead) as LabelStyleComponent;

                if (component.GetType() == typeof(LabelStyleTextComponent))
                {
                    var textComponent = component as LabelStyleTextComponent;
                    calculatedHeight += textComponent.Text.Height.Value;
                    var currentScale = 1000 / SystemVariables.CANNOSCALEVALUE;
                    calculatedHeight *= currentScale;
                }
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
                    textComponent.Border.BackgroundMask.Value = value;
                }
            }
            catch (ArgumentException e)
            {
            }

            try
            {
                // ReferenceText
                foreach (ObjectId referenceTextId in labelStyle.GetComponents(LabelStyleComponentType.ReferenceText))
                {
                    var referenceText = tr.GetObject(referenceTextId, OpenMode.ForWrite) as LabelStyleReferenceTextComponent;
                    referenceText.Border.BackgroundMask.Value = value;
                }
            }
            catch (ArgumentException e)
            {
            }

            try
            {
                // ForEachText
                foreach (ObjectId foreachTextId in labelStyle.GetComponents(LabelStyleComponentType.TextForEach))
                {
                    var foreachText = tr.GetObject(foreachTextId, OpenMode.ForWrite) as LabelStyleTextForEachComponent;
                    foreachText.Border.BackgroundMask.Value = value;
                }
            }
            catch (ArgumentException e)
            {
            }
        }


        public static void GetLabelWidth(LabelStyle labelStyle)
        {

        }




    }
}