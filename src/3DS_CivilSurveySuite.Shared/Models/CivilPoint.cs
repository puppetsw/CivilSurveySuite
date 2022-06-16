// Copyright Scott Whitney. All Rights Reserved.
// Reproduction or transmission in whole or in part, any form or by any
// means, electronic, mechanical or otherwise, is prohibited without the
// prior written consent of the copyright owner.

using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace _3DS_CivilSurveySuite.Shared.Models
{
    /// <summary>
    /// Abstraction class for Civil 3D's CogoPoints.
    /// </summary>
    public sealed class CivilPoint : INotifyPropertyChanged, IEquatable<CivilPoint>, ICloneable
    {
        private uint _pointNumber;
        private double _easting;
        private double _northing;
        private double _elevation;
        private string _rawDescription = string.Empty;
        private string _descriptionFormat = string.Empty;
        private string _objectIdHandle;
        private string _pointName = string.Empty;
        private string _fullDescription = string.Empty;

        public uint PointNumber
        {
            get => _pointNumber;
            set
            {
                _pointNumber = value;
                NotifyPropertyChanged();
            }
        }

        public double Easting
        {
            get => _easting;
            set
            {
                _easting = value;
                NotifyPropertyChanged();
            }
        }

        public double Northing
        {
            get => _northing;
            set
            {
                _northing = value;
                NotifyPropertyChanged();
            }
        }

        public double Elevation
        {
            get => _elevation;
            set
            {
                _elevation = value;
                NotifyPropertyChanged();
            }
        }

        public string RawDescription
        {
            get => _rawDescription;
            set
            {
                _rawDescription = value;
                NotifyPropertyChanged();
            }
        }

        public string DescriptionFormat
        {
            get => _descriptionFormat;
            set
            {
                _descriptionFormat = value;
                NotifyPropertyChanged();
            }
        }

        public string FullDescription
        {
            get => _fullDescription;
            set
            {
                _fullDescription = value;
                NotifyPropertyChanged();
            }
        }

        public string ObjectId
        {
            get => _objectIdHandle;
            set
            {
                _objectIdHandle = value;
                NotifyPropertyChanged();
            }
        }

        public string PointName
        {
            get => _pointName;
            set
            {
                _pointName = value;
                NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Raised when a property on this object has a new value.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raises this object's PropertyChanged event.
        /// </summary>
        /// <param name="propertyName">The property that has a new value.</param>
        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public bool Equals(CivilPoint other)
        {
            if (ReferenceEquals(null, other))
                return false;

            if (ReferenceEquals(this, other))
                return true;

            return _pointNumber == other._pointNumber
                   && _easting.Equals(other._easting)
                   && _northing.Equals(other._northing)
                   && _elevation.Equals(other._elevation)
                   && _rawDescription == other._rawDescription
                   && _descriptionFormat == other._descriptionFormat
                   && _fullDescription == other._fullDescription
                   && _objectIdHandle == other._objectIdHandle
                   && _pointName == other._pointName;
        }

        public override bool Equals(object obj)
        {
            return obj is CivilPoint item && Equals(item);
        }

        public override int GetHashCode()
        {
            unchecked // Overflow is fine, just wrap
            {
                var hash = 17;
                hash = hash * 23 + Easting.GetHashCode();
                hash = hash * 23 + Northing.GetHashCode();
                hash = hash * 23 + Elevation.GetHashCode();
                hash = hash * 23 + (RawDescription == null ? 0 : RawDescription.GetHashCode());
                hash = hash * 23 + (DescriptionFormat == null ? 0 : DescriptionFormat.GetHashCode());
                hash = hash * 23 + (FullDescription == null ? 0 : FullDescription.GetHashCode());
                hash = hash * 23 + (ObjectId == null ? 0 : ObjectId.GetHashCode());
                hash = hash * 23 + (PointName == null ? 0 : PointName.GetHashCode());
                return hash;
            }
        }

        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}