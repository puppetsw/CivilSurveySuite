using System;
using System.Globalization;
using System.Windows.Data;

using _3DS_CivilSurveySuite.Helpers;

namespace _3DS_CivilSurveySuite.Traverse.Converters
{
    public class DistanceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string distStr = (string)value;

            string[] splitDistance;
            if (distStr.Contains("+"))
            {
                splitDistance = distStr.Split('+');
                double dist1 = StringHelpers.ExtractDoubleFromString(splitDistance[0]);
                double dist2 = StringHelpers.ExtractDoubleFromString(splitDistance[1]);

                return dist1 + dist2;
            }
            else if (distStr.Contains("-"))
            {
                splitDistance = distStr.Split('-');
                double dist1 = StringHelpers.ExtractDoubleFromString(splitDistance[0]);
                double dist2 = StringHelpers.ExtractDoubleFromString(splitDistance[1]);

                return dist1 - dist2;
            }
            return distStr;
        }
    }
}
