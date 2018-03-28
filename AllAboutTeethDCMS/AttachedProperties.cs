using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace AllAboutTeethDCMS
{
    public class AttachedProperties : DependencyObject
    {
        #region ErrorDP
        public static readonly DependencyProperty ErrorProperty = DependencyProperty.RegisterAttached("Error", typeof(String), typeof(AttachedProperties), new FrameworkPropertyMetadata(""));

        public static void SetError(DependencyObject element, String value)
        {
            element.SetValue(ErrorProperty, value);
        }

        public static String GetError(DependencyObject element)
        {
            return (String)element.GetValue(ErrorProperty);
        }
        #endregion

        #region IconDP
        public static readonly DependencyProperty IconProperty = DependencyProperty.RegisterAttached("Icon", typeof(ImageSource), typeof(AttachedProperties), new FrameworkPropertyMetadata(new ImageBrush().ImageSource));

        public static void SetIcon(DependencyObject element, ImageSource value)
        {
            element.SetValue(IconProperty, value);
        }

        public static ImageSource GetIcon(DependencyObject element)
        {
            return (ImageSource)element.GetValue(IconProperty);
        }
        #endregion

        #region LabelDP
        public static readonly DependencyProperty LabelProperty = DependencyProperty.RegisterAttached("Label", typeof(String), typeof(AttachedProperties), new FrameworkPropertyMetadata(""));

        public static void SetLabel(DependencyObject element, String value)
        {
            element.SetValue(LabelProperty, value);
        }

        public static String GetLabel(DependencyObject element)
        {
            return (String)element.GetValue(LabelProperty);
        }
        #endregion

        #region ConditionDP
        public static readonly DependencyProperty ConditionProperty = DependencyProperty.RegisterAttached("Condition", typeof(String), typeof(AttachedProperties), new FrameworkPropertyMetadata(""));

        public static void SetCondition(DependencyObject element, String value)
        {
            element.SetValue(ConditionProperty, value);
        }

        public static String GetCondition(DependencyObject element)
        {
            return (String)element.GetValue(ConditionProperty);
        }
        #endregion

        #region ToothNoDP
        public static readonly DependencyProperty ToothNoProperty = DependencyProperty.RegisterAttached("ToothNo", typeof(String), typeof(AttachedProperties), new FrameworkPropertyMetadata(""));

        public static void SetToothNo(DependencyObject element, String value)
        {
            element.SetValue(ToothNoProperty, value);
        }

        public static String GetToothNo(DependencyObject element)
        {
            return (String)element.GetValue(ToothNoProperty);
        }
        #endregion
    }
}
