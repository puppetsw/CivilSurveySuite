using System.Collections.Generic;
using System.Text.RegularExpressions;
using _3DS_CivilSurveySuite.Shared.Services.Interfaces;

namespace _3DS_CivilSurveySuite.Shared.Models
{
    /// <summary>
    /// DescriptionKeyMatch class
    /// </summary>
    public class DescriptionKeyMatch
    {
        public DescriptionKey DescriptionKey { get; }
        public Dictionary<string, List<CivilPoint>> JoinablePoints { get; }

        public DescriptionKeyMatch(DescriptionKey descriptionKey)
        {
            DescriptionKey = descriptionKey;
            JoinablePoints = new Dictionary<string, List<CivilPoint>>();
        }

        /// <summary>
        /// Builds the Regex match pattern for each <see cref="DescriptionKey"/>.
        /// </summary>
        /// <param name="descriptionKey">The description key.</param>
        /// <remarks>This pattern should detect when there is a whitespace between the code and the number.</remarks>
        /// <returns>A string containing the regex pattern using the given <see cref="DescriptionKey"/>.</returns>
        private static string BuildPattern(DescriptionKey descriptionKey)
        {
            return "^(" + descriptionKey.Key.Replace("#", ")(\\s?\\d\\d?\\d?)").Replace("*", ".*?");
        }

        /// <summary>
        /// Gets the line number from the CogoPoint's RawDescription
        /// </summary>
        /// <param name="rawDescription"></param>
        /// <param name="descriptionKey"></param>
        /// <returns></returns>
        public static string LineNumber(string rawDescription, DescriptionKey descriptionKey)
        {
            Match regMatch = Regex.Match(rawDescription.ToUpperInvariant(), BuildPattern(descriptionKey));
            if (!regMatch.Success)
            {
                return string.Empty;
            }

            var match = regMatch.Groups[2].Value;
            return match;
        }

        /// <summary>
        /// Gets the description from the CogoPoint's RawDescription
        /// </summary>
        /// <param name="rawDescription"></param>
        /// <param name="descriptionKey"></param>
        /// <returns></returns>
        public static string Description(string rawDescription, DescriptionKey descriptionKey)
        {
            Match regMatch = Regex.Match(rawDescription.ToUpperInvariant(), BuildPattern(descriptionKey));
            if (!regMatch.Success)
            {
                return string.Empty;
            }

            var match = regMatch.Groups[1].Value;
            return match;
        }

        /// <summary>
        /// Returns true if the CogoPoint's RawDescription is a match to the <see cref="DescriptionKey"/>.
        /// </summary>
        /// <param name="rawDescription"></param>
        /// <param name="descriptionKey"></param>
        /// <returns></returns>
        public static bool IsMatch(string rawDescription, DescriptionKey descriptionKey, ILogger logger = null)
        {
            var matchPattern = BuildPattern(descriptionKey);
            Match regMatch = Regex.Match(rawDescription.ToUpperInvariant(), matchPattern);

            if (regMatch.Success)
            {
                logger?.Info($"MATCH! Pattern: {matchPattern}, RawDes: {rawDescription}, DesKey: {descriptionKey.Key}");
            }

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