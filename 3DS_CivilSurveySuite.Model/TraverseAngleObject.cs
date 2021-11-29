// Copyright Scott Whitney. All Rights Reserved.
// Reproduction or transmission in whole or in part, any form or by any
// means, electronic, mechanical or otherwise, is prohibited without the
// prior written consent of the copyright owner.

using System.Diagnostics;

namespace _3DS_CivilSurveySuite.Model
{
    public class TraverseAngleObject : TraverseObject
    {
        private AngleRotationDirection _angleDirection;
        private AngleReferenceDirection _referenceDirection;

        public AngleRotationDirection RotationDirection
        {
            [DebuggerStepThrough]
            get => _angleDirection;
            [DebuggerStepThrough]
            set
            {
                _angleDirection = value;
                NotifyPropertyChanged();
            }
        }

        public AngleReferenceDirection ReferenceDirection
        {
            [DebuggerStepThrough]
            get => _referenceDirection;
            [DebuggerStepThrough]
            set
            {
                _referenceDirection = value;
                NotifyPropertyChanged();
            }
        }

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
    }
}