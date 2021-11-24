// Copyright Scott Whitney. All Rights Reserved.
// Reproduction or transmission in whole or in part, any form or by any
// means, electronic, mechanical or otherwise, is prohibited without the
// prior written consent of the copyright owner.

using System;

namespace _3DS_CivilSurveySuite.Model
{
    public sealed class CivilSite : CivilObject, IEquatable<CivilSite>
    {
        public bool Equals(CivilSite other)
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
            return obj is CivilSite item && Equals(item);
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