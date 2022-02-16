// Copyright Scott Whitney. All Rights Reserved.
// Reproduction or transmission in whole or in part, any form or by any
// means, electronic, mechanical or otherwise, is prohibited without the
// prior written consent of the copyright owner.

using System;
using System.Globalization;
using _3DS_CivilSurveySuite.UI.Helpers;
using Autodesk.AutoCAD.DatabaseServices;

namespace _3DS_CivilSurveySuite.ACAD2017
{
    /// <summary>
    /// Text utilities class for handling manipulation of text objects.
    /// </summary>
    /// <remarks>
    /// Scott Whitney, 23/11/2021.
    /// </remarks>
    public static class TextUtils
    {
        /// <summary>
        /// Creates a selection set with just TEXT and MTEXT entities.
        /// </summary>
        /// <param name="objectIds"><see cref="ObjectIdCollection"/> containing the selected entities.</param>
        /// <returns>True if the selection was successful, otherwise false.</returns>
        private static bool TrySelectText(out ObjectIdCollection objectIds)
        {
            var typedValue = new TypedValue[1];
            typedValue.SetValue(new TypedValue((int)DxfCode.Start, "TEXT,MTEXT"), 0);

            objectIds = new ObjectIdCollection();

            if (!EditorUtils.TryGetSelection("\n3DS> Select text entities: ", "\n>3DS Remove text entities: ",
                    typedValue, out var selectedTextIds))
                return false;

            objectIds = selectedTextIds;
            return true;
        }

        /// <summary>
        /// Updates either a TEXT or MTEXT entity with the updateText.
        /// </summary>
        /// <param name="textEntity">The entity to update.</param>
        /// <param name="updateText">The text to update the entity with.</param>
        /// <typeparam name="T">The base item type.</typeparam>
        private static void UpdateText<T>(T textEntity, string updateText) where T : Entity
        {
            if (textEntity.ObjectId.IsType<DBText>())
            {
                var textObj = textEntity as DBText;
                if (textObj != null &&
                    textObj.TextString != updateText &&
                    textObj.IsWriteEnabled)
                {
                    textObj.TextString = updateText;
                }
            }

            if (textEntity.ObjectId.IsType<MText>())
            {
                var textObj = textEntity as MText;
                if (textObj != null &&
                    textObj.Contents != updateText &&
                    textObj.IsWriteEnabled)
                {
                    textObj.Contents = updateText;
                }
            }
        }

        /// <summary>
        /// Gets the text from a TEXT or MTEXT entity.
        /// </summary>
        /// <param name="textEntity"></param>
        /// <typeparam name="T">The base item type.</typeparam>
        /// <returns>A string containing the text from the entity.</returns>
        private static string GetText<T>(T textEntity) where T : Entity
        {
            if (textEntity.ObjectId.IsType<DBText>())
            {
                var textObj = textEntity as DBText;
                if (textObj != null)
                {
                    return textObj.TextString;
                }
            }

            if (textEntity.ObjectId.IsType<MText>())
            {
                var textObj = textEntity as MText;
                if (textObj != null)
                {
                    return textObj.Contents;
                }
            }
            return string.Empty;
        }

        /// <summary>
        /// Strips any alpha characters from text leaving only a number value (if it contains one)
        /// </summary>
        public static void RemoveAlphaCharactersFromText()
        {
            if (!TrySelectText(out var objectIds))
                return;

            using (var tr = AcadApp.StartTransaction())
            {

                foreach (ObjectId objectId in objectIds)
                {
                    var textEnt = tr.GetObject(objectId, OpenMode.ForWrite) as Entity;

                    if (textEnt == null)
                        throw new InvalidOperationException("textEnd was null.");

                    string text = GetText(textEnt);
                    string cleanedString = StringHelpers.RemoveAlphaCharacters(text);

                    if (string.IsNullOrEmpty(cleanedString))
                        continue;

                    textEnt.UpgradeOpen();
                    UpdateText(textEnt, cleanedString);
                    textEnt.DowngradeOpen();
                }
                tr.Commit();
            }
        }

        /// <summary>
        /// Adds a prefix to the selected text objects
        /// </summary>
        public static void AddPrefixToText()
        {
            if (!EditorUtils.TryGetString("\n3DS> Enter prefix text: ", out string prefixText, allowSpaces: true))
                return;

            if (!TrySelectText(out var objectIds))
                return;

            using (var tr = AcadApp.StartTransaction())
            {
                foreach (ObjectId objectId in objectIds)
                {
                    var textEnt = tr.GetObject(objectId, OpenMode.ForWrite) as Entity;

                    if (textEnt == null)
                        throw new InvalidOperationException("textEnd was null.");

                    textEnt.UpgradeOpen();
                    string text = prefixText + GetText(textEnt);
                    UpdateText(textEnt, text);
                    textEnt.DowngradeOpen();
                }
                tr.Commit();
            }
        }

