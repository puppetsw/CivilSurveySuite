// Copyright Scott Whitney. All Rights Reserved.
// Reproduction or transmission in whole or in part, any form or by any
// means, electronic, mechanical or otherwise, is prohibited without the
// prior written consent of the copyright owner.

using System;
using System.Globalization;
using _3DS_CivilSurveySuite.Model;

namespace _3DS_CivilSurveySuite.UI.ViewModels
{
    /// <summary>
    /// ViewModel for AngleCalculatorView.
    /// </summary>
    public class AngleCalculatorViewModel : ViewModelBase
    {
        private string _display;
        private string _fullExpression;

        private string _lastOperation;
        private string _lastValue = "0";

        private bool _clearDisplay;
        private bool _clearAll;

        public string Display
        {
            get => _display;
            set => SetProperty(ref _display, value);
        }

        public string FullExpression
        {
            get => _fullExpression;
            set => SetProperty(ref _fullExpression, value);
        }

        public RelayCommand<string> DigitButtonPressCommand => new RelayCommand<string>(DigitButtonPress);

        public RelayCommand<string> OperationButtonPressCommand => new RelayCommand<string>(OperationButtonPress);

        public AngleCalculatorViewModel()
        {
            Display = "0";
        }

        private void DigitButtonPress(string button)
        {
            if (_clearAll)
            {
                FullExpression = "";
                Display = "";
                _lastOperation = "";
                _lastValue = "0";
                _clearAll = false;
            }

            switch (button)
            {
                case "C":
                    Display = "0";
                    break;
                case "Del":
                    if (Display.Length > 0)
                        Display = Display.Remove(Display.Length - 1, 1);

                    if (Display.Length == 0)
                        Display = "0";
                    break;
                case ".":
                    if (Display == "0" || _clearDisplay)
                    {
                        Display = "0.";
                        _clearDisplay = false;
                    }

                    if (!Display.Contains("."))
                        Display += ".";
                    break;
                default:
                    if (Display == "0" || _clearDisplay)
                    {
                        Display = button;
                        _clearDisplay = false;
                    }
                    else
                        Display += button;
                    break;
            }
        }

        //TODO: This is terrible. Please fix.
        private void OperationButtonPress(string operation)
        {
            var currentDisplay = new Angle(double.Parse(Display));

            switch (operation)
            {
                case "+":
                    FullExpression += Display + " " + operation;
                    var valAdd = new Angle(double.Parse(_lastValue));
                    var angleAdd = Angle.Add(currentDisplay, valAdd);
                    Display = angleAdd.ToDouble().ToString(CultureInfo.InvariantCulture);
                    _clearDisplay = true;
                    break;
                case "-":
                    FullExpression += Display + " " + operation;
                    var valSub = new Angle(double.Parse(_lastValue));
                    var angleSub = Angle.Subtract(currentDisplay, valSub);
                    Display = angleSub.ToDouble().ToString(CultureInfo.InvariantCulture);

                    _clearDisplay = true;
                    break;
                case "=":
                    switch (_lastOperation)
                    {
                        case "+":
                            FullExpression += Display + " " + operation;
                            var valAdd1 = new Angle(double.Parse(_lastValue));
                            var angleAdd1 = Angle.Add(currentDisplay, valAdd1);
                            Display = angleAdd1.ToDouble().ToString(CultureInfo.InvariantCulture);
                            _lastOperation = "=";
                            _clearAll = true;
                            _clearDisplay = true;
                            break;
                        case "-":
                            FullExpression += Display + " " + operation;
                            var valSub1 = new Angle(double.Parse(_lastValue));
                            var angleSub1 = Angle.Subtract(valSub1, currentDisplay);
                            Display = angleSub1.ToDouble().ToString(CultureInfo.InvariantCulture);
                            _lastOperation = "=";
                            _clearAll = true;
                            _clearDisplay = true;
                            break;
                    }
                    break;
                default:
                    throw new InvalidOperationException("Invalid operation.");
            }

            _lastValue = Display;
            _lastOperation = operation;
        }
    }
}