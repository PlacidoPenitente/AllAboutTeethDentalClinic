using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace AllAboutTeethDCMS
{
    public class ConditionToSymbolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch((string)value)
            {
                case "Decayed (Caries Indicated For Filling)": return "D";
                case "Missing Due To Caries": return "M";
                case "Filled": return "F";
                case "Caries Indicated For Extraction": return "I";
                case "Root Fragment": return "RF";
                case "Missing Due To Other Causes": return "MO";
                case "Impacted Tooth": return "Im";
                case "Jacket Crown": return "J";
                case "Amalgam Filling": return "A";
                case "Abutment": return "AB";
                case "Pontic": return "P";
                case "Inlay": return "In";
                case "Fixed Cure Composite": return "FX";
                case "Removable Denture": return "Rm";
                case "Extraction Due To Caries": return "X";
                case "Extraction Due To Other Causes": return "XO";
                case "Present Teeth": return "√";
                case "Congenitally Missing": return "Cm";
                case "Supernumerary": return "Sp";
                default: return "";
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return "Present Teeth";
        }
    }
}
