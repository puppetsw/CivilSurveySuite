using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace CivilSurveySuite.Common.Models
{
    public abstract class CivilObject : INotifyPropertyChanged
    {
        private string _name;
        private string _description;
        private string _objectId;
        private bool _isSelected;

        public string ObjectId
        {
            [DebuggerStepThrough]
            get => _objectId;
            [DebuggerStepThrough]
            set
            {
                _objectId = value;
                NotifyPropertyChanged();
            }
        }

        public string Name
        {
            [DebuggerStepThrough]
            get => _name;
            [DebuggerStepThrough]
            set
            {
                _name = value;
                NotifyPropertyChanged();
            }
        }

        public string Description
        {
            [DebuggerStepThrough]
            get => _description;
            [DebuggerStepThrough]
            set
            {
                _description = value;
                NotifyPropertyChanged();
            }
        }

        public bool IsSelected
        {
            [DebuggerStepThrough]
            get => _isSelected;
            [DebuggerStepThrough]
            set
            {
                _isSelected = value;
                NotifyPropertyChanged();
            }
        }

        /// <summary>
        /// Raised when a property on this object has a new value.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raises this object's PropertyChanged event.
        /// </summary>
        /// <param name="propertyName">The property that has a new value.</param>
        [DebuggerStepThrough]
        protected void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}