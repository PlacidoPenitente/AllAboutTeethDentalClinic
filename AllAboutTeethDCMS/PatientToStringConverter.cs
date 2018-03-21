using AllAboutTeethDCMS.Patients;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace AllAboutTeethDCMS
{
    public class PatientToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Patient patient = (Patient)value;
            return patient.No + ". " + patient.LastName + ", " + patient.FirstName + " " + patient.MiddleName;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return new Patient();
        }
    }
}
