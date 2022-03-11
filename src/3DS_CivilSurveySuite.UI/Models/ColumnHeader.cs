using System;

namespace _3DS_CivilSurveySuite.UI.Models
{
    public sealed class ColumnHeader : ObservableObject, IEquatable<ColumnHeader>
    {
        private string _headerText;
        private bool _isVisible;
        private ColumnType _columnType;
        private string _key;

        public string Key
        {
            get => _key;
            set => SetProperty(ref _key, value);
        }

        public string HeaderText
        {
            get => _headerText;
            set => SetProperty(ref _headerText, value);
        }

        public bool IsVisible
        {
            get => _isVisible;
            set => SetProperty(ref _isVisible, value);
        }

        public ColumnType ColumnType
        {
            get => _columnType;
            set => SetProperty(ref _columnType, value);
        }

        public bool Equals(ColumnHeader other)
        {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;

            return _headerText == other._headerText &&
                   _isVisible  == other._isVisible  &&
                   _columnType == other._columnType &&
                   _key        == other._key;
        }

        public override bool Equals(object obj)
        {
            return ReferenceEquals(this, obj) || obj is ColumnHeader other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = (_headerText != null ? _headerText.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ _isVisible.GetHashCode();
                hashCode = (hashCode * 397) ^ (int)_columnType;
                hashCode = (hashCode * 397) ^ (_key != null ? _key.GetHashCode() : 0);
                return hashCode;
            }
        }
    }
}