using System;
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
                _fileName = dialog.FileName;
                var isLoaded = ((ConnectLineworkViewModel)DataContext).LoadSettings(_fileName);
                if (!isLoaded)
                {
                    MessageBox.Show("Unable to load Description Key file", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new SaveFileDialog();

            if (dialog.ShowDialog() == true)
            {
                _fileName = dialog.FileName;
                var isSaved = ((ConnectLineworkViewModel)DataContext).SaveSettings(_fileName);
                if (!isSaved)
                {
                    MessageBox.Show("Unable to save Description Key file. Please try again.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

        private void ConnectButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
