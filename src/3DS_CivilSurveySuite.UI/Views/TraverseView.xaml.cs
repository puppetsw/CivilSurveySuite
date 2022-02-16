using System.Windows;
using _3DS_CivilSurveySuite.UI.ViewModels;

namespace _3DS_CivilSurveySuite.UI.Views
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