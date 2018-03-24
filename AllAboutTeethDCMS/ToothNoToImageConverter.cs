using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace AllAboutTeethDCMS
{
    public class ToothNoToImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(!String.IsNullOrEmpty((string)value))
            {
                int tooth = Int32.Parse(((string)value));
                if (tooth > 48)
                {
                    tooth = tooth - 40;
                }
                return "/AllAboutTeethDCMS;component/Resources/" + tooth + ".png";
            }
            return "";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return "0";
        }
    }
}
