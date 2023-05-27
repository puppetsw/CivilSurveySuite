using System.Windows;
using CivilSurveySuite.UI.ViewModels;

namespace CivilSurveySuite.UI.Views
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