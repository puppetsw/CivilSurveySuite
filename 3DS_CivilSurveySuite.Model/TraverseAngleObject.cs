// Copyright Scott Whitney. All Rights Reserved.
// Reproduction or transmission in whole or in part, any form or by any
// means, electronic, mechanical or otherwise, is prohibited without the
// prior written consent of the copyright owner.

using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace _3DS_CivilSurveySuite.Model
{
    public class TraverseAngleObject : INotifyPropertyChanged
    {
        private Angle _angle;
        private double _distance;
        private double _bearing;
        private int _index;
        private AngleRotationDirection _angleDirection;
        private AngleReferenceDirection _referenceDirection;

        public int Index
        {
            get => _index;
            set
            {
                _index = value;
                NotifyPropertyChanged();
            }
        }

        public double Bearing
        {
            get => _bearing;
            set
            {
                if (Angle.IsValid(value))
                {
                    _bearing = value;
                    Angle = new Angle(value);
                }
                else
                {
                    _bearing = 0;
                    Angle = new Angle();
                }

                NotifyPropertyChanged();
            }
        }

        public double Distance
        {
            get => _distance;
            set
            {
                _distance = value;
                NotifyPropertyChanged();
            }
        }

        public Angle Angle
        {
            get => _angle;
            private set
            {
                _angle = value;
                NotifyPropertyChanged();
            }
        }

        public AngleRotationDirection RotationDirection
        {
            get => _angleDirection;
            set
            {
                _angleDirection = value;
                NotifyPropertyChanged();
            }
        }

        public AngleReferenceDirection ReferenceDirection
        {
            get => _referenceDirection;
            set
            {
                _referenceDirection = value;
                NotifyPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public TraverseAngleObject()
        {
            Angle = new Angle();
            Bearing = 0;
            Distance = 0;
        }

        public TraverseAngleObject(double bearing, double distance)
        {
            Bearing = bearing;
            Distance = distance;
        }

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}