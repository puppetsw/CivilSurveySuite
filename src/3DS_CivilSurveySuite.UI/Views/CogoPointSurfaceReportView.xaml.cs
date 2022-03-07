using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
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
            if (e.PropertyName.Contains(".") && e.Column is DataGridBoundColumn dataGridBoundColumn)
            {
                dataGridBoundColumn.Binding = new Binding("[" + e.PropertyName + "]");
            }

            if (e.PropertyType == typeof(double))
            {
                DataGridTextColumn dataGridTextColumn = e.Column as DataGridTextColumn;
                if (dataGridTextColumn != null)
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
    }
}