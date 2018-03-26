using AllAboutTeethDCMS.DentalChart;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllAboutTeethDCMS.Patients
{
    public class DentalChartPreviewViewModel : ViewModelBase
    {
        private Patient patient;
        private string visibility = "Collapsed";
        public DentalChartPreviewViewModel()
        {
            DentalChartViewModel = new DentalChartViewModel();
        }
        private DentalChartViewModel dentalChartViewModel;

        public Patient Patient { get => patient; set {
                patient = value;
                DentalChartViewModel.Patient = value;
                OnPropertyChanged(); } }
        public string Visibility { get => visibility; set { visibility = value; OnPropertyChanged(); } }
        public DentalChartViewModel DentalChartViewModel { get => dentalChartViewModel; set { dentalChartViewModel = value; OnPropertyChanged(); } }
    }
}
