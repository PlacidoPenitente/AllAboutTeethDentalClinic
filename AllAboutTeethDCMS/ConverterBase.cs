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
    public abstract class ConverterBase : IValueConverter
    {
        private MySqlConnection connection;

        public MySqlConnection Connection { get => connection; set => connection = value; }

        public abstract object Convert(object value, Type targetType, object parameter, CultureInfo culture);
        public abstract object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture);

        public void createConnection()
        {
            Connection = new MySqlConnection();
            Connection.ConnectionString = "server=localhost; database='allaboutteeth_database'; user='docnanz'; password='docnanz';";
            Connection.Open();
        }
    }
}
