using System;
using System.Globalization;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using CivilSurveySuite.Common.Helpers;

namespace CivilSurveySuite.ACAD
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

            if (!EditorUtils.TryGetSelection("\nSelect text entities: ", "\n>3DS Remove text entities: ",
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
            if (!EditorUtils.TryGetString("\nEnter prefix text: ", out string prefixText, allowSpaces: true))
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
            if (!EditorUtils.TryGetString("\nEnter suffix text: ", out string suffixText))
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

            if (!EditorUtils.TryGetDouble("\nEnter amount to add to text: ", out var addValue))
                return;

            if (addValue == null)
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
                        double mathValue = Convert.ToDouble(text) + addValue.Value;
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

            if (!EditorUtils.TryGetDouble("\nEnter amount to subtract from text: ", out var subtractValue))
                return;

            if (subtractValue == null)
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
                        double mathValue = Convert.ToDouble(text) - subtractValue.Value;
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

            if (!EditorUtils.TryGetDouble("\nEnter value to multiply by text: ", out var multiplyValue))
                return;

            if (multiplyValue == null)
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
                        double mathValue = Convert.ToDouble(text) * multiplyValue.Value;
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

            if (!EditorUtils.TryGetDouble("\nEnter value to divide by text: ", out var divideValue))
                return;

            if (divideValue == null)
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
                        double mathValue = Convert.ToDouble(text) / divideValue.Value;
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
        /// Rounds text to the desired decimal place.
        /// </summary>
        public static void RoundTextDecimalPlaces()
        {
            if (!TrySelectText(out var objectIds))
                return;

            if (!EditorUtils.TryGetInt("\nEnter number of decimal places: ", out int decimalPlaces))
                return;

            using (var tr = AcadApp.StartTransaction())
            {
                foreach (ObjectId objectId in objectIds)
                {
                    var textEnt = tr.GetObject(objectId, OpenMode.ForRead) as Entity;

                    if (textEnt == null)
                    {
                        throw new InvalidOperationException("textEnd was null.");
                    }

                    string text = GetText(textEnt);

                    if (text.IsNumeric())
                    {
                        textEnt.UpgradeOpen();
                        double mathValue = Math.Round(Convert.ToDouble(text), decimalPlaces);
                        UpdateText(textEnt, mathValue.ToString("F" + decimalPlaces, CultureInfo.InvariantCulture));
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
        /// Creates a MText entity at the specified location.
        /// </summary>
        /// <param name="tr">Action transaction.</param>
        /// <param name="location">The location to insert the text at.</param>
        /// <param name="textString">The contents of the MText entity.</param>
        /// <param name="textHeight">The height of the text. Defaults to 0.4 units.</param>
        public static void CreateText(Transaction tr, Point3d location, string textString, double textHeight = 0.4)
        {
            var bt = (BlockTable)tr.GetObject(AcadApp.ActiveDatabase.BlockTableId, OpenMode.ForRead);
            var btr = (BlockTableRecord)tr.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite);

            var text = new MText();
            text.SetDatabaseDefaults();
            text.Location = location;
            text.Height = textHeight;
            text.Contents = textString;
            text.Attachment = AttachmentPoint.BottomLeft;
            btr.AppendEntity(text);
            tr.AddNewlyCreatedDBObject(text, true);
        }
    }
}
