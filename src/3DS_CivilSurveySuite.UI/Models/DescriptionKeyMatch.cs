using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace _3DS_CivilSurveySuite.UI.Models
{
    /// <summary>
    /// DescriptionKeyMatch class
    /// </summary>
    public class DescriptionKeyMatch
    {
        public DescriptionKey DescriptionKey { get; private set; }
        public Dictionary<string, List<CivilPoint>> JoinablePoints { get; set; }

        public DescriptionKeyMatch(DescriptionKey descriptionKey)
        {
            DescriptionKey = descriptionKey;
            JoinablePoints = new Dictionary<string, List<CivilPoint>>();
        }

        private static string BuildPattern(DescriptionKey descriptionKey)
        {
            return "^(" + descriptionKey.Key.Replace("#", ")(\\d\\d?\\d?)").Replace("*", ".*?");
        }

        /// <summary>
        /// Gets the line number from the CogoPoint's RawDescription
        /// </summary>
        /// <param name="rawDescription"></param>
        /// <param name="descriptionKey"></param>
        /// <returns></returns>
        public static string LineNumber(string rawDescription, DescriptionKey descriptionKey)
        {
            Match regMatch = Regex.Match(rawDescription, BuildPattern(descriptionKey));
            return regMatch.Success ? regMatch.Groups[2].Value : string.Empty;
        }

        /// <summary>
        /// Gets the description from the CogoPoint's RawDescription
        /// </summary>
        /// <param name="rawDescription"></param>
        /// <param name="descriptionKey"></param>
        /// <returns></returns>
        public static string Description(string rawDescription, DescriptionKey descriptionKey)
        {
            Match regMatch = Regex.Match(rawDescription, BuildPattern(descriptionKey));
            return regMatch.Success ? regMatch.Groups[1].Value : string.Empty;
        }

        /// <summary>
        /// Returns true if the CogoPoint's RawDescription is a match to the <see cref="Models.DescriptionKey"/>
        /// </summary>
        /// <param name="rawDescription"></param>
        /// <param name="descriptionKey"></param>
        /// <returns></returns>
        public static bool IsMatch(string rawDescription, DescriptionKey descriptionKey)
        {
            Match regMatch = Regex.Match(rawDescription, BuildPattern(descriptionKey));
            return regMatch.Success;
        }

        /// <summary>
        /// Adds the <paramref name="cogoPoint"/> to the <see cref="JoinablePoints"/>
        /// </summary>
        /// <param name="cogoPoint"></param>
        /// <param name="lineNumber"></param>
        public void AddCogoPoint(CivilPoint cogoPoint, string lineNumber)
        {
            /* check if the DescriptionKeyMatch joinablepoints contains the current linenumber and point
               if it does, add the current point to that dictiionary using the key
               else, create a new list of points and add it using the key.
             */
            if (JoinablePoints.ContainsKey(lineNumber))
            {
                JoinablePoints[lineNumber].Add(cogoPoint);
            }
            else
            {
                var cogoPoints = new List<CivilPoint> { cogoPoint };
                JoinablePoints.Add(lineNumber, cogoPoints);
            }
        }
    }
}