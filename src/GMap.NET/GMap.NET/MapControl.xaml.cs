using System;
using System.Collections.Generic;
using System.Linq;
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

namespace GMap.NET
{
    /// <summary>
    /// Interaction logic for MapControl.xaml
    /// </summary>
    public partial class MapControl : UserControl
    {
        public static readonly DependencyProperty ZoomProperty = DependencyProperty.Register(
            "Zoom", typeof(int), typeof(MapControl), new PropertyMetadata(default(int)));

        public int Zoom
        {
            get => (int)GetValue(ZoomProperty);
            set => SetValue(ZoomProperty, value);
        }

        public static readonly DependencyProperty MinZoomProperty = DependencyProperty.Register(
            "MinZoom", typeof(int), typeof(MapControl), new PropertyMetadata(default(int)));

        public int MinZoom
        {
            get => (int)GetValue(MinZoomProperty);
            set => SetValue(MinZoomProperty, value);
        }

        public static readonly DependencyProperty MaxZoomProperty = DependencyProperty.Register(
            "MaxZoom", typeof(int), typeof(MapControl), new PropertyMetadata(default(int)));

        public int MaxZoom
        {
            get => (int)GetValue(MaxZoomProperty);
            set => SetValue(MaxZoomProperty, value);
        }

        public static readonly DependencyProperty PositionProperty = DependencyProperty.Register(
            "Position", typeof(LatLong), typeof(MapControl), new PropertyMetadata(default(LatLong)));

        public LatLong Position
        {
            get => (LatLong)GetValue(PositionProperty);
            set
            {
                SetValue(PositionProperty, value);
                // Update the map position.
                MainMap.Position = new PointLatLng(Position.Latitude, Position.Longitude);
            }
        }

        public MapControl()
        {
            InitializeComponent();
        }
    }
}
