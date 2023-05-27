using System.Windows;
using Microsoft.Xaml.Behaviors;

namespace CivilSurveySuite.UI.Behaviors
{
    public class TakeFocusAction : TriggerAction<UIElement>
    {
        protected override void Invoke(object parameter)
        {
            AssociatedObject.Focus();
        }
    }
}
