using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace CivilSurveySuite.UI.Converters
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
        public bool Inverted { get; set; }

        public bool Not { get; set; }

        private object VisibilityToBool(object value)
        {
            if (!(value is Visibility visibility))
                return DependencyProperty.UnsetValue;

            return (visibility == Visibility.Visible) ^ Not;
        }

        private object BoolToVisibility(object value)
        {
            if (!(value is bool b))
                return DependencyProperty.UnsetValue;

            return b ^ Not
                ? Visibility.Visible
                : Visibility.Collapsed;
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