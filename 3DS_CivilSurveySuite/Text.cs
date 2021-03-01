using Autodesk.AutoCAD.Runtime;
using System;
using System.Collections.Generic;
using System.Text;

[assembly: CommandClass(typeof(_3DS_CivilSurveySuite.Text))]
namespace _3DS_CivilSurveySuite
{
    public class Text
    {
        public void StripAlphaCharactersFromText()
        { }

        public void AddPrefixToText()
        { }

        public void AddSuffixToText()
        { }

        public void AddPointAtTextInsertion()
        { }
        

        public void TextToUpper()
        {

        }

        public void TextToLower() { }
        
        private static bool IsTextNumeric()
        { return false; }




        //Math Commands
        public void AddNumberToText() { }

        public void SubtractNumberFromText() { }

        public void MultiplyTextByNumber() { }

        public void DivideTextByNumber() { }




    }
}
