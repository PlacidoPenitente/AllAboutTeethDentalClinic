using AllAboutTeethDCMS.Patients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AllAboutTeethDCMS.DentalCharts
{
    public class ToothViewModel : ViewModelBase
    {
        private Tooth tooth;

        public Patient Owner { get => Tooth.Owner; set { Tooth.Owner = value; OnPropertyChanged(); } }
        public string Condition { get => Tooth.Condition; set { Tooth.Condition = value; OnPropertyChanged(); } }
        public string ToothNo { get => Tooth.ToothNo; set { Tooth.ToothNo = value; OnPropertyChanged(); } }
        public Tooth Tooth { get => tooth; set { tooth = value; OnPropertyChanged();
                foreach (PropertyInfo info in GetType().GetProperties())
                {
                    OnPropertyChanged(info.Name);
                }
            } }
    }
}
