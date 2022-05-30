using System.Windows;
using _3DS_CivilSurveySuite.Shared.Models;
using Microsoft.Maps.MapControl.WPF;

namespace _3DS_CivilSurveySuite.UI.UserControl
{
    /// <summary>
    /// Interaction logic for BingMapControl.xaml
    /// </summary>
    public partial class BingMapControl
    {
        public static readonly DependencyProperty PositionProperty = DependencyProperty.Register(
            "Position", typeof(GpsPosition), typeof(BingMapControl), new PropertyMetadata(default(GpsPosition), UpdateMainMapPosition));

        private static void UpdateMainMapPosition(
            DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            var control = (BingMapControl)obj;

            // react on value change here
            var pos = (GpsPosition)e.NewValue;
            var location = new Location(pos.Latitude, pos.Longitude);
            control.MainMap.Center = location;
            control.MainMap.ZoomLevel = 16;

            // Adds the pushpin to the map.
            control.MainMap.Children.Clear();
            var pin = new Pushpin
            {
                Location = location
            };
            control.MainMap.Children.Add(pin);
        }

        public GpsPosition Position
        {
            get => (GpsPosition)GetValue(PositionProperty);
            set => SetValue(PositionProperty, value);
        }

        public BingMapControl()
        {
            InitializeComponent();
            LayoutRoot.DataContext = this;
        }
    }
}
