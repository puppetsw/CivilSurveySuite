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
        private Angle _adjacentAngleDMS;
        private Angle _internalAngleDMS;
        private double _distance;
        private double _internalAngle;
        private double _adjacentAngle;
        private int _index;

        //TODO: Change Internal and Adjacent to bearing only.
        //TODO: Add Enum for Internal and Adjacent angle.

        public int Index
        {
            get => _index;
            set
            {
                _index = value;
                NotifyPropertyChanged();
            }
        }

        public double InternalAngle
        {
            get => _internalAngle;
            set
            {
                if (Angle.IsValid(value))
                {
                    _internalAngle = value;
                    DMSInternalAngle = new Angle(value);
                }
                else
                {
                    _internalAngle = 0;
                    DMSInternalAngle = new Angle();
                }

                if (DMSAdjacentAngle != null && !DMSAdjacentAngle.IsEmpty)
                {
                    _adjacentAngle = 0;
                    _adjacentAngleDMS = new Angle(0);
                    NotifyPropertyChanged(nameof(AdjacentAngle));
                    NotifyPropertyChanged(nameof(DMSAdjacentAngle));
                }

                NotifyPropertyChanged();
            }
        }

        public double AdjacentAngle
        {
            get => _adjacentAngle;
            set
            {
                if (Angle.IsValid(value))
                {
                    _adjacentAngle = value;
                    DMSAdjacentAngle = new Angle(value);
                }
                else
                {
                    _adjacentAngle = 0;
                    DMSAdjacentAngle = new Angle();
                }

                if (DMSInternalAngle != null && !DMSInternalAngle.IsEmpty)
                {
                    _internalAngle = 0;
                    _internalAngleDMS = new Angle(0);
                    NotifyPropertyChanged(nameof(InternalAngle));
                    NotifyPropertyChanged(nameof(DMSInternalAngle));
                }

                NotifyPropertyChanged();
            }
        }

        public Angle DMSInternalAngle
        {
            get => _internalAngleDMS;
            private set
            {
                _internalAngleDMS = value;
                NotifyPropertyChanged();
            }
        }

        public Angle DMSAdjacentAngle
        {
            get => _adjacentAngleDMS;
            private set
            {
                _adjacentAngleDMS = value;
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

        public event PropertyChangedEventHandler PropertyChanged;

        public TraverseAngleObject()
        {
            DMSInternalAngle = new Angle();
            InternalAngle = 0;
            DMSAdjacentAngle = new Angle();
            AdjacentAngle = 0;
            Distance = 0;
        }

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}