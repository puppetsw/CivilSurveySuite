using System;

namespace _3DS_CivilSurveySuite.Traverse
{
    public class TraverseBase
    {
        /// <summary>
        /// Degrees, minutes and seconds object
        /// </summary>
        public struct DMS
        {
            public int Degrees;
            public int Minutes;
            public int Seconds;
        }

        public static DMS BearingAddition(double bearing1, double bearing2)
        {
            var dms1 = ParseBearing(bearing1);
            var dms2 = ParseBearing(bearing2);

            var degrees = dms1.Degrees + dms2.Degrees;
            var minutes = dms1.Minutes + dms2.Minutes;
            var seconds = dms1.Seconds + dms2.Seconds;

            //work out seconds first, carry over to minutes
            if (seconds > 60)
            {
                seconds -= 60;
                minutes++;
            }

            //work out minutes, carry over to degrees
            if (minutes > 60)
            {
                minutes -= 60;
                degrees++;
            }

            return new DMS() { Degrees = degrees, Minutes = minutes, Seconds = seconds };
        }

        public static DMS BearingSubtraction(double bearing1, double bearing2)
        {
            var dms1 = ParseBearing(bearing1);
            var dms2 = ParseBearing(bearing2);

            var degrees = dms1.Degrees - dms2.Degrees;
            var minutes = dms1.Minutes - dms2.Minutes;
            var seconds = dms1.Seconds - dms2.Seconds;

            //work out seconds first, carry over to minutes
            if (dms1.Seconds < dms2.Seconds)
            {
                minutes--;
                seconds += 60;
            }

            //work out minutes, carry over to degrees
            if (dms1.Minutes < dms2.Minutes)
            {
                degrees--;
                minutes += 60;
            }
            return new DMS() { Degrees = degrees, Minutes = minutes, Seconds = seconds };
        }

        /// <summary>
        /// Converts a bearing of degrees, minutes, seconds in decimal format to a <see cref="DMS"/> object
        /// </summary>
        /// <param name="bearing">bearing in degrees, minutes and seconds. 354.5020 (354 degrees, 50 minutes, 20 seconds)</param>
        /// <returns><see cref="DMS"/>object containing the parsed values</returns>
        private static DMS ParseBearing(double bearing)
        {
            var degrees = Convert.ToInt32(Math.Truncate(bearing));
            var minutes = Convert.ToInt32((bearing - degrees) * 100);
            var seconds = Convert.ToInt32((((bearing - degrees) * 100) - minutes) * 100);
            return new DMS() { Degrees = degrees, Minutes = minutes, Seconds = seconds };
        }

        /// <summary>
        /// Returns true if the DMS object contains a valid bearing
        /// </summary>
        /// <param name="dMS"><see cref="DMS"/> object to check</param>
        /// <returns></returns>
        public static bool IsValid(DMS dMS)
        {
            return dMS.Degrees < 360 && dMS.Minutes < 60 && dMS.Seconds < 60;
        }

        /// <summary>
        /// Returns true if the DMS object contains a valid bearing
        /// </summary>
        /// <param name="bearing"></param>
        /// <returns></returns>
        public static bool IsValid(double bearing)
        {
            var dMS = ParseBearing(bearing);
            return dMS.Degrees < 360 && dMS.Minutes < 60 && dMS.Seconds < 60;
        }

        /// <summary>
        /// Converts link to meters
        /// </summary>
        /// <param name="link"></param>
        /// <returns></returns>
        public static double ConvertLinkToMeters(double link)
        {
            const double linkConversion = 0.201168;

            return link * linkConversion;
        }

        /// <summary>
        /// Converts feet and inches to meters
        /// </summary>
        /// <param name="feetAndInches">
        /// Feet and inches represented as decimal. 5feet 2inch 5.02.
        /// Inches less than 10 must have a preceeding 0. 
        /// </param>
        /// <returns></returns>
        public static double ConvertFeetToMeters(double feetAndInches)
        {
            const double feetConversion = 0.3048;
            const double inchConversion = 0.0254;

            var feet = Math.Truncate(feetAndInches) * feetConversion;
            var inch1 = feetAndInches - Math.Truncate(feetAndInches);
            var inch2 = (inch1 * 100) * inchConversion;

            return feet + inch2;
        }
    }
}
