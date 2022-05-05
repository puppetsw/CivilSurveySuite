using System.Windows;

namespace TestApp
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
