using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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
            txtInput.PreviewKeyDown += TxtInput_PreviewKeyDown; 
        }

        private void TxtInput_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Enter:
                    BtnEnter.Command.Execute(null);
                    break;
                case Key.OemPlus:
                    BtnAdd.Command.Execute(null);
                    break;
                case Key.OemMinus:
                    BtnSubtract.Command.Execute(null);
                    break;
            }
        }
    }
}
