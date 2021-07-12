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
using _3DS_CivilSurveySuite.Model;
using _3DS_CivilSurveySuite.UI.Helpers;

namespace _3DS_CivilSurveySuite.UI.ViewModels
{
    /// <summary>
    /// Class DMSCalculatorViewModel.
    /// Implements the <see cref="ViewModelBase" />
    /// </summary>
    /// <seealso cref="ViewModelBase" />
    public class AngleCalculatorViewModel : ViewModelBase
    {
        private ObservableCollection<Angle> _dmsList;
        private string _inputBearing;

        public ObservableCollection<Angle> DMSList
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

        public AngleCalculatorViewModel()
        {
            DMSList = new ObservableCollection<Angle>();
        }

        private void AddDMSToList()
        {
            //Validate InputBearing
            Angle dmsToAdd = null;

            try
            {
                dmsToAdd = new Angle(InputBearing);
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

            Angle dmsResult;
            // Add the last 2 in list together
            if (string.IsNullOrEmpty(InputBearing) && DMSList.Count > 1)
            {
                var dms1 = DMSList[DMSList.Count - 1];
                var dms2 = DMSList[DMSList.Count - 2];

                dmsResult = Angle.Add(dms1, dms2);

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
                var dms2 = new Angle(InputBearing);

                dmsResult = Angle.Add(dms1, dms2);
                DMSList.Remove(dms1);

                DMSList.Add(dmsResult);
                InputBearing = string.Empty;
            }
        }

        private void SubtractDMS()
        {

        }

        private static Angle FlipPlusMinusSymbolDMS(Angle dms)
        {
            int degrees = dms.Degrees > 0 ? dms.Degrees * -1 : Math.Abs(dms.Degrees);
            return new Angle { Degrees = degrees, Minutes = dms.Minutes, Seconds = dms.Seconds };
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