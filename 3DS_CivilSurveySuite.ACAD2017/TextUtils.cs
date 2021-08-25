// Copyright Scott Whitney. All Rights Reserved.
// Reproduction or transmission in whole or in part, any form or by any
// means, electronic, mechanical or otherwise, is prohibited without the
// prior written consent of the copyright owner.

using System;
using _3DS_CivilSurveySuite.Core;
using Autodesk.AutoCAD.DatabaseServices;

namespace _3DS_CivilSurveySuite.ACAD2017
{
    public static class TextUtils
    {
        private static bool SelectText(out ObjectIdCollection objectIds, string message)
        {
            var typedValue = new TypedValue[1];
            typedValue.SetValue(new TypedValue((int)DxfCode.Start, "TEXT,MTEXT"), 0);

            objectIds = new ObjectIdCollection();

            if (!EditorUtils.GetSelection(out var selectedTextIds, typedValue, message))
                return false;

            objectIds = selectedTextIds;
            return true;
        }

        private static bool UpdateText<T>(T textEntity, string updateText) where T : Entity
        {
            if (textEntity.ObjectId.IsType<DBText>())
            {
                var textObj = textEntity as DBText;
                if (textObj != null &&
                    textObj.TextString != updateText &&
                    textObj.IsWriteEnabled)
                {
                    textObj.TextString = updateText;
                    return true;
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
                    return true;
                }
            }
            
            return false;
        }

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
            if (!SelectText(out var objectIds, "\n3DS> Select text entities: "))
                return;

            using (var tr = AcadApp.StartTransaction())
            {

                foreach (ObjectId objectId in objectIds)
                {
                    var textEnt = tr.GetObject(objectId, OpenMode.ForWrite) as Entity;
                    var text = GetText(textEnt);
                    var cleanedString = StringHelpers.RemoveAlphaCharacters(text);

                    if (!string.IsNullOrEmpty(cleanedString))
                    {
                        textEnt.UpgradeOpen();
                        var canUpdate = UpdateText(textEnt, cleanedString);
                        textEnt.DowngradeOpen();
                    }
                }
                tr.Commit();
            }
        }

