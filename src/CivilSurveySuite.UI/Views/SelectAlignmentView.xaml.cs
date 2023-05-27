using System.Windows;
using CivilSurveySuite.UI.Extensions;
using CivilSurveySuite.Shared.Models;
using CivilSurveySuite.Shared.Services.Interfaces;
using CivilSurveySuite.UI.ViewModels;

namespace CivilSurveySuite.UI.Views
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
