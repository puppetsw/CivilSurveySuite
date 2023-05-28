using System;

namespace CivilSurveySuite.Common.Models
{
    public class Vector
    {
        public readonly double X;
        public readonly double Y;

        public Vector(double x, double y) { X = x; Y = y; }
        public Vector() : this(double.NaN, double.NaN) {}

        public static Vector operator -(Vector v, Vector w)
        {
            return new Vector(v.X - w.X, v.Y - w.Y);
        }

        public static Vector operator +(Vector v, Vector w)
        {
            return new Vector(v.X + w.X, v.Y + w.Y);
        }

        public static double operator *(Vector v, Vector w)
        {
            return v.X * w.X + v.Y * w.Y;
        }

        public static Vector operator *(Vector v, double mult)
        {
            return new Vector(v.X * mult, v.Y * mult);
        }

        public static Vector operator *(double mult, Vector v)
        {
            return new Vector(v.X * mult, v.Y * mult);
        }

        public double Cross(Vector v)
        {
            return X * v.Y - Y * v.X;
        }

        public override bool Equals(object obj)
        {
            var v = (Vector)obj;

            if (v == null)
                return false;

            return IsZero(X - v.X) && IsZero(Y - v.Y);
        }

        public override int GetHashCode()
        {
            unchecked // Overflow is fine, just wrap
            {
                var hash = 17;
                hash = hash * 23 + X.GetHashCode();
                hash = hash * 23 + Y.GetHashCode();
                return hash;
            }
        }

        public static bool IsZero(double d)
        {
            return Math.Abs(d) < double.Epsilon;
        }

        public override string ToString()
        {
            return $"X:{X},Y:{Y}";
        }
    }
}