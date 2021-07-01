﻿using System.Windows.Controls;
using System.Windows.Input;

namespace _3DS_CivilSurveySuite.Views
{
    /// <summary>
    /// Interaction logic for BearingCalculator.xaml
    /// </summary>
    public partial class DMSCalculatorView : UserControl
    {
        public DMSCalculatorView()
        {
            InitializeComponent();
            txtInput.KeyDown += TxtInput_KeyDown;
        }

        private void TxtInput_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Enter:
                    BtnEnter.Command.Execute(null);
                    e.Handled = true;
                    break;
                case Key.Add:
                case Key.OemPlus:
                    BtnAdd.Command.Execute(null);
                    e.Handled = true;
                    break;
                case Key.Subtract:
                case Key.OemMinus:
                    BtnSubtract.Command.Execute(null);
                    e.Handled = true;
                    break;
            }
        }
    }
}