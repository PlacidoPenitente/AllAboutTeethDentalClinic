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
            User user = (User)value;
            if(String.IsNullOrEmpty(user.Username))
            {
                return "Unknown User";
            }
            return "(" + user.Username + ") " + user.LastName + ", " + user.FirstName + " " + user.MiddleName;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return new User();
        }
    }
}
