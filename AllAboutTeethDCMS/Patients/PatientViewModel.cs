using AllAboutTeethDCMS.DentalChart;
using AllAboutTeethDCMS.TreatmentRecords;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AllAboutTeethDCMS.Patients
{
    public class PatientViewModel : CRUDPage<Patient>
    {
        private Patient patient;
        private List<Patient> patients;
        private string filter = "";
        private string condition;
        private DentalChartViewModel dentalChartViewModel;
        private string visibility = "collapsed";
        private int selectedTooth = 0;
        private List<TreatmentRecord> treatmentRecords;
        private TreatmentRecordViewModel treatmentRecordsViewModel;

        public PatientViewModel()
        {
            DentalChartViewModel = new DentalChartViewModel();
            treatmentRecordsViewModel = new TreatmentRecordViewModel();
        }

        private List<string> conditions = new List<string>()
        {
            "Decayed (Caries Indicated for Filling)",
            "Missing due to Caries",
            "Filled",
            "Caries Indicated for Extraction",
            "Root Fragment",
            "Missing due to Other Causes",
            "Impacted Tooth",
            "Jacket Crown",
            "Amalgam Filling",
            "Abutment",
            "Pontic",
            "Inlay",
            "Fixed Cure Composite",
            "Removable Denture",
            "Extracted due to Caries",
            "Extracted due to Other Causes",
            "Present Teeth",
            "Congenitally Missing",
            "Supernumerary"
        };

        public Patient Patient { get => patient;
            set
            {
                patient = value;
                DentalChartViewModel = new DentalChartViewModel();
                DentalChartViewModel.TeethView.Clear();
                DentalChartViewModel.User = ActiveUser;
                DentalChartViewModel.Patient = value;
                TreatmentRecords = null;
                treatmentRecordsViewModel.CustomFilter = "treatmentrecord_patient='" + value.No + "'";
                treatmentRecordsViewModel.loadTreatmentRecords();
                startLoadThread();
                OnPropertyChanged();
            }
        }
        public List<Patient> Patients { get => patients; set { patients = value; OnPropertyChanged(); } }
        public string Filter { get => filter; set { filter = value; OnPropertyChanged(); loadPatients(); } }

        public List<string> Conditions { get => conditions; set => conditions = value; }
        public string Condition { get => condition; set { condition = value; OnPropertyChanged(); } }

        public string Visibility { get => visibility; set { visibility = value; OnPropertyChanged(); } }

        public DentalChartViewModel DentalChartViewModel { get => dentalChartViewModel; set { dentalChartViewModel = value; OnPropertyChanged(); } }

        public int SelectedTooth { get => DentalChartViewModel.TeethView.Count; set { selectedTooth = value; OnPropertyChanged(); } }

        public List<TreatmentRecord> TreatmentRecords { get => treatmentRecords; set { treatmentRecords = value; OnPropertyChanged(); } }


        private Thread loadThread;

        public void startLoadThread()
        {
            if (loadThread == null || !loadThread.IsAlive)
            {
                loadThread = new Thread(setItemsSources);
                loadThread.IsBackground = true;
                loadThread.Start();
            }
        }

        public void setItemsSources()
        {
            while (TreatmentRecords == null)
            {
                TreatmentRecords = treatmentRecordsViewModel.TreatmentRecords;
            }
            Console.WriteLine(TreatmentRecords.Count+"---------------------------------------------");
        }

        public void setPatients(List<Patient> patients)
        {
            patients = this.patients;
        }

        public void loadPatients()
        {
            startLoadFromDatabase("allaboutteeth_" + GetType().Namespace.Replace("AllAboutTeethDCMS.", ""), Filter);
        }

        public void deleteUser()
        {
            startDeleteFromDatabase(Patient, "allaboutteeth_" + GetType().Namespace.Replace("AllAboutTeethDCMS.", ""));
        }

        protected override void setLoaded(List<Patient> list)
        {
            Patients = list;
        }

        protected override bool beforeUpdate()
        {
            throw new NotImplementedException();
        }

        protected override void afterUpdate()
        {
            throw new NotImplementedException();
        }

        protected override bool beforeSave()
        {
            throw new NotImplementedException();
        }

        protected override void afterSave()
        {
            throw new NotImplementedException();
        }
    }
}
