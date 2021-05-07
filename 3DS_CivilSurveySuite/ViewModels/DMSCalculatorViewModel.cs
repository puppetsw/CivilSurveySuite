using _3DS_CivilSurveySuite.Helpers.Wpf;
using _3DS_CivilSurveySuite.Model;
using System;
using System.Collections.ObjectModel;
using System.Text;

namespace _3DS_CivilSurveySuite.ViewModels
{
    public class DMSCalculatorViewModel : ViewModelBase
    {
        #region Private Members
        private string _inputBearing;
        #endregion

        #region Properties
        public ObservableCollection<DMS> DMSList { get; set; }

        public string InputBearing { get => _inputBearing; set { _inputBearing = value; NotifyPropertyChanged(); } }
        #endregion

        #region Constructor
        public DMSCalculatorViewModel()
        {
            DMSList = new ObservableCollection<DMS>();
        }
        #endregion

        #region Commands

        public RelayCommand EnterDMSCommand => new RelayCommand((_) => AddDMSToList(), (_) => true);
        public RelayCommand AdditionDMSCommand => new RelayCommand((_) => AddDMS(), (_) => true);
        public RelayCommand SubtractionDMSCommand => new RelayCommand((_) => SubtractDMS(), (_) => true);
        public RelayCommand NumPadCommand => new RelayCommand((buttonPressed) => NumPad(buttonPressed), (buttonPressed) => true);

        #endregion

        #region Command Methods
        private void AddDMSToList()
        {
            //Validate InputBearing
            DMS dmsToAdd;

            try {
                dmsToAdd = new DMS(InputBearing);
            } catch {
                Console.WriteLine("Invalid bearing value entered");
                return;
            }

            DMSList.Add(dmsToAdd);
            InputBearing = string.Empty;
        }

        private void AddDMS()
        {
            //No need to add if the list count is low
            if (DMSList.Count < 1) return;

            DMS dmsResult;
            //add the last 2 in list together
            if (string.IsNullOrEmpty(InputBearing) && DMSList.Count > 1)
            {
                var dms1 = DMSList[DMSList.Count - 1];
                var dms2 = DMSList[DMSList.Count - 2];

                dmsResult = DMS.Add(dms1, dms2);

                DMSList.Remove(dms1);
                DMSList.Remove(dms2);

                DMSList.Add(dmsResult);
                return;
            }

            //if the list has more than one and the inputbearing is not empty.
            //we add the input bearing to the last value in the list
            if (!string.IsNullOrEmpty(InputBearing) && DMSList.Count >= 1)
            {
                var dms1 = DMSList[DMSList.Count - 1];
                var dms2 = new DMS(InputBearing);

                dmsResult = DMS.Add(dms1, dms2);
                DMSList.Remove(dms1);

                DMSList.Add(dmsResult);
                InputBearing = string.Empty;
            }
        }

        private void SubtractDMS()
        { 

        }

        private void NumPad(object buttonPressed)
        {
            var key = (string)buttonPressed;

            var sb = new StringBuilder();
            sb.Append(InputBearing);
            sb.Append(key);

            InputBearing = sb.ToString();
        }

        #endregion
    }
}
