﻿using System;

namespace CivilSurveySuite.Common.Models
{
    public sealed class CivilPointGroup : CivilObject, IEquatable<CivilPointGroup>
    {
        public bool Equals(CivilPointGroup other)
        {
            if (ReferenceEquals(null, other))
                return false;

            if (ReferenceEquals(this, other))
                return true;

            return Name == other.Name
                   && Description == other.Description
                   && ObjectId == other.ObjectId
                   && IsSelected == other.IsSelected;
        }

        public override bool Equals(object obj)
        {
            return obj is CivilPointGroup item && Equals(item);
        }

        public override int GetHashCode()
        {
            unchecked // Overflow is fine, just wrap
            {
                var hash = 17;
                hash = hash * 23 + (Name == null ? 0 : Name.GetHashCode());
                hash = hash * 23 + (Description == null ? 0 : Description.GetHashCode());
                hash = hash * 23 + (ObjectId == null ? 0 : ObjectId.GetHashCode());
                hash = hash * 23 + IsSelected.GetHashCode();
                return hash;
            }
        }
    }
}