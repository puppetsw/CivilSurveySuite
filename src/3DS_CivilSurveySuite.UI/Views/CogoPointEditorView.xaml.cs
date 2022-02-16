using System.Windows;
using _3DS_CivilSurveySuite.UI.ViewModels;

namespace _3DS_CivilSurveySuite.UI.Views
{
    /// <summary>
    /// Interaction logic for CogoPointViewer.xaml
    /// </summary>
    public partial class CogoPointEditorView : Window
    {
        public CogoPointEditorView(CogoPointEditorViewModel viewModel)
        {
            InitializeComponent();

            DataContext = viewModel;
        }
    }
}