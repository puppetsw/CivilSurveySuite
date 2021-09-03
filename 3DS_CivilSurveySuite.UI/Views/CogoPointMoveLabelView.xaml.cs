using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using _3DS_CivilSurveySuite.UI.ViewModels;

namespace _3DS_CivilSurveySuite.UI.Views
{
    /// <summary>
    /// Interaction logic for CogoPointMoveLabelView.xaml
    /// </summary>
    public partial class CogoPointMoveLabelView : Window
    {
        public CogoPointMoveLabelView(CogoPointMoveLabelViewModel viewModel)
        {
            InitializeComponent();

            DataContext = viewModel;
        }
    }
}
