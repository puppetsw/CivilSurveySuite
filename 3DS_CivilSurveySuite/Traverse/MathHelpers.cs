using System;

namespace _3DS_CivilSurveySuite.Traverse
{
    public class MathHelpers
    {
        /// <summary>
        /// Converts link to meters
        /// </summary>
        /// <param name="link"></param>
        /// <returns></returns>
        public static double ConvertLinkToMeters(double link)
        {
            const double linkConversion = 0.201168;

            return Math.Round(link * linkConversion, 4);
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

            return Math.Round(feet + inch2, 4);
        }
    }
}
