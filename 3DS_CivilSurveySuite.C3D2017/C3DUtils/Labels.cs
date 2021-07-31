// Copyright Scott Whitney. All Rights Reserved.
// Reproduction or transmission in whole or in part, any form or by any
// means, electronic, mechanical or otherwise, is prohibited without the
// prior written consent of the copyright owner.

using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.Civil.DatabaseServices.Styles;

namespace _3DS_CivilSurveySuite.C3D2017.C3DUtils
{
    public class Labels
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

    }
}