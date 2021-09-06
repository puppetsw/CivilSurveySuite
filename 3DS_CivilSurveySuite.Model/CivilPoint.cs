// Copyright Scott Whitney. All Rights Reserved.
// Reproduction or transmission in whole or in part, any form or by any
// means, electronic, mechanical or otherwise, is prohibited without the
// prior written consent of the copyright owner.

using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace _3DS_CivilSurveySuite.Model
{
    /// <summary>
    /// Abstraction class for Civil 3D's CogoPoints.
    /// </summary>
    public class CivilPoint : INotifyPropertyChanged, IEquatable<CivilPoint>
    {
        private uint _pointNumber;
        private double _easting;
        private double _northing;
        private double _elevation;
        private string _rawDescription = string.Empty;
        private string _descriptionFormat = string.Empty;
        private string _objectIdHandle;
        private string _pointName = string.Empty;

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

        public string ObjectIdHandle
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
        protected void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
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
                   && _objectIdHandle == other._objectIdHandle 
                   && _pointName == other._pointName;
        }

        public override bool Equals(object obj)
        {
            return obj is CivilPoint item && Equals(item);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (int)_pointNumber;
                hashCode = (hashCode * 397) ^ _easting.GetHashCode();
                hashCode = (hashCode * 397) ^ _northing.GetHashCode();
                hashCode = (hashCode * 397) ^ _elevation.GetHashCode();
                hashCode = (hashCode * 397) ^ (_rawDescription != null ? _rawDescription.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (_descriptionFormat != null ? _descriptionFormat.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (_objectIdHandle != null ? _objectIdHandle.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (_pointName != null ? _pointName.GetHashCode() : 0);
                return hashCode;
            }
        }
    }
}
