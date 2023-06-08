using System.Windows;
using CivilSurveySuite.UI.ViewModels;

namespace CivilSurveySuite.UI.Views
{
    /// <summary>
    /// Interaction logic for CogoPointReplaceDuplicateView.xaml
    /// </summary>
    public partial class CogoPointReplaceDuplicateView : Window
    {
        public CogoPointReplaceDuplicateView(CogoPointReplaceDuplicateViewModel viewModel)
        {
            InitializeComponent();

            DataContext = viewModel;
        }

        private void BtnOk_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}