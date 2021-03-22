using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace _3DS_CivilSurveySuite.Models
{
    public class TraverseItem : INotifyPropertyChanged
    {
        #region Private Members
        private int index;
        private double bearing;
        private double distance;
        private DMS dms;
        #endregion

        #region Properties
        public DMS DMSBearing { get => dms; private set { dms = value; NotifyPropertyChanged(); } }

        public int Index { get => index; set { index = value; NotifyPropertyChanged(); } }

        public double Bearing
        {
            get => bearing;
            set
            {
                if (DMS.IsValid(value))
                {
                    bearing = value;
                    DMSBearing = new DMS(value);
                }
                else
                {
                    bearing = 0;
                    DMSBearing = new DMS();
                }
                NotifyPropertyChanged();
            }
        }
        public double Distance { get => distance; set { distance = value; NotifyPropertyChanged(); } }
        #endregion

        #region Constructors

        public TraverseItem() { DMSBearing = new DMS(); Bearing = 0; }

        public TraverseItem(double bearing, double distance)
        {
            DMSBearing = new DMS(bearing);
            Distance = distance;
        }

        #endregion

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
