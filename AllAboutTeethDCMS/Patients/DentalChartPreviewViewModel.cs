using AllAboutTeethDCMS.DentalChart;
using AllAboutTeethDCMS.DentalCharts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace AllAboutTeethDCMS.Patients
{
    public class DentalChartPreviewViewModel : ViewModelBase
    {
        private Patient patient;

        private List<string> outputs;
        private string output = "Present Teeth";

        public DentalChartPreviewViewModel()
        {
            DentalChartViewModel = new DentalChartViewModel();
            Outputs = new List<string>()
            {
                "Decayed (Caries Indicated For Filling)",
                "Missing Due To Caries",
                "Filled",
                "Caries Indicated For Extraction",
                "Root Fragment",
                "Missing Due To Other Causes",
                "Impacted Tooth",
                "Jacket Crown",
                "Amalgam Filling",
                "Abutment",
                "Pontic",
                "Inlay",
                "Fixed Cure Composite",
                "Removable Denture",
                "Extraction Due To Caries",
                "Extraction Due To Other Causes",
                "Present Teeth",
                "Congenitally Missing",
                "Supernumerary"
            };

            UpdateCommand = new DelegateCommand(new Action(update));
        }
        private DentalChartViewModel dentalChartViewModel;

        public Patient Patient { get => patient; set {
                patient = value;
                DentalChartViewModel.Patient = value;
                OnPropertyChanged(); } }
        public DentalChartViewModel DentalChartViewModel { get => dentalChartViewModel; set { dentalChartViewModel = value; OnPropertyChanged(); } }

        public List<string> Outputs { get => outputs; set => outputs = value; }
        public string Output { get => output; set { output = value; OnPropertyChanged(); } }

        public DelegateCommand UpdateCommand { get => updateCommand; set => updateCommand = value; }

        private DelegateCommand updateCommand;

        public void update()
        {
            if(MessageBox.Show("Are you sure you want to change the condition of selected teeth?", "Change Condition", MessageBoxButton.YesNo, MessageBoxImage.Question)==MessageBoxResult.Yes)
            {
                foreach (ToothViewModel toothViewModel in DentalChartViewModel.TeethView)
                {
                    toothViewModel.Condition = Output;
                    toothViewModel.ActiveUser = DentalChartViewModel.User;
                    toothViewModel.saveTooth();
                    toothViewModel.loadTooth();
                }
                MessageBox.Show("Teeth successfully updated.", "Teeth Updated", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }
    }
}
