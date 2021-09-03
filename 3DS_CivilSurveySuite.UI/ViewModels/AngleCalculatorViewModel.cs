// Copyright Scott Whitney. All Rights Reserved.
// Reproduction or transmission in whole or in part, any form or by any
// means, electronic, mechanical or otherwise, is prohibited without the
// prior written consent of the copyright owner.

using _3DS_CivilSurveySuite.Model;

namespace _3DS_CivilSurveySuite.UI.ViewModels
{
    /// <summary>
    /// ViewModel for AngleCalculatorView.
    /// </summary>
    public class AngleCalculatorViewModel : ViewModelBase
    {
        private Angle _firstAngle;
        private Angle _secondAngle;
        private string _result;
        private double _firstBearing;
        private double _secondBearing;

        public Angle FirstAngle
        {
            get => _firstAngle;
            set => SetProperty(ref _firstAngle, value);
        }

        public Angle SecondAngle
        {
            get => _secondAngle;
            set => SetProperty(ref _secondAngle, value);
        }

        public string Result
        {
            get => _result;
            private set => SetProperty(ref _result, value);
        }

        public double FirstBearing
        {
            get => _firstBearing;
            set
            {
                if (Angle.IsValid(value, false)) // Don't limit degrees.
                {
                    _firstBearing = value;
                    FirstAngle = new Angle(value);
                }
                else
                {
                    _firstBearing = 0;
                    FirstAngle = new Angle();
                }

                NotifyPropertyChanged();
            }
        }

        public double SecondBearing
        {
            get => _secondBearing;
            set
            {
                if (Angle.IsValid(value, false)) // Don't limit degrees.
                {
                    _secondBearing = value;
                    SecondAngle = new Angle(value);
                }
                else
                {
                    _secondBearing = 0;
                    SecondAngle = new Angle();
                }

                NotifyPropertyChanged();
            }
        }

        public RelayCommand AddCommand => new RelayCommand(Add, () => true);

        public RelayCommand SubtractCommand => new RelayCommand(Subtract, () => true);

        public AngleCalculatorViewModel()
        {
            FirstAngle = new Angle();
            SecondAngle = new Angle();
            Result = "";
        }

        private void Add()
        {
            if (FirstAngle == null || SecondAngle == null)
                return;

            Result = Angle.Add(FirstAngle, SecondAngle).ToString();
        }

        private void Subtract()
        {
            if (FirstAngle == null || SecondAngle == null)
                return;

            Result = Angle.Subtract(FirstAngle, SecondAngle).ToString();
        }
    }
}