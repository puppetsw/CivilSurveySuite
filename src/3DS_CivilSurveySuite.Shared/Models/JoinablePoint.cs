using System;

namespace _3DS_CivilSurveySuite.Shared.Models
{
    public class JoinablePoint : IEquatable<JoinablePoint>
    {
        public CivilPoint CivilPoint { get; }

        public bool HasSpecialCode => !string.IsNullOrEmpty(SpecialCode);

        public string SpecialCode { get; }

        public JoinablePoint(CivilPoint civilPoint, string specialCode)
        {
            CivilPoint = civilPoint;
            SpecialCode = specialCode;
        }

        public bool Equals(JoinablePoint other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }

            if (ReferenceEquals(this, other))
            {
                return true;
            }

            if (Equals(CivilPoint, other.CivilPoint))
            {
                return SpecialCode == other.SpecialCode;
            }

            return false;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != this.GetType())
            {
                return false;
            }

            return Equals((JoinablePoint)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((CivilPoint != null ? CivilPoint.GetHashCode() : 0) * 397) ^
                       (SpecialCode != null ? SpecialCode.GetHashCode() : 0);
            }
        }
    }
}
