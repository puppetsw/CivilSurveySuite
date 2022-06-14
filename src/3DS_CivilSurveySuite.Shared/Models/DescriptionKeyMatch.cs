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
        public Dictionary<string, List<JoinablePoint>> JoinablePoints { get; }

        public DescriptionKeyMatch(DescriptionKey descriptionKey)
        {
            DescriptionKey = descriptionKey;
            JoinablePoints = new Dictionary<string, List<JoinablePoint>>();
        }

        /// <summary>
        /// Builds the Regex match pattern for each <see cref="DescriptionKey"/>.
        /// </summary>
        /// <param name="descriptionKey">The description key.</param>
        /// <param name="matchSpecial"></param>
        /// <remarks>This pattern should detect when there is a whitespace between the code and the number.</remarks>
        /// <returns>A string containing the regex pattern using the given <see cref="DescriptionKey"/>.</returns>
        private static string BuildPattern(DescriptionKey descriptionKey, bool matchSpecial = false)
        {
            if (matchSpecial)
            {
                return "^(" + descriptionKey.Key.Replace("#", ")(\\s?\\d\\d?\\d?)(\\.\\w\\w?\\w?\\w?)?").Replace("*", ".*?");
            }

            return "^(" + descriptionKey.Key.Replace("#", ")(\\s?\\d\\d?\\d?)").Replace("*", ".*?");
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
        /// Gets the special code if one exists in the RawDescription.
        /// </summary>
        /// <param name="rawDescription"></param>
        /// <param name="descriptionKey"></param>
        /// <returns></returns>
        public static string SpecialCode(string rawDescription, DescriptionKey descriptionKey)
        {
            Match regMatch = Regex.Match(rawDescription.ToUpperInvariant(), BuildPattern(descriptionKey, true));
            if (!regMatch.Success)
            {
                return string.Empty;
            }

            var match = regMatch.Groups[3].Value;
            return match;
        }

        /// <summary>
        /// Returns true if the CogoPoint's RawDescription is a match to the <see cref="DescriptionKey"/>.
        /// </summary>
        /// <param name="rawDescription">The raw description from the CogoPoint.</param>
        /// <param name="descriptionKey">The <see cref="DescriptionKey"/> to match against.</param>
        /// <param name="logger">Optional logger.</param>
        /// <returns></returns>
        public static bool IsMatch(string rawDescription, DescriptionKey descriptionKey, ILogger logger = null)
        {
            var matchPattern = BuildPattern(descriptionKey, true);
            Match regMatch = Regex.Match(rawDescription.ToUpperInvariant(), matchPattern);

            if (regMatch.Success)
            {
                logger?.Info($"KEY MATCH! Pattern={matchPattern}, RawDes={rawDescription}");
            }

            return regMatch.Success;
        }

        /// <summary>
        /// Adds the <paramref name="civilPoint"/> to the <see cref="JoinablePoints"/>
        /// </summary>
        /// <param name="civilPoint"></param>
        /// <param name="lineNumber"></param>
        /// <param name="specialCode"></param>
        /// <remarks>
        /// Checks if the <see cref="DescriptionKeyMatch.JoinablePoints"/> contains the current line number
        /// for the point. If it does, add the current point to that dictionary using the key
        /// else, create a new list of points and add it using the key.
        /// </remarks>
        public void AddCogoPoint(CivilPoint civilPoint, string lineNumber, string specialCode, ILogger logger = null)
        {
            var joinablePoint = new JoinablePoint(civilPoint, specialCode);

            if (JoinablePoints.ContainsKey(lineNumber))
            {
                JoinablePoints[lineNumber].Add(joinablePoint);
                logger?.Info($"Adding to existing list {joinablePoint.CivilPoint.RawDescription}");
            }
            else
            {
                var cogoPoints = new List<JoinablePoint> { joinablePoint };
                JoinablePoints.Add(lineNumber, cogoPoints);
                logger?.Info($"Creating new list {joinablePoint.CivilPoint.RawDescription}");
            }
        }
    }
}