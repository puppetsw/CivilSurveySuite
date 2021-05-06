using _3DS_CivilSurveySuite.Helpers.Wpf;
using _3DS_CivilSurveySuite.Model;
using _3DS_CivilSurveySuite.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace _3DS_CivilSurveySuite.ViewModels
{
    public class DMSCalculatorViewModel : ViewModelBase
    {
        private double _inputBearing;


        public ObservableCollection<DMS> DMSList { get; set; }

        public double InputBearing { get => _inputBearing; set { _inputBearing = value; NotifyPropertyChanged(); } }

        public DMSCalculatorViewModel()
        {
            DMSList = new ObservableCollection<DMS>();
        }


        #region Commands

        public RelayCommand EnterDMSCommand => new RelayCommand((_) => AddDMSToList(), (_) => true);
        public RelayCommand AdditionDMSCommand => new RelayCommand((_) => AddDMS(), (_) => true);
        public RelayCommand SubtractionDMSCommand => new RelayCommand((_) => SubtractDMS(), (_) => true);



        #endregion

        #region Command Methods
        private void AddDMSToList()
        {
            //validate InputBearing
            if (!DMS.IsValid(InputBearing)) return;

            var newDMS = new DMS(InputBearing);
            DMSList.Add(newDMS);

            InputBearing = 0;
        }

        private void AddDMS()
        {
            //No need to add if the count is low
            if (DMSList.Count < 1) return; 


        }

        private void SubtractDMS()
        { 

        }



        #endregion
    }
}
