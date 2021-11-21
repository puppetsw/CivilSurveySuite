// Copyright Scott Whitney. All Rights Reserved.
// Reproduction or transmission in whole or in part, any form or by any
// means, electronic, mechanical or otherwise, is prohibited without the
// prior written consent of the copyright owner.

using System.Globalization;
using _3DS_CivilSurveySuite.Core;
using _3DS_CivilSurveySuite.Model;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;

namespace _3DS_CivilSurveySuite.ACAD2017
{
    /// <summary>
    /// Utilities to assist with drawing on screen graphics.
    /// </summary>
    public static class TransientGraphicsUtils
    {
        /// <summary>
        /// Draws a chainage between two <see cref="Point3d"/> entities.
        /// </summary>
        /// <param name="graphics">The <see cref="TransientGraphics"/> object to draw the chainage.</param>
        /// <param name="firstPoint"></param>
        /// <param name="secondPoint"></param>
        /// <param name="interval"></param>
        /// <param name="textSize"></param>
        public static void DrawChainage(this TransientGraphics graphics, Point3d firstPoint, Point3d secondPoint, int interval = 10, double textSize = 1.0)
        {
            var distance = PointHelpers.GetDistanceBetweenPoints(firstPoint.ToPoint(), secondPoint.ToPoint());
            var angle = AngleHelpers.GetAngleBetweenPoints(firstPoint.ToPoint(), secondPoint.ToPoint());

            var intervalStep = 0;
            while (intervalStep < distance)
            {
                var point = PointHelpers.AngleAndDistanceToPoint(angle, intervalStep, firstPoint.ToPoint());
                graphics.DrawTick(point.ToPoint3d(), angle);
                graphics.DrawText(point.ToPoint3d(), intervalStep.ToString(), textSize, angle, planReadability: false);
                intervalStep += interval;
            }
            //draw last
            graphics.DrawTick(secondPoint, angle);
            graphics.DrawText(secondPoint, distance.ToString(CultureInfo.InvariantCulture), textSize, angle, planReadability: false);
        }

        public static void DrawChainage(this TransientGraphics graphics, Line line) => DrawChainage(graphics, line.StartPoint, line.EndPoint);

        public static void DrawTick(this TransientGraphics graphics, Point3d tickPoint, Angle drawAngle, double tickLength = 1.0)
        {
            var point1 = PointHelpers.AngleAndDistanceToPoint(drawAngle + 90, tickLength, tickPoint.ToPoint());
            var point2 = PointHelpers.AngleAndDistanceToPoint(drawAngle - 90, tickLength, tickPoint.ToPoint());
            graphics.DrawLine(point1.ToPoint3d(), point2.ToPoint3d(), useDashedLine: false);
        }
    }
}