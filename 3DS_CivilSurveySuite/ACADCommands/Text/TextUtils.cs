using Autodesk.AutoCAD.Runtime;

[assembly: CommandClass(typeof(_3DS_CivilSurveySuite.ACADCommands.Text.TextUtils))]
namespace _3DS_CivilSurveySuite.ACADCommands.Text
{
    public class TextUtils
    {
        /// <summary>
        /// Strips any alpha characters from text leaving only a number value (if it contains one)
        /// </summary>
        public void RemoveAlphaCharactersFromText()
        { }

        /// <summary>
        /// Adds a prefix to the selected text objects
        /// </summary>
        public void AddPrefixToText()
        { }

        /// <summary>
        /// Adds a suffix to the selected text objects.
        /// </summary>
        public void AddSuffixToText()
        { }

        /// <summary>
        /// Converts the selected text to Uppercase
        /// </summary>
        public void TextToUpper()
        { }

        /// <summary>
        /// Converts the selected text to lowercase
        /// </summary>
        public void TextToLower() 
        { }

        /// <summary>
        /// Converts the selected text to sentence case
        /// </summary>
        public void TextToSentence()
        { }

        /// <summary>
        /// Checks if the passed text is a numeric value
        /// </summary>
        /// <returns></returns>
        private static bool IsTextNumeric(string text)
        { return false; }

        /// <summary>
        /// Adds a number to a text entity if it is a valid number
        /// </summary>
        public void AddNumberToText() 
        { }

        /// <summary>
        /// Subtracts a number from a text entity if it is a valid number
        /// </summary>
        public void SubtractNumberFromText()
        { }

        /// <summary>
        /// Multiplies the text by a number
        /// </summary>
        public void MultiplyTextByNumber() 
        { }

        /// <summary>
        /// Divides the text by a number
        /// </summary>
        public void DivideTextByNumber() 
        { }
    }
}
