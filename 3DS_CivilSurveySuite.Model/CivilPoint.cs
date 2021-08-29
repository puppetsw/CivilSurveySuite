// Copyright Scott Whitney. All Rights Reserved.
// Reproduction or transmission in whole or in part, any form or by any
// means, electronic, mechanical or otherwise, is prohibited without the
// prior written consent of the copyright owner.

using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace _3DS_CivilSurveySuite.Model
{
    /// <summary>
    /// Abstraction class for Civil 3D's CogoPoints.
    /// </summary>
    public class CivilPoint : INotifyPropertyChanged
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
    }
}
