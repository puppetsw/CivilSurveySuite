using System.Windows;
using _3DS_CivilSurveySuite.Model;
using _3DS_CivilSurveySuite.UI.Extensions;
using _3DS_CivilSurveySuite.UI.Services;
using _3DS_CivilSurveySuite.UI.ViewModels;

namespace _3DS_CivilSurveySuite.UI.Views
{
    /// <summary>
    /// Interaction logic for SurfaceSelectView.xaml
    /// </summary>
    public partial class SelectAlignmentView : Window, IDialogService<CivilAlignment>
    {
        public SelectAlignmentView(SelectAlignmentViewModel viewModel)
        {
            InitializeComponent();

            SourceInitialized += (x, y) => this.HideMinimizeAndMaximizeButtons();

            DataContext = viewModel;
        }

        private void Button_OK_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            DialogText = CmbAlignments.Text;
            ResultObject = (CivilAlignment)CmbAlignments.SelectedItem;
            Close();
        }

        private void Button_Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            DialogText = string.Empty;
            Close();
        }

        public string DialogText { get; set; }

        public CivilAlignment ResultObject { get; set; }
    }
}
