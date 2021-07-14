using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;
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
        private const double XIncrement = 10;
        private const double YIncrement = 10;
        private Point _startPoint;
        private Point _endPoint;
        private TranslateTransform _transform;

        public double XMin { get; set; } = -50;
        public double YMin { get; set; } = -50;
        public double XMax { get; set; } = 50;
        public double YMax { get; set; } = 50;

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
                return;

            _points = points;
            var polyline = new Polyline();
            polyline.Stroke = Brushes.Red;
            polyline.StrokeThickness = 2;
            foreach (var point in points)
            {
                Point newPoint = _transform == null ? new Point(XNormalise(point.X), YNormalise(point.Y)) : 
                                                      new Point(XNormalise(point.X) + _transform.X, YNormalise(point.Y) + _transform.Y);
                polyline.Points.Add(newPoint);
            }

            ViewerCanvas.Children.Clear();
            ViewerCanvas.Children.Add(polyline);

            // Apply transform from a shift if there is one.
            if (_transform != null)
            {
                foreach (Shape drawingObject in ViewerCanvas.Children)
                {
                    drawingObject.RenderTransform = _transform;
                }
            }
        }

        private double XNormalise(double x)
        {
            return (x - XMin) * ViewerCanvas.Width / (XMax - XMin);
        }

        private double YNormalise(double y)
        {
            return (ViewerCanvas.Height - (y - YMin) * ViewerCanvas.Height / (YMax - YMin));
        }

        private void ViewerCanvas_OnMouseWheel(object sender, MouseWheelEventArgs e)
        {
            double dx = (e.Delta > 0) ? XIncrement : -XIncrement;
            double dy = (e.Delta > 0) ? YIncrement : -YIncrement;
            double x0 = XMin + (XMax - XMin) * dx / ViewerCanvas.Width;
            double x1 = XMax - (XMax - XMin) * dx / ViewerCanvas.Width;
            double y0 = YMin + (YMax - YMin) * dy / ViewerCanvas.Height;
            double y1 = YMax - (YMax - YMin) * dy / ViewerCanvas.Height;

            XMin = x0;
            XMax = x1;
            YMin = y0;
            YMax = y1;

            Draw(_points);
        }

        private void ViewerCanvas_OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (!ViewerCanvas.IsMouseCaptured)
            {
                _startPoint = e.GetPosition(ViewerCanvas);
                ViewerCanvas.CaptureMouse();
            }
        }

        private void ViewerCanvas_OnMouseMove(object sender, MouseEventArgs e)
        {
            if (!ViewerCanvas.IsMouseCaptured) 
                return;

            _endPoint = e.GetPosition(ViewerCanvas);

            if (_transform == null)
            {
                _transform = new TranslateTransform();
            }

            _transform.X = _endPoint.X - _startPoint.X;
            _transform.Y = _endPoint.Y - _startPoint.Y;

            foreach (Shape drawingObject in ViewerCanvas.Children)
            {
                drawingObject.RenderTransform = _transform;
            }
        }

        //BUG: Position is updated, but when you click to drag, it returns to the previous position, leaving the mouse where you licked
        // So it's like it's offset. Need to apply the transform shift to the actual points, and not just a render?

        private void ViewerCanvas_OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            _endPoint = e.GetPosition(ViewerCanvas);
            //var dx = (XMax - XMin) * (_endPoint.X - _startPoint.X) / ViewerCanvas.Width;
            //var dy = (YMax - YMin) * (_endPoint.Y - _startPoint.Y) / ViewerCanvas.Height;

            //XMin += dx;
            //XMax += dx;
            //YMin += dy;
            //YMax += dy;

            ViewerCanvas.ReleaseMouseCapture();
            ViewerCanvas.Cursor = Cursors.Arrow;

            //Draw(_points);
        }
    }
}