        /// <summary>
        /// Adds a prefix to the selected text objects
        /// </summary>
        public static void AddPrefixToText()
        {
            //TODO: add space option?
            if (!EditorUtils.GetString(out string prefixText, "\n3DS> Enter prefix text: "))
                return;

            if (!SelectText(out var objectIds, "\n3DS> Select text entities: "))
                return;

            using (var tr = AcadApp.StartTransaction())
            {
                foreach (ObjectId objectId in objectIds)
                {
                    var textEnt = tr.GetObject(objectId, OpenMode.ForWrite) as Entity;
                    textEnt.UpgradeOpen();
                    var text = prefixText + GetText(textEnt);
                    var canUpdate = UpdateText(textEnt, text);
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
            if (!EditorUtils.GetString(out string suffixText, "\n3DS> Enter suffix text: "))
                return;

            if (!SelectText(out var objectIds, "\n3DS> Select text entities: "))
                return;

            using (var tr = AcadApp.StartTransaction())
            {
                foreach (ObjectId objectId in objectIds)
                {
                    var textEnt = tr.GetObject(objectId, OpenMode.ForWrite) as Entity;
                    textEnt.UpgradeOpen();
                    var text = GetText(textEnt) + suffixText;
                    var canUpdate = UpdateText(textEnt, text);
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
            if (!SelectText(out var objectIds, "\n3DS> Select text entities: "))
                return;

            using (var tr = AcadApp.StartTransaction())
            {
                foreach (ObjectId objectId in objectIds)
                {
                    var textEnt = tr.GetObject(objectId, OpenMode.ForWrite) as Entity;
                    var text = GetText(textEnt).ToUpper();

                    textEnt.UpgradeOpen();
                    var canUpdate = UpdateText(textEnt, text);
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
            if (!SelectText(out var objectIds, "\n3DS> Select text entities: "))
                return;

            using (var tr = AcadApp.StartTransaction())
            {
                foreach (ObjectId objectId in objectIds)
                {
                    var textEnt = tr.GetObject(objectId, OpenMode.ForWrite) as Entity;
                    var text = GetText(textEnt).ToLower();

                    textEnt.UpgradeOpen();
                    var canUpdate = UpdateText(textEnt, text);
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
            if (!SelectText(out var objectIds, "\n3DS> Select text entities: "))
                return;

            using (var tr = AcadApp.StartTransaction())
            {
                foreach (ObjectId objectId in objectIds)
                {
                    var textEnt = tr.GetObject(objectId, OpenMode.ForRead) as Entity;
                    var text = GetText(textEnt).ToSentence();

                    textEnt.UpgradeOpen();
                    var canUpdate = UpdateText(textEnt, text);
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
            if (!SelectText(out var objectIds, "\n3DS> Select text entities: "))
                return;

            if (!EditorUtils.GetDouble(out double addValue, "\n3DS> Enter amount to add to text: "))
                return;

            using (var tr = AcadApp.StartTransaction())
            {
                foreach (ObjectId objectId in objectIds)
                {
                    var textEnt = tr.GetObject(objectId, OpenMode.ForRead) as Entity;
                    var text = GetText(textEnt);

                    if (text.IsNumeric())
                    {
                        textEnt.UpgradeOpen();
                        var mathValue = Convert.ToDouble(text) + addValue;
                        var canUpdate = UpdateText(textEnt, mathValue.ToString()); //BUG: Possible culture issue.
                        textEnt.DowngradeOpen();
                    }
                    else
                    {
                        var entExtent = textEnt.GeometricExtents;
                        var midpointX = Math.Round((entExtent.MaxPoint.X+entExtent.MinPoint.X)/2, SystemVariables.LUPREC);
                        var midpointY = Math.Round((entExtent.MaxPoint.Y+entExtent.MinPoint.Y)/2, SystemVariables.LUPREC);

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
            if (!SelectText(out var objectIds, "\n3DS> Select text entities: "))
                return;

            if (!EditorUtils.GetDouble(out double subtractValue, "\n3DS> Enter amount to subtract from text: "))
                return;

            using (var tr = AcadApp.StartTransaction())
            {
                foreach (ObjectId objectId in objectIds)
                {
                    var textEnt = tr.GetObject(objectId, OpenMode.ForRead) as Entity;
                    var text = GetText(textEnt);

                    if (text.IsNumeric())
                    {
                        textEnt.UpgradeOpen();
                        var mathValue = Convert.ToDouble(text) - subtractValue;
                        var canUpdate = UpdateText(textEnt, mathValue.ToString()); //BUG: Possible culture issue.
                        textEnt.DowngradeOpen();
                    }
                    else
                    {
                        var entExtent = textEnt.GeometricExtents;
                        var midpointX = Math.Round((entExtent.MaxPoint.X+entExtent.MinPoint.X)/2, SystemVariables.LUPREC);
                        var midpointY = Math.Round((entExtent.MaxPoint.Y+entExtent.MinPoint.Y)/2, SystemVariables.LUPREC);

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
            if (!SelectText(out var objectIds, "\n3DS> Select text entities: "))
                return;

            if (!EditorUtils.GetDouble(out double multiplyValue, "\n3DS> Enter value to multiply by text: "))
                return;

            using (var tr = AcadApp.StartTransaction())
            {
                foreach (ObjectId objectId in objectIds)
                {
                    var textEnt = tr.GetObject(objectId, OpenMode.ForRead) as Entity;
                    var text = GetText(textEnt);

                    if (text.IsNumeric())
                    {
                        textEnt.UpgradeOpen();
                        var mathValue = Convert.ToDouble(text) * multiplyValue;
                        var canUpdate = UpdateText(textEnt, mathValue.ToString()); //BUG: Possible culture issue.
                        textEnt.DowngradeOpen();
                    }
                    else
                    {
                        var entExtent = textEnt.GeometricExtents;
                        var midpointX = Math.Round((entExtent.MaxPoint.X+entExtent.MinPoint.X)/2, SystemVariables.LUPREC);
                        var midpointY = Math.Round((entExtent.MaxPoint.Y+entExtent.MinPoint.Y)/2, SystemVariables.LUPREC);

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
            if (!SelectText(out var objectIds, "\n3DS> Select text entities: "))
                return;

            if (!EditorUtils.GetDouble(out double divideValue, "\n3DS> Enter value to divide by text: "))
                return;

            using (var tr = AcadApp.StartTransaction())
            {
                foreach (ObjectId objectId in objectIds)
                {
                    var textEnt = tr.GetObject(objectId, OpenMode.ForRead) as Entity;
                    var text = GetText(textEnt);

                    if (text.IsNumeric())
                    {
                        textEnt.UpgradeOpen();
                        var mathValue = Convert.ToDouble(text) / divideValue;
                        var canUpdate = UpdateText(textEnt, mathValue.ToString()); //BUG: Possible culture issue.
                        textEnt.DowngradeOpen();
                    }
                    else
                    {
                        var entExtent = textEnt.GeometricExtents;
                        var midpointX = Math.Round((entExtent.MaxPoint.X+entExtent.MinPoint.X)/2, SystemVariables.LUPREC);
                        var midpointY = Math.Round((entExtent.MaxPoint.Y+entExtent.MinPoint.Y)/2, SystemVariables.LUPREC);

                        // ignoring text at...
                        AcadApp.WriteMessage($"Ignoring text at X:{midpointX} Y:{midpointY}. Not a number.");
                    }
                }
                tr.Commit();
            }
        }
    }
}
