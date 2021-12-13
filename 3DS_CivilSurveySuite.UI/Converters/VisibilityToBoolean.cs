using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace _3DS_CivilSurveySuite.UI.Converters
{
    // <src:VisToBool x:Key="TrueIfVisible" Inverted="False" Not="False" />
    // <src:VisToBool x:Key="TrueIfNotVisible" Inverted="False" Not="True" />
    // <src:VisToBool x:Key="VisibleIfTrue" Inverted="True" Not="False" />
    // <src:VisToBool x:Key="VisibleIfNotTrue" Inverted="True" Not="True" />

    /// <summary>
    /// Visibility to boolean converter.
    /// </summary>
    /// <remarks>
    /// https://social.msdn.microsoft.com/Forums/vstudio/en-US/f2154f63-ccb5-4d6d-8c01-81f9da9ab347/invert-booleantovisibilityconverter?forum=wpf
    /// </remarks>
    public class VisibilityToBoolean : IValueConverter
    {
        private bool _inverted = false;

        public bool Inverted
        {
            get { return _inverted; }
            set { _inverted = value; }
        }

        private bool _not = false;

        public bool Not
        {
            get { return _not; }
            set { _not = value; }
        }

        private object VisibilityToBool(object value)
        {
            if (!(value is Visibility)) return DependencyProperty.UnsetValue;
            return (((Visibility)value) == Visibility.Visible) ^ Not;
        }

        private object BoolToVisibility(object value)
        {
            if (!(value is bool)) return DependencyProperty.UnsetValue;
            return ((bool)value ^ Not) ? Visibility.Visible : Visibility.Collapsed;
        }

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Inverted ? BoolToVisibility(value) : VisibilityToBool(value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Inverted ? VisibilityToBool(value) : BoolToVisibility(value);
        }
    }
}