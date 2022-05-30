// Copyright Scott Whitney. All Rights Reserved.
// Reproduction or transmission in whole or in part, any form or by any
// means, electronic, mechanical or otherwise, is prohibited without the
// prior written consent of the copyright owner.

using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace _3DS_CivilSurveySuite.Shared.Models
{
    public class TraverseObject : INotifyPropertyChanged
    {
        private double _bearing;
        private double _distance;
        private Angle _angle;
        private int _index;

        public Angle Angle
        {
            [DebuggerStepThrough]
            get => _angle;
            [DebuggerStepThrough]
            protected set
            {
                _angle = value;
                NotifyPropertyChanged();
            }
        }

        public int Index
        {
            [DebuggerStepThrough]
            get => _index;
            [DebuggerStepThrough]
            set
            {
                _index = value;
                NotifyPropertyChanged();
            }
        }

        public double Bearing
        {
            [DebuggerStepThrough]
            get => _bearing;
            [DebuggerStepThrough]
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
            [DebuggerStepThrough]
            get => _distance;
            [DebuggerStepThrough]
            set
            {
                _distance = value;
                NotifyPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public TraverseObject()
        {
            Angle = new Angle();
            Bearing = 0;
        }

        public TraverseObject(double bearing, double distance)
        {
            Bearing = bearing;
            Distance = distance;
        }

        protected void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Converts a CSV string to a TraverseObject
        /// </summary>
        /// <param name="csvString">The CSV string containing the data.</param>
        /// <param name="delimiter">The delimiter.</param>
        /// <returns>A <see cref="TraverseObject"/>.</returns>
        public static TraverseObject FromCsv(string csvString, char delimiter = ',')
        {
            var data = csvString.Split(delimiter);

            var traverseObject = new TraverseObject();
            traverseObject.Index = int.Parse(data[0]);
            traverseObject.Bearing = double.Parse(data[1]);
            traverseObject.Distance = double.Parse(data[2]);

            return traverseObject;
        }

        /// <summary>
        /// Converts a <see cref="TraverseObject"/> to a CSV string.
        /// </summary>
        /// <param name="delimiter">The delimiter.</param>
        /// <returns>A string representing the <see cref="TraverseObject"/>.</returns>
        public string ToCsv(char delimiter = ',')
        {
            return $"{Index}{delimiter}{Bearing}{delimiter}{Distance}";
        }
    }
}