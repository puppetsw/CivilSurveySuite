using _3DS_CivilSurveySuite.Helpers;
using _3DS_CivilSurveySuite.Model;
using System;
using System.Globalization;
using System.Windows.Data;

namespace _3DS_CivilSurveySuite.Converters
{
    public class BearingConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string input = (string)value;
            char[] charArray = input.ToCharArray();
            charArray = Array.FindAll<char>(charArray, (c => (char.IsDigit(c) || c == '-' || c == '+' || c == '.')));
            var str = new string(charArray); //convert character array back to string

            string[] numbersArray = str.Split(new string[] { "+", "-" }, StringSplitOptions.RemoveEmptyEntries);

            if (input.Contains("+") && numbersArray.Length > 1)
                return (new DMS(numbersArray[0]) + new DMS(numbersArray[1])).ToDouble();
            else if (input.Contains("-") && numbersArray.Length > 1)
                return (new DMS(numbersArray[0]) - new DMS(numbersArray[1])).ToDouble();
            else
                return new DMS(StringHelpers.ExtractDoubleFromString(input)).ToDouble();
        }
    }
}
