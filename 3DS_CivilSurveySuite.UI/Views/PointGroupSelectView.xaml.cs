using System.Windows;
using _3DS_CivilSurveySuite.Model;
using _3DS_CivilSurveySuite.UI.Extensions;
using _3DS_CivilSurveySuite.UI.Services;
using _3DS_CivilSurveySuite.UI.ViewModels;

namespace _3DS_CivilSurveySuite.UI.Views
{
    /// <summary>
    /// Interaction logic for PointGroupSelectView.xaml
    /// </summary>
    public partial class PointGroupSelectView : Window, IDialogService<CivilPointGroup>
    {
        public PointGroupSelectView(PointGroupSelectViewModel viewModel)
        {
            InitializeComponent();

            SourceInitialized += (x, y) => this.HideMinimizeAndMaximizeButtons();

            DataContext = viewModel;
        }

        private void Button_OK_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            ResultObject = (CivilPointGroup)CmbPointGroup.SelectedItem;
            Close();
        }

        private void Button_Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            ResultObject = null;
            Close();
        }

        public CivilPointGroup ResultObject { get; set; }
    }
}
