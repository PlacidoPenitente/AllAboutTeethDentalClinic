using AllAboutTeethDCMS.DentalChart;
using AllAboutTeethDCMS.TreatmentRecords;
using MySql.Data.MySqlClient;
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
        private PatientPreviewViewModel patientPreviewViewModel;
        private DentalChartPreviewViewModel dentalChartPreviewViewModel;

        private DialogBoxViewModel dialogBoxViewModel;
        private string archiveVisibility = "Collapsed";
        private string unarchiveVisibility = "Collapsed";
        private string filterResult = "";

        public DialogBoxViewModel DialogBoxViewModel { get => dialogBoxViewModel; set { dialogBoxViewModel = value; OnPropertyChanged(); } }
        public string ArchiveVisibility { get => archiveVisibility; set { archiveVisibility = value; OnPropertyChanged(); } }
        public string UnarchiveVisibility { get => unarchiveVisibility; set { unarchiveVisibility = value; OnPropertyChanged(); } }
        public string FilterResult { get => filterResult; set { filterResult = value; OnPropertyChanged(); } }

        public void archive()
        {
            startUpdateToDatabase(Patient, "allaboutteeth_" + GetType().Namespace.Replace("AllAboutTeethDCMS.", ""));
        }

        public void unarchive()
        {
            startUpdateToDatabase(Patient, "allaboutteeth_" + GetType().Namespace.Replace("AllAboutTeethDCMS.", ""));
        }
        protected override bool beforeUpdate()
        {
            DialogBoxViewModel.Answer = "None";
            DialogBoxViewModel.Mode = "Question";
            if (Patient.Status.Equals("Active"))
            {
                DialogBoxViewModel.Title = "Archive Patient";
                DialogBoxViewModel.Message = "Are you sure you want to archive this patient? Patient can no longer be set for appointment.";
            }
            else
            {
                DialogBoxViewModel.Title = "Unarchive Patient";
                DialogBoxViewModel.Message = "Are you sure you want to activate this patient?";
            }
            while (DialogBoxViewModel.Answer.Equals("None"))
            {
                Thread.Sleep(100);
            }
            if (DialogBoxViewModel.Answer.Equals("Yes"))
            {
                if (Patient.Status.Equals("Active"))
                {
                    Patient.Status = "Archived";
                    DialogBoxViewModel.Mode = "Progress";
                    DialogBoxViewModel.Message = "Archiving patient. Please wait.";
                    DialogBoxViewModel.Answer = "None";
                }
                else
                {
                    Patient.Status = "Active";
                    DialogBoxViewModel.Mode = "Progress";
                    DialogBoxViewModel.Message = "Activating patient. Please wait.";
                    DialogBoxViewModel.Answer = "None";
                }
                return true;
            }
            return false;
        }

        protected override void afterUpdate(bool isSuccessful)
        {
            if (isSuccessful)
            {
                DialogBoxViewModel.Mode = "Success";
                DialogBoxViewModel.Message = "Operation completed.";
                DialogBoxViewModel.Answer = "None";
                loadPatients();
            }
            else
            {
                DialogBoxViewModel.Mode = "Error";
                DialogBoxViewModel.Message = "Operation failed.";
                DialogBoxViewModel.Answer = "None";
            }
            while (DialogBoxViewModel.Answer.Equals("None"))
            {
                Thread.Sleep(100);
            }
            DialogBoxViewModel.Answer = "";
        }

        protected override bool beforeDelete()
        {
            DialogBoxViewModel.Answer = "None";
            DialogBoxViewModel.Mode = "Question";
            DialogBoxViewModel.Title = "Delete Patient";
            DialogBoxViewModel.Message = "Are you sure you want to totally delete this patient?";

            while (DialogBoxViewModel.Answer.Equals("None"))
            {
                Thread.Sleep(100);
            }

            if (DialogBoxViewModel.Answer.Equals("Yes"))
            {
                DialogBoxViewModel.Mode = "Progress";
                DialogBoxViewModel.Message = "Deleteing patient. Please wait.";
                DialogBoxViewModel.Answer = "None";
                return true;
            }
            return false;
        }

        protected override void afterDelete(bool isSuccessful)
        {
            if (isSuccessful)
            {
                loadPatients();
                DialogBoxViewModel.Mode = "Success";
                DialogBoxViewModel.Message = "Operation completed.";
                DialogBoxViewModel.Answer = "None";
            }
            else
            {
                DialogBoxViewModel.Mode = "Error";
                DialogBoxViewModel.Message = "Operation failed.";
                DialogBoxViewModel.Answer = "None";
            }
            while (DialogBoxViewModel.Answer.Equals("None"))
            {
                Thread.Sleep(100);
            }
            DialogBoxViewModel.Answer = "";
        }

        public PatientViewModel()
        {
            DialogBoxViewModel = new DialogBoxViewModel();
            PatientPreviewViewModel = new PatientPreviewViewModel();
            DentalChartPreviewViewModel = new DentalChartPreviewViewModel();
        }

        public Patient Patient
        {
            get => patient; set
            {
                patient = value; OnPropertyChanged();
                ArchiveVisibility = "Collapsed";
                UnarchiveVisibility = "Collapsed";
                if (value != null)
                {
                    if (value.Status.Equals("Active"))
                    {
                        ArchiveVisibility = "Visible";
                    }
                    else
                    {
                        UnarchiveVisibility = "Visible";
                    }
                    PatientPreviewViewModel.Patient = value;
                    DentalChartPreviewViewModel.DentalChartViewModel.User = ActiveUser;
                    DentalChartPreviewViewModel.Patient = value;
                }
            }
        }

        public List<Patient> Patients { get => patients; set { patients = value; OnPropertyChanged(); } }
        public string Filter { get => filter; set { filter = value; loadPatients(); OnPropertyChanged(); } }

        public PatientPreviewViewModel PatientPreviewViewModel { get => patientPreviewViewModel; set { patientPreviewViewModel = value; OnPropertyChanged(); } }

        public DentalChartPreviewViewModel DentalChartPreviewViewModel { get => dentalChartPreviewViewModel; set { dentalChartPreviewViewModel = value; OnPropertyChanged(); } }

        public void loadPatients()
        {
            startLoadFromDatabase("allaboutteeth_" + GetType().Namespace.Replace("AllAboutTeethDCMS.", ""), Filter);
        }

        public void deletePatient()
        {
            startDeleteFromDatabase(Patient, "allaboutteeth_" + GetType().Namespace.Replace("AllAboutTeethDCMS.", ""));
        }

        protected override void afterLoad(List<Patient> list)
        {
            Patients = list;
            FilterResult = "";
            if (list.Count > 0)
            {
                FilterResult = "Found " + list.Count + " result/s.";
            }
        }

        protected override bool beforeCreate()
        {
            return true;
        }

        protected override void afterCreate(bool isSuccessful)
        {
            throw new NotImplementedException();
        }

        protected override void beforeLoad(MySqlCommand command)
        {
        }
    }
}
