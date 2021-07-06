using _3DS_CivilSurveySuite_C3DBase21;
using Autodesk.AutoCAD.Runtime;

[assembly: CommandClass(typeof(PointUtils))]
namespace _3DS_CivilSurveySuite_C3DBase21
{
    class PointUtils
    {
        public void CreatePointAtOffsetTwoLines()
        { }

        public void CreatePointOnProduction()
        { }

        /// <summary>
        /// Add multiple points (with interpolated elevation) between two points.
        /// </summary>
        void CreatePointBetweenPoints()
        { }

        /// <summary>
        /// Add multiple points that are offsets of a line defined by two points.
        /// </summary>
        void CreatePointBetweenPointsAtOffset()
        { }

        /// <summary>
        /// Add a point at a picked location with elevation calculated at designated slope.
        /// </summary>
        void CreatePointAtLocationWithSlope()
        { }

        /// <summary>
        /// Create point at extension distance on grade between 2 points.
        /// </summary>
        void CreatePointAtExtensionBetweenPoints()
        { }

        /// <summary>
        /// Intersection of Bearing-Bearing, Bearing-Distance, Distance-Distance, 4 Points.
        /// </summary>
        void CreatePointAtIntersectionOf()
        { }

        /// <summary>
        /// Inverses between points (pick or number), echoes coordinates, 
        /// azimuths, bearings, horz/vert distance and slope.
        /// </summary>
        void InverseBetweenPoints()
        { }

        /// <summary>
        /// The CreatePointsFromLabels command can be used to create Civil-3D Points at the 
        /// exact location and elevation of Surface Elevation Labels found in a drawing.
        /// </summary>
        void CreatePointAtLabel()
        { }

        /// <summary>
        /// The UsedPt command displays a list of used point numbers in the command window.
        /// Usage
        /// Type UsedPt at the command line.The available point numbers in the drawing are displayed in the 
        /// command window, as in the following example:
        /// </summary>
        void UsedPoints()
        { }

        /// <summary>
        /// The ZoomPt command zooms the display to the specified point number.
        /// Usage
        /// Type ZoomPt at the command line.You will be prompted to enter either the Number or Name of the Cogo Point to zoom to.
        /// You may also hit ENTER without typing anything to enter a new height factor.The zoom height is determined by taking the current Annotation Scale and
        /// multiplying it by this number.Enter a lower number for the zoom height factor to zoom in closer to the point, or a higher number to zoom out further.
        /// The default zoom height factor is 4.
        /// </summary>
        void ZoomPoint()
        { }


    }
}
