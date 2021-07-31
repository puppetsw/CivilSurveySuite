using Autodesk.AutoCAD.Geometry;
using Autodesk.Civil.DatabaseServices;

namespace _3DS_CivilSurveySuite.C3D2017.C3DUtils
{
    public class CogoPoints
    {
        public static void CreateCogoPoint(Point3d position)
        {
            CogoPointCollection cogoPoints = C3DApp.ActiveCivilDocument.CogoPoints;
            cogoPoints.Add(position, true);
        }

        /// <summary>
        /// Changes a <see cref="CogoPoint"/>'s Draw Description to upper case.
        /// </summary>
        public static void RawDescriptionToUpperCase(ref CogoPoint point)
        {
            point.RawDescription = point.RawDescription.ToUpper();
        }

        /// <summary>
        /// Changes a <see cref="CogoPoint"/>'s Full Description to upper case.
        /// </summary>
        public static void FullDescriptionToUpperCase(ref CogoPoint point)
        {
            point.DescriptionFormat = point.DescriptionFormat.ToUpper();
        }

        /// <summary>
        /// Creates a point at the offset of 2 lines.
        /// </summary>
        public void CreatePointAtOffsetTwoLines()
        {
        }

        /// <summary>
        /// Creates a point on production of a line.
        /// </summary>
        public void CreatePointOnProduction()
        {
        }

        /// <summary>
        /// Add multiple points (with interpolated elevation) between two points.
        /// </summary>
        private void CreatePointBetweenPoints()
        {
        }

        /// <summary>
        /// Add multiple points that are offsets of a line defined by two points.
        /// </summary>
        private void CreatePointBetweenPointsAtOffset()
        {
        }

        /// <summary>
        /// Add a point at a picked location with elevation calculated at designated slope.
        /// </summary>
        private void CreatePointAtLocationWithSlope()
        {
        }

        /// <summary>
        /// Create point at extension distance on grade between 2 points.
        /// </summary>
        private void CreatePointAtExtensionBetweenPoints()
        {
        }

        /// <summary>
        /// Intersection of Bearing-Bearing, Bearing-Distance, Distance-Distance, 4 Points.
        /// </summary>
        private void CreatePointAtIntersectionOf()
        {
        }

        /// <summary>
        /// Inverses between points (pick or number), echoes coordinates, 
        /// azimuths, bearings, horz/vert distance and slope.
        /// </summary>
        private void InverseBetweenPoints()
        {
        }

        /// <summary>
        /// The CreatePointsFromLabels command can be used to create Civil-3D Points at the 
        /// exact location and elevation of Surface Elevation Labels found in a drawing.
        /// </summary>
        private void CreatePointAtLabel()
        {
        }

        /// <summary>
        /// The UsedPt command displays a list of used point numbers in the command window.
        /// Usage
        /// Type UsedPt at the command line.The available point numbers in the drawing are displayed in the 
        /// command window, as in the following example:
        /// </summary>
        private void UsedPoints()
        {
        }

        /// <summary>
        /// The ZoomPt command zooms the display to the specified point number.
        /// Usage
        /// Type ZoomPt at the command line.You will be prompted to enter either the Number or Name of the Cogo Point to zoom to.
        /// You may also hit ENTER without typing anything to enter a new height factor.The zoom height is determined by taking the current Annotation Scale and
        /// multiplying it by this number.Enter a lower number for the zoom height factor to zoom in closer to the point, or a higher number to zoom out further.
        /// The default zoom height factor is 4.
        /// </summary>
        private void ZoomPoint()
        {
        }
    }
}