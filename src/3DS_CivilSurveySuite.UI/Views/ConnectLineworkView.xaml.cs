using System.ComponentModel;
using System.Windows;
using _3DS_CivilSurveySuite.UI.ViewModels;
using Microsoft.Win32;

namespace _3DS_CivilSurveySuite.UI.Views
{
    /// <summary>
    /// Interaction logic for LineConnectView.xaml
    /// </summary>
    public partial class ConnectLineworkView : Window
    {
        private string _fileName;

        public ConnectLineworkView(ConnectLineworkViewModel viewModel)
        {
            InitializeComponent();

            DataContext = viewModel;

            Closing += SaveSettings;
        }

        private void SaveSettings(object sender, CancelEventArgs e)
        {
            Properties.Settings.Default.DescriptionKeyFileName = _fileName;
            Properties.Settings.Default.Save();
            Closing -= SaveSettings;
        }

        private void Load_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog();

            if (dialog.ShowDialog() == true)
            {
                string destinationFilePath = dialog.FileName;
                _fileName = destinationFilePath;
                (DataContext as ConnectLineworkViewModel)?.Load(destinationFilePath);
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new SaveFileDialog();

            if (dialog.ShowDialog() == true)
            {
                string destinationFilePath = dialog.FileName;
                _fileName = destinationFilePath;
                (DataContext as ConnectLineworkViewModel)?.Save(destinationFilePath);
            }
        }

        private void ConnectButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
