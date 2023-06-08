using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CivilSurveySuite.UI.Behaviors
{
    public class SimpleCalculationTextBoxBehavior
    {
        protected SimpleCalculationTextBoxBehavior()
        { }

        public static bool GetEnable(FrameworkElement frameworkElement)
        {
            return (bool)frameworkElement.GetValue(EnableProperty);
        }

        public static void SetEnable(FrameworkElement frameworkElement, bool value)
        {
            frameworkElement.SetValue(EnableProperty, value);
        }

        public static readonly DependencyProperty EnableProperty = DependencyProperty.RegisterAttached("Enable", typeof(bool), typeof(SimpleCalculationTextBoxBehavior), new FrameworkPropertyMetadata(false, OnEnableChanged));

        private static void OnEnableChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var frameworkElement = d as FrameworkElement;
            if (frameworkElement == null) return;

            if (e.NewValue is bool == false) return;

            if ((bool)e.NewValue)
                frameworkElement.PreviewTextInput += PreviewTextInput;
            else
                frameworkElement.PreviewTextInput -= PreviewTextInput;
        }

        private static void PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            Regex regex = new Regex("[^0-9.+-]+");

            if (e.Text == "+" || e.Text == "-")
            {
                // if the there is no numbers in the textbox don't allow +
                if (string.IsNullOrEmpty(textBox.Text))
                {
                    e.Handled = true;
                    return;
                }

                if (textBox.Text.Contains("+") || textBox.Text.Contains("-"))
                    e.Handled = true;
            }
            else
                e.Handled = regex.IsMatch(e.Text);
        }

    }
}