        /// <summary>
        /// Adds a suffix to the selected text objects.
        /// </summary>
        public static void AddSuffixToText()
        {
            if (!EditorUtils.TryGetString("\n3DS> Enter suffix text: ", out string suffixText))
                return;

            if (!TrySelectText(out var objectIds))
                return;

            using (var tr = AcadApp.StartTransaction())
            {
                foreach (ObjectId objectId in objectIds)
                {
                    var textEnt = tr.GetObject(objectId, OpenMode.ForWrite) as Entity;

                    if (textEnt == null)
                        throw new InvalidOperationException("textEnd was null.");

                    textEnt.UpgradeOpen();
                    string text = GetText(textEnt) + suffixText;
                    UpdateText(textEnt, text);
                    textEnt.DowngradeOpen();
                }
                tr.Commit();
            }


        }

        /// <summary>
        /// Converts the selected text to Uppercase
        /// </summary>
        public static void TextToUpper()
        {
            if (!TrySelectText(out var objectIds))
                return;

            using (var tr = AcadApp.StartTransaction())
            {
                foreach (ObjectId objectId in objectIds)
                {
                    var textEnt = tr.GetObject(objectId, OpenMode.ForWrite) as Entity;

                    if (textEnt == null)
                        throw new InvalidOperationException("textEnd was null.");

                    string text = GetText(textEnt).ToUpper();
                    textEnt.UpgradeOpen();
                    UpdateText(textEnt, text);
                    textEnt.DowngradeOpen();
                }
                tr.Commit();
            }
        }

        /// <summary>
        /// Converts the selected text to lowercase
        /// </summary>
        public static void TextToLower()
        {
            if (!TrySelectText(out var objectIds))
                return;

            using (var tr = AcadApp.StartTransaction())
            {
                foreach (ObjectId objectId in objectIds)
                {
                    var textEnt = tr.GetObject(objectId, OpenMode.ForWrite) as Entity;

                    if (textEnt == null)
                        throw new InvalidOperationException("textEnd was null.");

                    string text = GetText(textEnt).ToLower();

                    textEnt.UpgradeOpen();
                    UpdateText(textEnt, text);
                    textEnt.DowngradeOpen();
                }
                tr.Commit();
            }
        }

        /// <summary>
        /// Converts the selected text to sentence case
        /// </summary>
        public static void TextToSentence()
        {
            if (!TrySelectText(out var objectIds))
                return;

            using (var tr = AcadApp.StartTransaction())
            {
                foreach (ObjectId objectId in objectIds)
                {
                    var textEnt = tr.GetObject(objectId, OpenMode.ForRead) as Entity;

                    if (textEnt == null)
                        throw new InvalidOperationException("textEnd was null.");

                    string text = GetText(textEnt).ToSentence();

                    textEnt.UpgradeOpen();
                    UpdateText(textEnt, text);
                    textEnt.DowngradeOpen();
                }
                tr.Commit();
            }

        }

        /// <summary>
        /// Adds a number to a text entity if it is a valid number
        /// </summary>
        public static void AddNumberToText()
        {
            if (!TrySelectText(out var objectIds))
                return;

            if (!EditorUtils.TryGetDouble("\n3DS> Enter amount to add to text: ", out double addValue))
                return;

            using (var tr = AcadApp.StartTransaction())
            {
                foreach (ObjectId objectId in objectIds)
                {
                    var textEnt = tr.GetObject(objectId, OpenMode.ForRead) as Entity;

                    if (textEnt == null)
                        throw new InvalidOperationException("textEnd was null.");

                    string text = GetText(textEnt);

                    if (text.IsNumeric())
                    {
                        textEnt.UpgradeOpen();
                        double mathValue = Convert.ToDouble(text) + addValue;
                        UpdateText(textEnt, mathValue.ToString(CultureInfo.InvariantCulture));
                        textEnt.DowngradeOpen();
                    }
                    else
                    {
                        var entExtent = textEnt.GeometricExtents;
                        double midpointX = Math.Round((entExtent.MaxPoint.X+entExtent.MinPoint.X)/2, SystemVariables.LUPREC);
                        double midpointY = Math.Round((entExtent.MaxPoint.Y+entExtent.MinPoint.Y)/2, SystemVariables.LUPREC);

                        // ignoring text at...
                        AcadApp.WriteMessage($"Ignoring text at X:{midpointX} Y:{midpointY}. Not a number.");
                    }
                }
                tr.Commit();
            }
        }

