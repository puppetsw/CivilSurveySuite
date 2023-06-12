using System.Windows;
using CivilSurveySuite.UI.ViewModels;

namespace CivilSurveySuite.UI.Views
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