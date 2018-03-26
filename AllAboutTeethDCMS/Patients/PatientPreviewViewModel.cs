using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllAboutTeethDCMS.Patients
{
    public class PatientPreviewViewModel : ViewModelBase
    {
        private Patient patient;
        private string visibility = "Collapsed";

        public Patient Patient { get => patient; set { patient = value; OnPropertyChanged(); } }
        public string Visibility { get => visibility; set { visibility = value; OnPropertyChanged(); } }
    }
}
