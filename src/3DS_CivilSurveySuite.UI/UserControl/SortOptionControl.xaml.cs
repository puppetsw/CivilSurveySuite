using System.Collections.Generic;
using System.Windows;
using _3DS_CivilSurveySuite.Shared.Models;

namespace _3DS_CivilSurveySuite.UI.UserControl
{
    /// <summary>
    /// Interaction logic for SortOptionControl.xaml
    /// </summary>
    public partial class SortOptionControl
    {
        public static readonly DependencyProperty SelectedHeaderProperty = DependencyProperty.Register(
            "SelectedHeader", typeof(ColumnHeader), typeof(SortOptionControl),
            new PropertyMetadata(default(ColumnHeader)));

        public ColumnHeader SelectedHeader
        {
            get => (ColumnHeader)GetValue(SelectedHeaderProperty);
            set => SetValue(SelectedHeaderProperty, value);
        }

        public static readonly DependencyProperty HeadersProperty = DependencyProperty.Register(
            "Headers", typeof(IEnumerable<ColumnHeader>), typeof(SortOptionControl),
            new PropertyMetadata(default(IEnumerable<ColumnHeader>)));

        public IEnumerable<ColumnHeader> Headers
        {
            get => (IEnumerable<ColumnHeader>)GetValue(HeadersProperty);
            set => SetValue(HeadersProperty, value);
        }

        public static readonly DependencyProperty SortDirectionProperty = DependencyProperty.Register(
            "SortDirection", typeof(SortDirection), typeof(SortOptionControl),
            new PropertyMetadata(default(SortDirection)));

        public SortDirection SortDirection
        {
            get => (SortDirection)GetValue(SortDirectionProperty);
            set => SetValue(SortDirectionProperty, value);
        }

        public SortColumnHeader SortColumnHeader => new SortColumnHeader { ColumnHeader = SelectedHeader, SortDirection = SortDirection };

        public SortOptionControl()
        {
            InitializeComponent();

            LayoutRoot.DataContext = this;
        }
    }
}
