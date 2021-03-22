using _3DS_CivilSurveySuite.ViewModels;
using Microsoft.Win32;
using System.Windows;
using System.Windows.Controls;

namespace _3DS_CivilSurveySuite.Views
{
    /// <summary>
    /// Interaction logic for LineConnectView.xaml
    /// </summary>
    public partial class ConnectLineworkView : UserControl
    {
        public ConnectLineworkView()
        {
            InitializeComponent();
        }

        private void Load_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog();

            if (dialog.ShowDialog() == true)
            {
                string destinationFilePath = dialog.FileName;
                (DataContext as ConnectLineworkViewModel)?.Load(destinationFilePath);
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new SaveFileDialog();

            if (dialog.ShowDialog() == true)
            {
                string destinationFilePath = dialog.FileName;
                (DataContext as ConnectLineworkViewModel)?.Save(destinationFilePath);
            }
        }
    }
}
