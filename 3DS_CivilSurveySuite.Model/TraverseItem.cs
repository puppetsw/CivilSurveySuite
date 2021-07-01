// Copyright Scott Whitney. All Rights Reserved.
// Reproduction or transmission in whole or in part, any form or by any
// means, electronic, mechanical or otherwise, is prohibited without the
// prior written consent of the copyright owner.
// 
// Filename: TraverseItem.cs
// Date:     01/07/2021
// Author:   scott

using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace _3DS_CivilSurveySuite.Model
{
    public class TraverseItem : INotifyPropertyChanged
    {
        private double _bearing;
        private double _distance;
        private DMS _dms;
        private int _index;

        public DMS DMSBearing
        {
            get => _dms;
            private set
            {
                _dms = value;
                NotifyPropertyChanged();
            }
        }

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
                if (DMS.IsValid(value))
                {
                    _bearing = value;
                    DMSBearing = new DMS(value);
                }
                else
                {
                    _bearing = 0;
                    DMSBearing = new DMS();
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

        public event PropertyChangedEventHandler PropertyChanged;

        public TraverseItem()
        {
            DMSBearing = new DMS();
            Bearing = 0;
        }

        public TraverseItem(double bearing, double distance)
        {
            DMSBearing = new DMS(bearing);
            Distance = distance;
        }

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}