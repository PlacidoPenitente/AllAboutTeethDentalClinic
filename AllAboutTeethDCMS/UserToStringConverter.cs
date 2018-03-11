using AllAboutTeethDCMS.Users;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace AllAboutTeethDCMS
{
    public class UserToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value!=null)
            {
                User user = (User)value;
                return "(" + user.Username + ") " + user.LastName + ", " + user.FirstName + " " + user.MiddleName;
            }
            return "Unknown User";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return new User();
        }
    }
}
