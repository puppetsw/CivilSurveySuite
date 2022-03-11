using System.Collections.ObjectModel;
using _3DS_CivilSurveySuite.UI.Helpers;

namespace _3DS_CivilSurveySuite.UI.Models
{
    public class CivilSurfaceProperties : ObservableObject
    {
        private bool _interpolateLevels;
        private double _interpolateMaximumDistance = 0.5;
        private bool _showCutFill;
        private bool _invertCutFill;
        private int _decimalPlaces = 3;

        public bool InterpolateLevels
        {
            get => _interpolateLevels;
            set => SetProperty(ref _interpolateLevels, value);
        }

        public double InterpolateMaximumDistance
        {
            get => _interpolateMaximumDistance;
            set => SetProperty(ref _interpolateMaximumDistance, value);
        }

        public bool ShowCutFill
        {
            get => _showCutFill;
            set => SetProperty(ref _showCutFill, value);
        }

        public bool InvertCutFill
        {
            get => _invertCutFill;
            set => SetProperty(ref _invertCutFill, value);
        }

        public int DecimalPlaces
        {
            get => _decimalPlaces;
            set => SetProperty(ref _decimalPlaces, value);
        }
    }

    public class CivilAlignmentProperties : ObservableObject
    {
        private bool _isStationRounded;
        private int _stationDecimalPlaces = 3;
        private int _offsetDecimalPlaces = 3;
        private double _stationNearestValue = 0.5;

        public bool IsStationRounded
        {
            get => _isStationRounded;
            set => SetProperty(ref _isStationRounded, value);
        }

        public double StationNearestValue
        {
            get => _stationNearestValue;
            set => SetProperty(ref _stationNearestValue, value);
        }

        public int StationDecimalPlaces
        {
            get => _stationDecimalPlaces;
            set => SetProperty(ref _stationDecimalPlaces, value);
        }

        public int OffsetDecimalPlaces
        {
            get => _offsetDecimalPlaces;
            set => SetProperty(ref _offsetDecimalPlaces, value);
        }

        //TODO: Add station format property.
    }

    public class CivilPointGroupProperties : ObservableObject
    {
        private int _decimalPlaces = 3;

        public int DecimalPlaces
        {
            get => _decimalPlaces;
            set => SetProperty(ref _decimalPlaces, value);
        }
    }

    public class ColumnProperties : ObservableObject
    {
        private ObservableCollection<ColumnHeader> _headers;

        public ObservableCollection<ColumnHeader> Headers
        {
            get => _headers;
            private set => SetProperty(ref _headers, value);
        }

        public ColumnProperties()
        {
            Headers = new ObservableCollection<ColumnHeader>();
            LoadDefaultHeaders();
        }

        public void LoadDefaultHeaders()
        {
            Headers.Add(new ColumnHeader { HeaderText = ResourceHelpers.GetLocalisedString("PointNumber"), IsVisible = true, ColumnType = ColumnType.PointNumber});
            Headers.Add(new ColumnHeader { HeaderText = ResourceHelpers.GetLocalisedString("Easting"), IsVisible = true, ColumnType = ColumnType.Easting});
            Headers.Add(new ColumnHeader { HeaderText = ResourceHelpers.GetLocalisedString("Northing"), IsVisible = true, ColumnType = ColumnType.Northing});
            Headers.Add(new ColumnHeader { HeaderText = ResourceHelpers.GetLocalisedString("Elevation"), IsVisible = true, ColumnType = ColumnType.Elevation});
            Headers.Add(new ColumnHeader { HeaderText = ResourceHelpers.GetLocalisedString("RawDescription"), IsVisible = true, ColumnType = ColumnType.RawDescription});
            Headers.Add(new ColumnHeader { HeaderText = ResourceHelpers.GetLocalisedString("FullDescription"), IsVisible = true, ColumnType = ColumnType.FullDescription});
        }

        public void MoveUp(ColumnHeader header)
        {
            if (!Headers.Contains(header))
            {
                return;
            }

            var currentIndex = Headers.IndexOf(header);
            if (currentIndex >= 1)
            {
                Headers.Move(currentIndex, currentIndex - 1);
            }
        }

        public void MoveDown(ColumnHeader header)
        {
            if (!Headers.Contains(header))
            {
                return;
            }

            var currentIndex = Headers.IndexOf(header);
            if (currentIndex < Headers.Count)
            {
                Headers.Move(currentIndex, currentIndex + 1);
            }
        }
    }
}
