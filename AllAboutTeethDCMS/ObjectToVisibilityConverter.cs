﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace AllAboutTeethDCMS
{
    public class ObjectToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                if (((string)value).Trim().Equals(""))
                {
                    return "Collapsed";
                }
                return "Visible";
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                if (value == null)
                {
                    return "Collapsed";
                }
                return "Visible";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
