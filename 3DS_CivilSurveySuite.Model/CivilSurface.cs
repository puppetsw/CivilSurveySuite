// Copyright Scott Whitney. All Rights Reserved.
// Reproduction or transmission in whole or in part, any form or by any
// means, electronic, mechanical or otherwise, is prohibited without the
// prior written consent of the copyright owner.

using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace _3DS_CivilSurveySuite.Model
{
    public class CivilSurface : INotifyPropertyChanged, IEquatable<CivilSurface>
    {
        private string _name;
        private string _description;
        private string _objectId;

        public string ObjectId
        {
            get => _objectId;
            set
            {
                _objectId = value;
                OnPropertyChanged();
            }
        }

        public string Name
        {
            get => _name;
            set
            {
                _name = value;
                OnPropertyChanged();
            }
        }

        public string Description
        {
            get => _description;
            set
            {
                _description = value;
                OnPropertyChanged();
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public bool Equals(CivilSurface other)
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
            return obj is CivilSurface item && Equals(item);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = (_name != null ? _name.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (_description != null ? _description.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (_objectId != null ? _objectId.GetHashCode() : 0);
                return hashCode;
            }
        }
    }
}