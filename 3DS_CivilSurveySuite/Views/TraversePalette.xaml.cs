using _3DS_CivilSurveySuite.ViewModels;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Controls;
using System.Windows.Input;

namespace _3DS_CivilSurveySuite.Views
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

        //private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        //{
        //    Regex regex = new Regex("[^0-9.+-]+");
        //    e.Handled = regex.IsMatch(e.Text) && ((Regex.Matches(e.Text, "+").Count <= 1) || (Regex.Matches(e.Text, "-").Count <= 1));

        //    //e.Handled = e.Text.
        //}
    }
}

