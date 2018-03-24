using AllAboutTeethDCMS.Appointments;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace AllAboutTeethDCMS
{
    public class AppointmentToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((Appointment)value).No;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
           return new Appointment();
        }
    }
}
