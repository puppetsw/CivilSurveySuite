using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace _3DS_CivilSurveySuite.Traverse
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class TraverseWindow : Window
    {
        public ObservableCollection<TraverseItem> TraverseItems { get; set; }

        public TraverseWindow()
        {
            InitializeComponent();

            TraverseItems = new ObservableCollection<TraverseItem>();
            lstView.ItemsSource = TraverseItems;

            TraverseItems.Add(new TraverseItem()
            {
                Bearing = 354.5020,
                Distance = 34.21,
            });
            TraverseItems.Add(new TraverseItem()
            {
                Bearing = 54.5020,
                Distance = 20.81,
            });
        }

        private void btnFeetToMeters_Click(object sender, RoutedEventArgs e)
        {
            if (lstView.SelectedIndex < 0) return;

            int index = lstView.SelectedIndex;

            double distance = TraverseItems[index].Distance;
            TraverseItems[index].Distance = TraverseBase.ConvertFeetToMeters(distance);
        }

        private void btnLinksToMeters_Click(object sender, RoutedEventArgs e)
        {
            if (lstView.SelectedIndex < 0) return;

            double distance = TraverseItems[lstView.SelectedIndex].Distance;
            TraverseItems[lstView.SelectedIndex].Distance = TraverseBase.ConvertLinkToMeters(distance);
        }

        private void btnAddRow_Click(object sender, RoutedEventArgs e)
        {
            TraverseItems.Add(new TraverseItem()); 
            //hack: add index property and update method
        }

        private void btnRemoveRow_Click(object sender, RoutedEventArgs e)
        {
            if (lstView.SelectedIndex < 0) return;
            var index = lstView.SelectedIndex;

            TraverseItems.Remove(TraverseItems[index]);
        }
    }

    public class TraverseItem : INotifyPropertyChanged
    {
        private double bearing;
        private double distance;

        public double Bearing { get => bearing; 
            set 
            {
                if (TraverseBase.IsValid(value))
                {
                    bearing = value;
                    NotifyPropertyChanged();
                }
                else bearing = 0;
            } 
        }
        public double Distance { get => distance; set { distance = value; NotifyPropertyChanged(); } }

        public TraverseItem()
        {
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }

    public class IndexConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            ListViewItem item = (ListViewItem)value;
            ListView listView = ItemsControl.ItemsControlFromItemContainer(item) as ListView;
            int index = listView.ItemContainerGenerator.IndexFromContainer(item);
            return index.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
