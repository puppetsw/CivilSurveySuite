using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace _3DS_CivilSurveySuite.UI.UserControls
{
    /// <summary>
    /// Interaction logic for TraverseViewer.xaml
    /// </summary>
    public partial class TraverseViewer : Window
    {
        private IReadOnlyList<Model.Point> _points;

        private static double XMin => -50;
        private static double YMin => -50;
        private static double XMax => 50;
        private static double YMax => 50;

        public TraverseViewer()
        {
            InitializeComponent();
        }

        private void Grid_OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            ViewerCanvas.Width = ViewerGrid.ActualWidth;
            ViewerCanvas.Height = ViewerGrid.ActualHeight;
            Draw(_points);
        }

        public void Draw(IReadOnlyList<Model.Point> points)
        {
            if (points == null)
            {
                return;
            }

            _points = points;
            var polyline = new Polyline();
            polyline.Stroke = Brushes.Red;
            polyline.StrokeThickness = 1;
            foreach (var point in points)
            {
                var newPoint = new Point(XNormalise(point.X), YNormalise(point.Y));
                polyline.Points.Add(newPoint);
            }

            ViewerCanvas.Children.Clear();
            ViewerCanvas.Children.Add(polyline);
        }

        private double XNormalise(double x)
        {
            return (x - XMin) * ViewerCanvas.Width / (XMax - XMin);
        }

        private double YNormalise(double y)
        {
            return ViewerCanvas.Height - (y - YMin) * ViewerCanvas.Height / (YMax - YMin);
        }
    }
}
