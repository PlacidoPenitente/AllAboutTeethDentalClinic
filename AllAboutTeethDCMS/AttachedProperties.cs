using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace AllAboutTeethDCMS
{
    public class AttachedProperties : DependencyObject
    {
        public static readonly DependencyProperty LabelProperty = DependencyProperty.RegisterAttached("Label", typeof(String), typeof(AttachedProperties), new FrameworkPropertyMetadata(""));

        public static void SetLabel(DependencyObject element, String value)
        {
            element.SetValue(LabelProperty, value);
        }

        public static String GetLabel(DependencyObject element)
        {
            return (String)element.GetValue(LabelProperty);
        }

        public static readonly DependencyProperty LabelVisibilityProperty = DependencyProperty.RegisterAttached("LabelVisibility", typeof(Visibility), typeof(AttachedProperties), new FrameworkPropertyMetadata(Visibility.Visible));

        public static void SetLabelVisibility(DependencyObject element, Visibility value)
        {
            element.SetValue(LabelVisibilityProperty, value);
        }

        public static Visibility GetLabelVisibility(DependencyObject element)
        {
            return (Visibility)element.GetValue(LabelVisibilityProperty);
        }
    }
}
