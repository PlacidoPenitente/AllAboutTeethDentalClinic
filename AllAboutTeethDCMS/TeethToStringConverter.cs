using AllAboutTeethDCMS.DentalCharts;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace AllAboutTeethDCMS
{
    public class TeethToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            List<Tooth> teeth = (List<Tooth>)value;
            string toothNos = "";
            foreach (Tooth tooth in teeth)
            {
                toothNos += tooth.ToothNo+", ";
            }
            toothNos = toothNos.Trim();
            if(toothNos.Length>0)
            {
                toothNos = toothNos.Remove(toothNos.Length - 1);
            }
            return toothNos;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return new List<Tooth>();
        }
    }
}
