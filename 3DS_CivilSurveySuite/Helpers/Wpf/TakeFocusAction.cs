﻿using Microsoft.Xaml.Behaviors;
using System.Windows;

namespace _3DS_CivilSurveySuite.Helpers.Wpf
{
    public class TakeFocusAction : TriggerAction<UIElement>
    {
        protected override void Invoke(object parameter)
        {
            AssociatedObject.Focus();
        }
    }
}