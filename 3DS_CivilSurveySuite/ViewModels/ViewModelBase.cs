using _3DS_CivilSurveySuite.Helpers.AutoCAD;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace _3DS_CivilSurveySuite.ViewModels
{
    public class ViewModelBase : CivilBase, INotifyPropertyChanged
    {
        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        protected void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
