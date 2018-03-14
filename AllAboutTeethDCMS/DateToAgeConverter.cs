using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace AllAboutTeethDCMS
{
    public class DateToAgeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int age = 0;
            DateTime date = (DateTime)value;
            age = DateTime.Now.Year - date.Year;
            if(DateTime.Now.Month<date.Month)
            {
                age--;
            }
            else if(DateTime.Now.Month==date.Month&&DateTime.Now.Day<date.Day)
            {
                age--;
            }
            return age;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return DateTime.Now;
        }
    }
}
