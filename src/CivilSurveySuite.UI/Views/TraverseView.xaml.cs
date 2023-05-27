using System.Windows;
using CivilSurveySuite.UI.ViewModels;

namespace CivilSurveySuite.UI.Views
{
    /// <summary>
    /// Interaction logic for TraverseView.xaml
    /// </summary>
    public partial class TraverseView : Window
    {
        public TraverseView(TraverseViewModel viewModel)
        {
            InitializeComponent();

            DataContext = viewModel;
        }
    }
}