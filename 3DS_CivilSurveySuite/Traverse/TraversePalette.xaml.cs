using Autodesk.AutoCAD.Geometry;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
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
    public partial class TraversePalette : UserControl
    {
        public ObservableCollection<TraverseItem> TraverseItems { get; set; }

        public TraversePalette()
        {
            InitializeComponent();

            TraverseItems = new ObservableCollection<TraverseItem>();
            lstView.ItemsSource = TraverseItems;

            TraverseItems.Add(new TraverseItem()
            {
                Index = 0,
                Bearing = 354.5020,
                Distance = 34.21,
            });
            TraverseItems.Add(new TraverseItem()
            {
                Index = 1,
                Bearing = 84.5020,
                Distance = 20.81,
            });
            TraverseItems.Add(new TraverseItem()
            {
                Index = 2,
                Bearing = 174.5020,
                Distance = 20.81,
            });
        }

        #region Button Events

        private void btnFeetToMeters_Click(object sender, RoutedEventArgs e)
        {
            if (lstView.SelectedIndex < 0) return;

            int index = lstView.SelectedIndex;

            double distance = TraverseItems[index].Distance;
            TraverseItems[index].Distance = MathHelpers.ConvertFeetToMeters(distance);
        }

        private void btnLinksToMeters_Click(object sender, RoutedEventArgs e)
        {
            if (lstView.SelectedIndex < 0) return;

            double distance = TraverseItems[lstView.SelectedIndex].Distance;
            TraverseItems[lstView.SelectedIndex].Distance = MathHelpers.ConvertLinkToMeters(distance);
        }

        private void btnAddRow_Click(object sender, RoutedEventArgs e)
        {
            TraverseItems.Add(new TraverseItem());
            //hack: add index property and update method
            TraverseItem.UpdateIndex(TraverseItems);
        }

        private void btnRemoveRow_Click(object sender, RoutedEventArgs e)
        {
            if (lstView.SelectedIndex < 0) return;
            var index = lstView.SelectedIndex;

            TraverseItems.Remove(TraverseItems[index]);
            TraverseItem.UpdateIndex(TraverseItems);
        }

        private void btnClosure_Click(object sender, RoutedEventArgs e)
        {
            var coordinates = MathHelpers.BearingAndDistanceToCoordinates(TraverseItems, new Point2d(0,0));

            Point2d lastCoord = coordinates[coordinates.Count - 1];
            Point2d firstCoord = coordinates[0];

            var distance = MathHelpers.DistanceBetweenPoints(firstCoord.X, lastCoord.X, firstCoord.Y, lastCoord.Y);
            var angle = MathHelpers.AngleBetweenPoints(lastCoord.X, firstCoord.X, lastCoord.Y, firstCoord.Y);

            string message = string.Format("{0} {1}", distance, angle.ToString());

            MessageBox.Show(message);
        }

        #endregion
    }
}
