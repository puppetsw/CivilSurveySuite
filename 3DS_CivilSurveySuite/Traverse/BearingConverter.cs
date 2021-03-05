using System;
using System.Globalization;
using System.Windows.Data;

namespace _3DS_CivilSurveySuite.Traverse
{
    public class BearingConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var bearing = (double)value;
            var dms = new DMS(bearing);
            return dms.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //350°50'10"; //HACK: please fix all this
            // look for + and - symbols first
            var bearing = (string)value;
            
            if (bearing.Contains("+"))
            {
                var splitString = bearing.Split('+');
                var bearing1 = StringHelpers.StripDMSSymbols(splitString[0]);
                var bearing2 = StringHelpers.StripDMSSymbols(splitString[1]);

                var dms1 = new DMS(bearing1);
                var dms2 = new DMS(bearing2);

                var result = dms1 + dms2;

                return result.ToDouble();
            } 
            else if (bearing.Contains("-"))
            {
                var splitString = bearing.Split('-');
                var bearing1 = StringHelpers.StripDMSSymbols(splitString[0]);
                var bearing2 = StringHelpers.StripDMSSymbols(splitString[1]);

                var dms1 = new DMS(bearing1);
                var dms2 = new DMS(bearing2);
                var result = dms1 - dms2;

                return result.ToDouble();
            }

            var dms = new DMS(bearing);
            return dms.ToDouble();
        }
    }
}
