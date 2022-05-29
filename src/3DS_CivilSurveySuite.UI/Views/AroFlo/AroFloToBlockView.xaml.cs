using System.Windows;
using _3DS_CivilSurveySuite.UI.ViewModels.AroFlo;

namespace _3DS_CivilSurveySuite.UI.Views.AroFlo
{
    /// <summary>
    /// Interaction logic for AroFloToBlockView.xaml
    /// </summary>
    public partial class AroFloToBlockView : Window
    {
        public AroFloToBlockView(AroFloToBlockViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
