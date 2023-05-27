using System.Windows;
using CivilSurveySuite.UI.Extensions;
using CivilSurveySuite.Shared.Models;
using CivilSurveySuite.Shared.Services.Interfaces;
using CivilSurveySuite.UI.ViewModels;

namespace CivilSurveySuite.UI.Views
{
    /// <summary>
    /// Interaction logic for PointGroupSelectView.xaml
    /// </summary>
    public partial class SelectPointGroupView : Window, IDialogService<CivilPointGroup>
    {
        public SelectPointGroupView(SelectPointGroupViewModel viewModel)
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
