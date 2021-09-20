using System.Windows;
using _3DS_CivilSurveySuite.UI.ViewModels;

namespace _3DS_CivilSurveySuite.UI.Views
{
    /// <summary>
    /// Interaction logic for CogoPointSurfaceReportView.xaml
    /// </summary>
    public partial class CogoPointSurfaceReportView : Window
    {
        public CogoPointSurfaceReportView(CogoPointSurfaceReportViewModel viewModel)
        {
            InitializeComponent();

            DataContext = viewModel;
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
