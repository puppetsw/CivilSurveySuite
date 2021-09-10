using System.Windows;
using _3DS_CivilSurveySuite.UI.Extensions;
using _3DS_CivilSurveySuite.UI.ViewModels;

namespace _3DS_CivilSurveySuite.UI.Views
{
    /// <summary>
    /// Interaction logic for CogoPointMoveLabelView.xaml
    /// </summary>
    public partial class CogoPointMoveLabelView : Window
    {
        public CogoPointMoveLabelView(CogoPointMoveLabelViewModel viewModel)
        {
            InitializeComponent();

            SourceInitialized += (x, y) => this.HideMinimizeAndMaximizeButtons();

            DataContext = viewModel;
        }

        private void Move_Button_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void Cancel_Button_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
