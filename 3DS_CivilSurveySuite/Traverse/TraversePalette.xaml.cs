using System.Windows.Controls;

namespace _3DS_CivilSurveySuite.Traverse
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class TraversePalette : UserControl
    {
        public TraversePalette()
        {
            InitializeComponent();
            DataContext = new TraverseViewModel();

        }
    }
}
