using System.Windows;
using CivilSurveySuite.UI.ViewModels;

namespace CivilSurveySuite.UI.Views
{
    /// <summary>
    /// Interaction logic for TraverseAngleView.xaml
    /// </summary>
    public partial class TraverseAngleView : Window
    {
        public TraverseAngleView(TraverseAngleViewModel viewModel)
        {
            InitializeComponent();

            DataContext = viewModel;
        }
    }
}