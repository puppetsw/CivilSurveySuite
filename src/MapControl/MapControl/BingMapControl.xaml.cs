using System.Windows;
using System.Windows.Controls;
using Microsoft.Maps.MapControl.WPF;

namespace MapControl
{
    /// <summary>
    /// Interaction logic for BingMapControl.xaml
    /// </summary>
    public partial class BingMapControl : UserControl
    {
        public static readonly DependencyProperty PositionProperty = DependencyProperty.Register(
            "Position", typeof(Position), typeof(BingMapControl), new PropertyMetadata(default(Position), UpdateMainMapPosition));

        private static void UpdateMainMapPosition(
            DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            var control = (BingMapControl)obj;

            // react on value change here
            var pos = (Position)e.NewValue;
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

        public Position Position
        {
            get => (Position)GetValue(PositionProperty);
            set => SetValue(PositionProperty, value);
        }

        public BingMapControl()
        {
            InitializeComponent();
            LayoutRoot.DataContext = this;
        }
    }
}