        /// <summary>
        /// Subtracts a number from a text entity if it is a valid number
        /// </summary>
        public static void SubtractNumberFromText()
        {
            if (!TrySelectText(out var objectIds))
                return;

            if (!EditorUtils.TryGetDouble("\n3DS> Enter amount to subtract from text: ", out double subtractValue))
                return;

            using (var tr = AcadApp.StartTransaction())
            {
                foreach (ObjectId objectId in objectIds)
                {
                    var textEnt = tr.GetObject(objectId, OpenMode.ForRead) as Entity;

                    if (textEnt == null)
                        throw new InvalidOperationException("textEnd was null.");

                    string text = GetText(textEnt);

                    if (text.IsNumeric())
                    {
                        textEnt.UpgradeOpen();
                        double mathValue = Convert.ToDouble(text) - subtractValue;
                        UpdateText(textEnt, mathValue.ToString(CultureInfo.InvariantCulture));
                        textEnt.DowngradeOpen();
                    }
                    else
                    {
                        var entExtent = textEnt.GeometricExtents;
                        double midpointX = Math.Round((entExtent.MaxPoint.X+entExtent.MinPoint.X)/2, SystemVariables.LUPREC);
                        double midpointY = Math.Round((entExtent.MaxPoint.Y+entExtent.MinPoint.Y)/2, SystemVariables.LUPREC);

                        // ignoring text at...
                        AcadApp.WriteMessage($"Ignoring text at X:{midpointX} Y:{midpointY}. Not a number.");
                    }
                }
                tr.Commit();
            }
        }

        /// <summary>
        /// Multiplies the text by a number
        /// </summary>
        public static void MultiplyTextByNumber()
        {
            if (!TrySelectText(out var objectIds))
                return;

            if (!EditorUtils.TryGetDouble("\n3DS> Enter value to multiply by text: ", out double multiplyValue))
                return;

            using (var tr = AcadApp.StartTransaction())
            {
                foreach (ObjectId objectId in objectIds)
                {
                    var textEnt = tr.GetObject(objectId, OpenMode.ForRead) as Entity;

                    if (textEnt == null)
                        throw new InvalidOperationException("textEnd was null.");

                    string text = GetText(textEnt);

                    if (text.IsNumeric())
                    {
                        textEnt.UpgradeOpen();
                        double mathValue = Convert.ToDouble(text) * multiplyValue;
                        UpdateText(textEnt, mathValue.ToString(CultureInfo.InvariantCulture));
                        textEnt.DowngradeOpen();
                    }
                    else
                    {
                        var entExtent = textEnt.GeometricExtents;
                        double midpointX = Math.Round((entExtent.MaxPoint.X+entExtent.MinPoint.X)/2, SystemVariables.LUPREC);
                        double midpointY = Math.Round((entExtent.MaxPoint.Y+entExtent.MinPoint.Y)/2, SystemVariables.LUPREC);

                        // ignoring text at...
                        AcadApp.WriteMessage($"Ignoring text at X:{midpointX} Y:{midpointY}. Not a number.");
                    }
                }
                tr.Commit();
            }
        }

        /// <summary>
        /// Divides the text by a number
        /// </summary>
        public static void DivideTextByNumber()
        {
            if (!TrySelectText(out var objectIds))
                return;

            if (!EditorUtils.TryGetDouble("\n3DS> Enter value to divide by text: ", out double divideValue))
                return;

            using (var tr = AcadApp.StartTransaction())
            {
                foreach (ObjectId objectId in objectIds)
                {
                    var textEnt = tr.GetObject(objectId, OpenMode.ForRead) as Entity;

                    if (textEnt == null)
                        throw new InvalidOperationException("textEnd was null.");

                    string text = GetText(textEnt);

                    if (text.IsNumeric())
                    {
                        textEnt.UpgradeOpen();
                        double mathValue = Convert.ToDouble(text) / divideValue;
                        UpdateText(textEnt, mathValue.ToString(CultureInfo.InvariantCulture));
                        textEnt.DowngradeOpen();
                    }
                    else
                    {
                        var entExtent = textEnt.GeometricExtents;
                        double midpointX = Math.Round((entExtent.MaxPoint.X+entExtent.MinPoint.X)/2, SystemVariables.LUPREC);
                        double midpointY = Math.Round((entExtent.MaxPoint.Y+entExtent.MinPoint.Y)/2, SystemVariables.LUPREC);

                        // ignoring text at...
                        AcadApp.WriteMessage($"Ignoring text at X:{midpointX} Y:{midpointY}. Not a number.");
                    }
                }
                tr.Commit();
            }
        }
    }
}