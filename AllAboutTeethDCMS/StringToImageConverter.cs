using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace AllAboutTeethDCMS
{
    public class StringToImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(!String.IsNullOrEmpty(((string)value)))
            {
                return new Uri((string)value, UriKind.Relative);
            }
            else
            {
                return new ImageBrush().ImageSource;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return "";
        }
    }
}
