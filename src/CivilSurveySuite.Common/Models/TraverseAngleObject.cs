using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace CivilSurveySuite.Common.Models
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

        public static TraverseAngleObject FromTraverseObject(TraverseObject traverseObject)
        {
            return new TraverseAngleObject(traverseObject.Bearing, traverseObject.Distance);
        }

        public static IEnumerable<TraverseAngleObject> FromTraverseObjects(IEnumerable<TraverseObject> traverseObjects)
        {
            return traverseObjects.Select(FromTraverseObject);
        }
    }
}