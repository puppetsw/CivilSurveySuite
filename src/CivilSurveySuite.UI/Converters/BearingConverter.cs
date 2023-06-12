using System;
using System.Globalization;
using System.Windows.Data;
using CivilSurveySuite.Common.Helpers;
using CivilSurveySuite.Common.Models;

namespace CivilSurveySuite.UI.Converters
{
    public class BearingConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string input = (string) value;

            if (input == null)
            {
                return 0;
            }

            char[] charArray = input.ToCharArray();
            charArray = Array.FindAll(charArray, (c => char.IsDigit(c) || c == '-' || c == '+' || c == '.'));
            var str = new string(charArray); //convert character array back to string

            string[] numbersArray = str.Split(new[] { "+", "-" }, StringSplitOptions.RemoveEmptyEntries);

            if (input.Contains("+") && numbersArray.Length > 1)
                return (new Angle(numbersArray[0]) + new Angle(numbersArray[1])).ToDouble();
            if (input.Contains("-") && numbersArray.Length > 1)
                return (new Angle(numbersArray[0]) - new Angle(numbersArray[1])).ToDouble();

            return new Angle(StringHelpers.ExtractDoubleFromString(input)).ToDouble();
        }
    }
}