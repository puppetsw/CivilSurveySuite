using System;
using System.Globalization;
using System.Windows.Data;

namespace CivilSurveySuite.UI.Converters
{
    public class NullToBooleanConverter : IValueConverter
    {
        public bool Inverse { get; set; }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Inverse ? value == null : value != null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
