// Copyright Scott Whitney. All Rights Reserved.
// Reproduction or transmission in whole or in part, any form or by any
// means, electronic, mechanical or otherwise, is prohibited without the
// prior written consent of the copyright owner.

namespace _3DS_CivilSurveySuite.Model
{
    public struct Point
    {
        public double X { get; private set; }
        public double Y { get; private set; }
        public double Z { get; private set; }

        public static Point Origin => new Point(0, 0, 0);

        public Point(double x, double y)
        {
            X = x;
            Y = y;
            Z = 0;
        }

        public Point(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }
    }
}