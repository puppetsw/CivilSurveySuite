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
    }

    public class CivilPointGroupProperties : ObservableObject
    {
        private bool _allowDuplicates;
        private int _decimalPlaces = 3;

        public bool AllowDuplicates
        {
            get => _allowDuplicates;
            set => SetProperty(ref _allowDuplicates, value);
        }

        public int DecimalPlaces
        {
            get => _decimalPlaces;
            set => SetProperty(ref _decimalPlaces, value);
        }
    }

    public class ColumnProperties : ObservableObject
    {
        private bool _showPointNumber = true;
        private bool _showEasting = true;
        private bool _showNorthing = true;
        private bool _showElevation = true;
        private bool _showRawDescription = true;
        private bool _showFullDescription = true;

        public bool ShowPointNumber
        {
            get => _showPointNumber;
            set => SetProperty(ref _showPointNumber, value);
        }

        public bool ShowEasting
        {
            get => _showEasting;
            set => SetProperty(ref _showEasting, value);
        }

        public bool ShowNorthing
        {
            get => _showNorthing;
            set => SetProperty(ref _showNorthing, value);
        }

        public bool ShowElevation
        {
            get => _showElevation;
            set => SetProperty(ref _showElevation, value);
        }

        public bool ShowRawDescription
        {
            get => _showRawDescription;
            set => SetProperty(ref _showRawDescription, value);
        }

        public bool ShowFullDescription
        {
            get => _showFullDescription;
            set => SetProperty(ref _showFullDescription, value);
        }
    }
}
