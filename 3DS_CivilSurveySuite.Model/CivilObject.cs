// Copyright Scott Whitney. All Rights Reserved.
// Reproduction or transmission in whole or in part, any form or by any
// means, electronic, mechanical or otherwise, is prohibited without the
// prior written consent of the copyright owner.

using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace _3DS_CivilSurveySuite.Model
{
    public abstract class CivilObject : INotifyPropertyChanged, IEquatable<CivilObject>
    {
        private string _name;
        private string _description;
        private string _objectId;
        private bool _isSelected;

        public string ObjectId
        {
            get => _objectId;
            set
            {
                _objectId = value;
                NotifyPropertyChanged();
            }
        }

        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                NotifyPropertyChanged();
            }
        }

        public string Description
        {
            get => _description;
            set
            {
                _description = value;
                NotifyPropertyChanged();
            }
        }

        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                _isSelected = value;
                NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Raised when a property on this object has a new value.
        /// </summary>
        public virtual event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raises this object's PropertyChanged event.
        /// </summary>
        /// <param name="propertyName">The property that has a new value.</param>
        protected void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public bool Equals(CivilObject other)
        {
            if (ReferenceEquals(null, other))
                return false;

            if (ReferenceEquals(this, other))
                return true;

            return _name == other._name
                   && _description == other._description
                   && _objectId == other._objectId;
        }

        public override bool Equals(object obj)
        {
            return obj is CivilObject item && Equals(item);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = _name != null ? _name.GetHashCode() : 0;
                hashCode = (hashCode * 397) ^ (_description != null ? _description.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (_objectId != null ? _objectId.GetHashCode() : 0);
                return hashCode;
            }
        }
    }
}