using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace _3DS_CivilSurveySuite_C3DBase21
{
    public class DescriptionKey : INotifyPropertyChanged
    {
        private string description;
        private bool draw2D;
        private bool draw3D;

        private string key;
        private string layer;

        public string Key
        {
            get => key;
            set
            {
                key = value;
                NotifyPropertyChanged();
            }
        }

        public string Layer
        {
            get => layer;
            set
            {
                layer = value;
                NotifyPropertyChanged();
            }
        }

        public string Description
        {
            get => description;
            set
            {
                description = value;
                NotifyPropertyChanged();
            }
        }

        public bool Draw2D
        {
            get => draw2D;
            set
            {
                draw2D = value;
                NotifyPropertyChanged();
            }
        }

        public bool Draw3D
        {
            get => draw3D;
            set
            {
                draw3D = value;
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