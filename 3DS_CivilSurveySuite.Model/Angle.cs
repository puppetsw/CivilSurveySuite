// Copyright Scott Whitney. All Rights Reserved.
// Reproduction or transmission in whole or in part, any form or by any
// means, electronic, mechanical or otherwise, is prohibited without the
// prior written consent of the copyright owner.

using System;
using System.Diagnostics;

namespace _3DS_CivilSurveySuite.Model
{
    /// <summary>
    /// A class for working with surveyors bearings in degrees, minutes, seconds.
    /// Works to Australian standards.
    /// </summary>
    public class Angle : IEquatable<Angle>
    {
        /// <summary>
        /// Gets or sets the degrees.
        /// </summary>
        public int Degrees { get; set; }

        /// <summary>
        /// Gets or sets the minutes.
        /// </summary>
        public int Minutes { get; set; }

        /// <summary>
        /// Gets or sets the seconds.
        /// </summary>
        public int Seconds { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="Angle"/> class.
        /// </summary>
        public Angle()
        {
            Degrees = 0;
            Minutes = 0;
            Seconds = 0;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Angle"/> class.
        /// </summary>
        /// <param name="bearing">
        /// The bearing in degrees, minutes and seconds in decimal format.
        /// </param>
        public Angle(double bearing)
        {
            Angle angle = Parse(bearing);

            Degrees = angle.Degrees;
            Minutes = angle.Minutes;
            Seconds = angle.Seconds;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Angle"/> class.
        /// </summary>
        /// <param name="bearing">The bearing in degrees, minutes and seconds in string format.</param>
        public Angle(string bearing)
        {
            var b = Convert.ToDouble(bearing);
            Angle angle = Parse(b);

            Degrees = angle.Degrees;
            Minutes = angle.Minutes;
            Seconds = angle.Seconds;
        }

        /// <summary>
        /// Converts a bearing of degrees, minutes, seconds in decimal format to a <see cref="Angle"/> object.
        /// </summary>
        /// <param name="bearing">bearing in degrees, minutes and seconds. 354.5020 (354 degrees, 50 minutes, 20 seconds)</param>
        /// <returns><see cref="Angle"/>object containing the parsed values.</returns>
        public static Angle Parse(double bearing)
        {
            try
            {
                var degrees = Convert.ToInt32(Math.Truncate(bearing));
                var minutes = Convert.ToInt32((bearing - degrees) * 100);
                var seconds = Convert.ToInt32((((bearing - degrees) * 100) - minutes) * 100);
                return new Angle { Degrees = degrees, Minutes = minutes, Seconds = seconds };
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message); //TODO: Change to trace
                throw new Exception("Error parsing bearing");
            }
        }

        /// <summary>
        /// Returns true if the <see cref="Angle"/> object contains a valid bearing.
        /// </summary>
        /// <param name="angle"><see cref="Angle"/> object to check.</param>
        public static bool IsValid(Angle angle) => angle.Degrees < 360 && angle.Minutes < 60 && angle.Seconds < 60;

        /// <summary>
        /// Returns true if the <see cref="Angle"/> object contains a valid bearing.
        /// </summary>
        /// <param name="bearing">The bearing represented as a double value.</param>
        public static bool IsValid(double bearing)
        {
            Angle angle = Parse(bearing);
            return IsValid(angle);
        }

        public static Angle operator +(Angle angle1, Angle angle2) => Add(angle1, angle2, true);

        public static Angle operator -(Angle angle1, Angle angle2) => Subtract(angle1, angle2, true);

        ///<inheritdoc/>
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

        /// <summary>
        /// Returns a double value that represents the current object.
        /// </summary>
        /// <returns>A double value that represents the current object.</returns>
        public double ToDouble()
        {
            var bearing = Degrees + ((double) Minutes / 100) + ((double) Seconds / 10000);
            return bearing;
        }

        public static Angle Add(Angle angle1, Angle angle2, bool limit = false)
        {
            int degrees = angle1.Degrees + angle2.Degrees;
            int minutes = angle1.Minutes + angle2.Minutes;
            int seconds = angle1.Seconds + angle2.Seconds;

            // Work out seconds first, carry over to minutes.
            if (seconds >= 60)
            {
                seconds -= 60;
                minutes++;
            }

            // Work out minutes, carry over to degrees.
            if (minutes >= 60)
            {
                minutes -= 60;
                degrees++;
            }

            if (limit && degrees > 360)
            {
                degrees -= 360;
            }

            return new Angle { Degrees = degrees, Minutes = minutes, Seconds = seconds };
        }

        public static Angle Subtract(Angle angle1, Angle angle2, bool limit = false)
        {
            int degrees = angle1.Degrees - angle2.Degrees;
            int minutes = angle1.Minutes - angle2.Minutes;
            int seconds = angle1.Seconds - angle2.Seconds;

            // Work out seconds first, carry over to minutes.
            if (angle1.Seconds < angle2.Seconds)
            {
                minutes--;
                seconds += 60;
            }

            // Work out minutes, carry over to degrees.
            if (angle1.Minutes < angle2.Minutes)
            {
                degrees--;
                minutes += 60;
            }

            if (limit && degrees <= 0)
            {
                degrees += 360;
            }

            return new Angle { Degrees = degrees, Minutes = minutes, Seconds = seconds };
        }

        public bool IsEmpty => Degrees == 0 && Minutes == 0 && Seconds == 0;

        public bool Equals(Angle other)
        {
            if (other == null)
            {
                throw new ArgumentNullException(nameof(other));
            }

            return (this.Degrees == other.Degrees)
                   && (this.Minutes == other.Minutes)
                   && (this.Seconds == other.Seconds);
        }

        public override bool Equals(object other)
        {
            return other is Angle angle && Equals(angle);
        }

        // ReSharper disable NonReadonlyMemberInGetHashCode
        public override int GetHashCode()
        {
            return Degrees.GetHashCode() ^ Minutes.GetHashCode() ^ Seconds.GetHashCode();
        }
    }
}