using System.Windows;
using _3DS_CivilSurveySuite.UI.ViewModels.AroFlo;

namespace _3DS_CivilSurveySuite.UI.Views.AroFlo
{
    /// <summary>
    /// Interaction logic for AroFloProjectView.xaml
    /// </summary>
    public partial class AroFloProjectView : Window
    {
        public AroFloProjectView(AroFloProjectViewModel viewModel)
        {
            InitializeComponent();

            DataContext = viewModel;
        }
    }
}
