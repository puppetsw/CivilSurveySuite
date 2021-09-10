using System.Windows;
using _3DS_CivilSurveySuite.Model;
using _3DS_CivilSurveySuite.UI.Services;
using _3DS_CivilSurveySuite.UI.ViewModels;

namespace _3DS_CivilSurveySuite.UI.Views
{
    /// <summary>
    /// Interaction logic for SurfaceSelectView.xaml
    /// </summary>
    public partial class SurfaceSelectView : Window, IDialogService<CivilSurface>
    {
        public SurfaceSelectView(SurfaceSelectViewModel viewModel)
        {
            InitializeComponent();

            DataContext = viewModel;
        }

        private void Button_OK_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            DialogText = CmbSurfaces.Text;
            ResultObject = (CivilSurface)CmbSurfaces.SelectedItem;
            Close();
        }

        private void Button_Cancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            DialogText = string.Empty;
            Close();
        }

        public string DialogText { get; set; }

        public CivilSurface ResultObject { get; set; }
    }
}
