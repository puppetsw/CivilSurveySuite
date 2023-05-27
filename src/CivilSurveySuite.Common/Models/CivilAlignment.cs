// Copyright Scott Whitney. All Rights Reserved.
// Reproduction or transmission in whole or in part, any form or by any
// means, electronic, mechanical or otherwise, is prohibited without the
// prior written consent of the copyright owner.

using System;
using System.Diagnostics;

namespace CivilSurveySuite.Shared.Models
{
    public sealed class CivilAlignment : CivilObject, IEquatable<CivilAlignment>
    {
        private double _stationStart;
        private double _stationEnd;
        private string _siteName;

        public double StationStart
        {
            [DebuggerStepThrough]
            get => _stationStart;
            [DebuggerStepThrough]
            set
            {
                _stationStart = value;
                NotifyPropertyChanged();
            }
        }

        public double StationEnd
        {
            [DebuggerStepThrough]
            get => _stationEnd;
            [DebuggerStepThrough]
            set
            {
                _stationEnd = value;
                NotifyPropertyChanged();
            }
        }

        public string SiteName
        {
            [DebuggerStepThrough]
            get => _siteName;
            [DebuggerStepThrough]
            set
            {
                _siteName = value;
                NotifyPropertyChanged();
            }
        }

        public bool Equals(CivilAlignment other)
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
            return obj is CivilAlignment item && Equals(item);
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