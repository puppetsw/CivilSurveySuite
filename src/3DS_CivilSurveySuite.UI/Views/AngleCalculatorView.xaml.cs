using System.Windows;
using _3DS_CivilSurveySuite.UI.ViewModels;

namespace _3DS_CivilSurveySuite.UI.Views
{
    /// <summary>
    /// Interaction logic for BearingCalculator.xaml
    /// </summary>
    public partial class AngleCalculatorView : Window
    {
        public AngleCalculatorView(AngleCalculatorViewModel viewModel)
        {
            InitializeComponent();

            DataContext = viewModel;
        }
    }
}