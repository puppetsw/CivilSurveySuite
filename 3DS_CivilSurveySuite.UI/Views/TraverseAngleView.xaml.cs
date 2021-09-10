using System.Windows.Controls;
using System.Windows.Input;
using _3DS_CivilSurveySuite.UI.ViewModels;

namespace _3DS_CivilSurveySuite.UI.Views
{
    /// <summary>
    /// Interaction logic for TraverseAngleView.xaml
    /// </summary>
    public partial class TraverseAngleView : UserControl
    {
        public TraverseAngleView(TraverseAngleViewModel viewModel)
        {
            InitializeComponent();

            DataContext = viewModel;
        }

        private void dgTraverse_PreviewKeyDown(object sender, KeyEventArgs e)
        {
        }
    }
}
