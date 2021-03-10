using _3DS_CivilSurveySuite.Helpers;
using _3DS_CivilSurveySuite.Models;
using System;
using System.Globalization;
using System.Windows.Data;

namespace _3DS_CivilSurveySuite.Converters
{
    public class BearingConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var bearing = (double)value;
            var dms = new DMS(bearing);
            return dms.ToString();
            //return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            //350°50'10"; //HACK: please fix all this
            // look for + and - symbols first
            var bearing = (string)value;
            //var nows = StringHelpers.RemoveWhitespace(bearing);
            //var noaplha = StringHelpers.RemoveAlphaCharacters(nows);

            //Regex regex = new Regex(@"([\d.]+)(?:[\+\-\*\/])([\d.]+)");

            //if (regex.IsMatch(noaplha))
            //{
            //    var regString = regex.Split(noaplha);
            //}

            //return value;

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
            var dms = new DMS(StringHelpers.StripDMSSymbols(bearing));
            return dms.ToDouble();
        }
    }
}
