// Copyright Scott Whitney. All Rights Reserved.
// Reproduction or transmission in whole or in part, any form or by any
// means, electronic, mechanical or otherwise, is prohibited without the
// prior written consent of the copyright owner.

using System.Diagnostics;
using System.Globalization;
using _3DS_CivilSurveySuite.UI.Models;

namespace _3DS_CivilSurveySuite.UI.ViewModels
{
    /// <summary>
    /// ViewModel for AngleCalculatorView.
    /// </summary>
    public class AngleCalculatorViewModel : ObservableObject
    {
        private string _display;
        private string _fullExpression;

        private string _lastOperation;
        private string _lastValue;

        private bool _clearDisplay;
        private bool _clearAll;

        public string Display
        {
            [DebuggerStepThrough]
            get => _display;
            [DebuggerStepThrough]
            set => SetProperty(ref _display, value);
        }

        public string FullExpression
        {
            [DebuggerStepThrough]
            get => _fullExpression;
            [DebuggerStepThrough]
            set => SetProperty(ref _fullExpression, value);
        }

        public RelayCommand<string> DigitButtonPressCommand  => new RelayCommand<string>(DigitButtonPress);

        public RelayCommand<string> OperationButtonPressCommand => new RelayCommand<string>(OperationButtonPress);

        public AngleCalculatorViewModel()
        {
            Display = "0";
            _lastValue = "0";
        }

        private void DigitButtonPress(string button)
        {
            Reset();

            switch (button)
            {
                case "C":
                    Display = "0";
                    break;
                case "Del":
                    DeleteButton();
                    break;
                case ".":
                    DecimalButton();
                    break;
                default:
                    if (Display == "0" || _clearDisplay)
                    {
                        Display = button;
                        _clearDisplay = false;
                    }
                    else
                    {
                        Display += button;
                    }
                    break;
            }
        }

        private void DeleteButton()
        {
            if (Display.Length > 0)
                Display = Display.Remove(Display.Length - 1, 1);

            if (Display.Length == 0)
                Display = "0";
        }

        private void DecimalButton()
        {
            if (Display == "0" || _clearDisplay)
            {
                Display = "0.";
                _clearDisplay = false;
            }

            if (!Display.Contains("."))
                Display += ".";
        }

        private void Reset()
        {
            if (!_clearAll)
                return;

            FullExpression = "";
            Display = "";
            _lastOperation = "";
            _lastValue = "0";
            _clearAll = false;
        }

        private void OperationButtonPress(string operation)
        {
            switch (operation)
            {
                case "+":
                case "-":
                case "=":
                    Calculate(operation);
                    break;
            }

            _lastValue = Display;
            _lastOperation = operation;
        }

        private void Calculate(string operation)
        {
            if (_lastOperation == "=")
                return;

            FullExpression += Display + " " + operation;

            var currentDisplay = new Angle(double.Parse(Display));
            var lastValue = new Angle(double.Parse(_lastValue));

            Angle angle = null;

            switch (operation)
            {
                case "+":
                    angle = Angle.Add(currentDisplay, lastValue);
                    break;
                case "-":
                    if (_lastValue != "0")
                        angle = Angle.Subtract(lastValue, currentDisplay);
                    break;
                case "=":
                    if (_lastOperation == "+")
                        angle = Angle.Add(currentDisplay, lastValue);
                    else if (_lastOperation == "-")
                        angle = Angle.Subtract(lastValue, currentDisplay);

                    _clearAll = true;
                    break;
            }
            _clearDisplay = true;

            if (angle == null)
                return;

            Display = angle.ToDouble().ToString(CultureInfo.InvariantCulture);
        }
    }
}