using System;

namespace _3DS_CivilSurveySuite.Model
{
    public class DMS : IEquatable<DMS>
    {
        public int Degrees { get; set; }
        public int Minutes { get; set; }
        public int Seconds { get; set; }

        public DMS()
        {
            Degrees = 0;
            Minutes = 0;
            Seconds = 0;
        }

        public DMS(double bearing)
        {
            var dms = Parse(bearing);

            Degrees = dms.Degrees;
            Minutes = dms.Minutes;
            Seconds = dms.Seconds;
        }

        public DMS(string bearing)
        {
            double b = Convert.ToDouble(bearing);
            var dms = Parse(b);

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
            try
            {
                var degrees = Convert.ToInt32(Math.Truncate(bearing));
                var minutes = Convert.ToInt32((bearing - degrees) * 100);
                var seconds = Convert.ToInt32((((bearing - degrees) * 100) - minutes) * 100);
                return new DMS() { Degrees = degrees, Minutes = minutes, Seconds = seconds };
            }
            catch (Exception)
            {
                throw new Exception("Error parsing bearing");
            }
        }

        /// <summary>
        /// Returns true if the <see cref="DMS"/> object contains a valid bearing
        /// </summary>
        /// <param name="dms"><see cref="DMS"/> object to check</param>
        public static bool IsValid(DMS dms)
        {
            return dms.Degrees < 360 && dms.Minutes < 60 && dms.Seconds < 60;
        }

        /// <summary>
        /// Returns true if the <see cref="DMS"/> object contains a valid bearing
        /// </summary>
        /// <param name="bearing"></param>
        public static bool IsValid(double bearing)
        {
            var dms = Parse(bearing);
            return IsValid(dms);
        }

        public static DMS operator +(DMS dms1, DMS dms2)
        {
            var degrees = dms1.Degrees + dms2.Degrees;
            var minutes = dms1.Minutes + dms2.Minutes;
            var seconds = dms1.Seconds + dms2.Seconds;

            //work out seconds first, carry over to minutes
            if (seconds >= 60)
            {
                seconds -= 60;
                minutes++;
            }

            //work out minutes, carry over to degrees
            if (minutes >= 60)
            {
                minutes -= 60;
                degrees++;
            }

            if (degrees > 360)
            {
                degrees -= 360;
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

            if (degrees <= 0)
                degrees += 360;

            return new DMS() { Degrees = degrees, Minutes = minutes, Seconds = seconds };
        }

        public override string ToString()
        {
            string mins, secs;

            if (Minutes < 10)
            {
                mins = "0" + Minutes; //add the 0 in front if its less than 10.
            }
            else
            {
                mins = Minutes.ToString();
            }

            if (Seconds < 10)
            {
                secs = "0" + Seconds; //add the 0 in front if its less than 10.
            }
            else
            {
                secs = Seconds.ToString();
            }

            return string.Format(Degrees + "°" + mins + "'" + secs + '"');
        }

        public double ToDouble()
        {
            var bearing = Degrees + ((double)Minutes / 100) + ((double)Seconds / 10000);
            return bearing;
        }

        public static DMS Add(DMS dms1, DMS dms2)
        {
            var degrees = dms1.Degrees + dms2.Degrees;
            var minutes = dms1.Minutes + dms2.Minutes;
            var seconds = dms1.Seconds + dms2.Seconds;

            //work out seconds first, carry over to minutes
            if (seconds >= 60)
            {
                seconds -= 60;
                minutes++;
            }

            //work out minutes, carry over to degrees
            if (minutes >= 60)
            {
                minutes -= 60;
                degrees++;
            }

            return new DMS() { Degrees = degrees, Minutes = minutes, Seconds = seconds };
        }

        public static DMS Subtract(DMS dms1, DMS dms2)
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

        public bool Equals(DMS other)
        {
            return (this.Degrees == other.Degrees)
                && (this.Minutes == other.Minutes)
                && (this.Seconds == other.Seconds);
        }

        public override bool Equals(object other)
        {
            return other is DMS dMS && Equals(dMS);
        }

        public override int GetHashCode()
        {
            return Degrees.GetHashCode() ^ Minutes.GetHashCode() ^ Seconds.GetHashCode();
        }
    }
}
