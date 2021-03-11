using _3DS_CivilSurveySuite.ViewModels;
using System.Windows.Controls;

namespace _3DS_CivilSurveySuite.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class TraversePalette : UserControl
    {
        TraverseViewModel dataContext;

        public TraversePalette()
        {
            InitializeComponent();
            dataContext = new TraverseViewModel();

            //HACK: No EventToCommand?
            //Unloaded += (s, e) => {
            //    if (dataContext != null)
            //        dataContext.ClearTransientGraphics();
            //};

            DataContext = dataContext;
        }
    }
}

