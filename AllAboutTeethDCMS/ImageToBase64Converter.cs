using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace AllAboutTeethDCMS
{
    public class ImageToBase64Converter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                string s = value as string;

                BitmapImage bi = new BitmapImage();

                bi.BeginInit();
                bi.StreamSource = new MemoryStream(System.Convert.FromBase64String(s));
                bi.EndInit();

                return bi;
            }
            catch(Exception ex)
            {
                return "/AllAboutTeethDCMS;component/Resources/icons8_User_100px_1.png";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
