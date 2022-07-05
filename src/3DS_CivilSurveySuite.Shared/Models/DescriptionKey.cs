using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace _3DS_CivilSurveySuite.Shared.Models
{
    public class DescriptionKey : INotifyPropertyChanged
    {
        private string _description;
        private bool _draw2D;
        private bool _draw3D;

        private string _key;
        private string _layer;
        private double _midOrdinate;
        private bool _explodeFeatureLine;

        /// <summary>
        /// Gets the key value.
        /// </summary>
        /// <remarks>Always returns in Uppercase.</remarks>
        public string Key
        {
            get => _key.ToUpperInvariant();
            set
            {
                _key = value;
                NotifyPropertyChanged();
            }
        }

        public string Layer
        {
            get => _layer;
            set
            {
                _layer = value;
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

        public bool Draw2D
        {
            get => _draw2D;
            set
            {
                _draw2D = value;
                NotifyPropertyChanged();
            }
        }

        public bool Draw3D
        {
            get => _draw3D;
            set
            {
                _draw3D = value;
                NotifyPropertyChanged();
            }
        }

        public double MidOrdinate
        {
            get => _midOrdinate;
            set
            {
                _midOrdinate = value;
                NotifyPropertyChanged();
            }
        }

        public bool ExplodeFeatureLine
        {
            get => _explodeFeatureLine;
            set
            {
                _explodeFeatureLine = value;
                NotifyPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}