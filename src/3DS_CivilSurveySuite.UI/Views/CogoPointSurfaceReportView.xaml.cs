using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using _3DS_CivilSurveySuite.UI.ViewModels;

namespace _3DS_CivilSurveySuite.UI.Views
{
    /// <summary>
    /// Interaction logic for CogoPointSurfaceReportView.xaml
    /// </summary>
    public partial class CogoPointSurfaceReportView : Window
    {
        public CogoPointSurfaceReportView(CogoPointSurfaceReportViewModel viewModel)
        {
            InitializeComponent();

            DataContext = viewModel;
        }

        private void DataGrid_OnAutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            // DataGrid doesn't allow special characters in the column names.
            // Adding the character here will fix that issue.
            if ((e.PropertyName.Contains(".") ||
                 e.PropertyName.Contains("/") ||
                 e.PropertyName.Contains("&")
                 ) &&
                 e.Column is DataGridBoundColumn dataGridBoundColumn)
            {
                dataGridBoundColumn.Binding = new Binding("[" + e.PropertyName + "]");
            }

            if (e.PropertyType == typeof(double))
            {
                if (e.Column is DataGridTextColumn dataGridTextColumn)
                {
                    dataGridTextColumn.Binding.StringFormat = "N3";
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // Set view to report table.
            TabControl.SelectedIndex = 1;
        }

        private void SelectCurrentItem(object sender, KeyboardFocusChangedEventArgs e)
        {
            ListViewItem item = (ListViewItem) sender;
            item.IsSelected = true;
        }
    }
}