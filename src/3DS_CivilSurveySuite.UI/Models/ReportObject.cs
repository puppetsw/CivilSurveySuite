// Copyright Scott Whitney. All Rights Reserved.
// Reproduction or transmission in whole or in part, any form or by any
// means, electronic, mechanical or otherwise, is prohibited without the
// prior written consent of the copyright owner.

using System.Collections.ObjectModel;

namespace _3DS_CivilSurveySuite.UI.Models
{
    public sealed class ReportObject : ObservableObject
    {
        private ObservableCollection<ReportSurfaceObject> _surfacePoints;
        private ObservableCollection<CivilPointGroup> _pointGroups;
        private StationOffset _stationOffset;
        private CivilAlignment _alignment;
        private CivilPoint _point;

        public CivilPoint Point
        {
            get => _point;
            set => SetProperty(ref _point, value);
        }

        public CivilAlignment Alignment
        {
            get => _alignment;
            set => SetProperty(ref _alignment, value);
        }

        public StationOffset StationOffset
        {
            get => _stationOffset;
            set => SetProperty(ref _stationOffset, value);
        }

        public ObservableCollection<CivilPointGroup> PointGroups
        {
            get => _pointGroups;
            set => SetProperty(ref _pointGroups, value);
        }
        public ObservableCollection<ReportSurfaceObject> SurfacePoints
        {
            get => _surfacePoints;
            set => SetProperty(ref _surfacePoints, value);
        }

        public uint PointNumber => Point.PointNumber;

        public double Easting => Point.Easting;

        public double Northing => Point.Northing;

        public double Elevation => Point.Elevation;

        public string RawDescription => Point.RawDescription;

        public double Chainage => StationOffset.Station;

        public double Offset => StationOffset.Offset;

        public string AlignmentName => Alignment.Name;

        public ReportObject(CivilPoint civilPoint)
        {
            Point = civilPoint;
            PointGroups = new ObservableCollection<CivilPointGroup>();
            SurfacePoints = new ObservableCollection<ReportSurfaceObject>();

            NotifyPropertyChanged(nameof(PointNumber));
            NotifyPropertyChanged(nameof(Easting));
            NotifyPropertyChanged(nameof(Northing));
            NotifyPropertyChanged(nameof(Elevation));
            NotifyPropertyChanged(nameof(RawDescription));
            NotifyPropertyChanged(nameof(Chainage));
            NotifyPropertyChanged(nameof(Offset));
            NotifyPropertyChanged(nameof(AlignmentName));
        }
    }
}
