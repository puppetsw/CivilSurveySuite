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
            "Position", typeof(Position), typeof(BingMapControl), new PropertyMetadata(default(Position)));

        public Position Position
        {
            get => (Position)GetValue(PositionProperty);
            set
            {
                SetValue(PositionProperty, value);
                MainMap.Center = new Location(Position.Latitude, Position.Longitude);
            }
        }

        public BingMapControl()
        {
            InitializeComponent();
        }
    }
}
