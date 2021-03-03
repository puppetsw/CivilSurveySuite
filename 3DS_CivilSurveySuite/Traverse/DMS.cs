using System;

namespace _3DS_CivilSurveySuite.Traverse
{
    public class DMS
    {
        public int Degrees { get; set; }
        public int Minutes { get; set; }
        public int Seconds { get; set; }

        public DMS() { }
        public DMS(double bearing)
        {
            var dms = Parse(bearing);
            
            Degrees = dms.Degrees;
            Minutes = dms.Minutes;
            Seconds = dms.Seconds;
        }

        /// <summary>
        /// Converts a bearing of degrees, minutes, seconds in decimal format to a <see cref="DMS"/> object
        /// </summary>
        /// <param name="bearing">bearing in degrees, minutes and seconds. 354.5020 (354 degrees, 50 minutes, 20 seconds)</param>
        /// <returns><see cref="DMS"/>object containing the parsed values</returns>
        private static DMS Parse(double bearing)
        {
            var degrees = Convert.ToInt32(Math.Truncate(bearing));
            var minutes = Convert.ToInt32((bearing - degrees) * 100);
            var seconds = Convert.ToInt32((((bearing - degrees) * 100) - minutes) * 100);
            return new DMS() { Degrees = degrees, Minutes = minutes, Seconds = seconds };
        }

        /// <summary>
        /// Check if <see cref="DMS"/> is empty;
        /// </summary>
        /// <returns>True if all values are 0, otherwise false.</returns>
        public bool IsEmpty()
        {
            return Degrees == 0 && Minutes == 0 && Seconds == 0;
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
            var dMS = Parse(bearing);
            return dMS.Degrees < 360 && dMS.Minutes < 60 && dMS.Seconds < 60;
        }

        public static DMS operator +(DMS dms1, DMS dms2)
        {
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

        public static DMS operator -(DMS dms1, DMS dms2)
        {
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
    }
}
