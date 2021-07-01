// Copyright Scott Whitney. All Rights Reserved.
// Reproduction or transmission in whole or in part, any form or by any
// means, electronic, mechanical or otherwise, is prohibited without the
// prior written consent of the copyright owner.
// 
// Filename: DMSCalculatorViewModel.cs
// Date:     01/07/2021
// Author:   scott

using System;
using System.Collections.ObjectModel;
using System.Text;
using _3DS_CivilSurveySuite.Helpers.Wpf;
using _3DS_CivilSurveySuite.Model;

namespace _3DS_CivilSurveySuite.ViewModels
{
    /// <summary>
    /// Class DMSCalculatorViewModel.
    /// Implements the <see cref="_3DS_CivilSurveySuite.ViewModels.ViewModelBase" />
    /// </summary>
    /// <seealso cref="_3DS_CivilSurveySuite.ViewModels.ViewModelBase" />
    /// TODO Edit XML Comment Template for DMSCalculatorViewModel
    public class DMSCalculatorViewModel : ViewModelBase
    {
        private ObservableCollection<DMS> _dmsList;
        private string _inputBearing;

        public ObservableCollection<DMS> DMSList
        {
            get => _dmsList;
            set
            {
                _dmsList = value; 
                NotifyPropertyChanged();
            }
        }

        public string InputBearing
        {
            get => _inputBearing;
            set
            {
                _inputBearing = value;
                NotifyPropertyChanged();
            }
        }

        public RelayCommand EnterDMSCommand => new RelayCommand(_ => AddDMSToList(), _ => true);
        public RelayCommand AdditionDMSCommand => new RelayCommand(_ => AddDMS(), _ => true);
        public RelayCommand SubtractionDMSCommand => new RelayCommand(_ => SubtractDMS(), _ => true);
        public RelayCommand NumPadCommand => new RelayCommand(NumPad, _ => true);

        public DMSCalculatorViewModel()
        {
            DMSList = new ObservableCollection<DMS>();
        }

        private void AddDMSToList()
        {
            //Validate InputBearing
            DMS dmsToAdd = null;

            try
            {
                dmsToAdd = new DMS(InputBearing);
            }
            catch
            {
                //Console.WriteLine("Invalid bearing value entered");
            }

            if (dmsToAdd != null)
            {
                DMSList.Add(dmsToAdd);
            }

            InputBearing = string.Empty;
        }

        private void AddDMS()
        {
            // No need to add if the list count is less than 1
            if (DMSList.Count < 1)
            {
                return;
            }

            DMS dmsResult;
            // Add the last 2 in list together
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

            // If the list has more than one and the InputBearing is not empty.
            // We add the InputBearing to the last value in the list
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

        private static DMS FlipPlusMinusSymbolDMS(DMS dms)
        {
            var degrees = dms.Degrees > 0 ? dms.Degrees * -1 : Math.Abs(dms.Degrees);
            return new DMS { Degrees = degrees, Minutes = dms.Minutes, Seconds = dms.Seconds };
        }

        private void NumPad(object buttonPressed)
        {
            var key = (string) buttonPressed;

            var sb = new StringBuilder();
            sb.Append(InputBearing);
            sb.Append(key);

            InputBearing = sb.ToString();
        }
    }
}