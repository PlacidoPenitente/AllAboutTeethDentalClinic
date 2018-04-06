using AllAboutTeethDCMS.DentalChart;
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
        #region Fields
        private Patient patient;
        private List<Patient> patients;

        private DelegateCommand loadChartCommand;

        private DelegateCommand loadCommand;
        private DelegateCommand archiveCommand;
        private DelegateCommand unarchiveCommand;
        private DelegateCommand deleteCommand;
        private DelegateCommand addCommand;
        private DelegateCommand editCommand;

        private string archiveVisibility = "Collapsed";
        private string unarchiveVisibility = "Collapsed";
        #endregion

        public PatientViewModel()
        {
            LoadCommand = new DelegateCommand(new Action(LoadPatients));
            ArchiveCommand = new DelegateCommand(new Action(Archive));
            UnarchiveCommand = new DelegateCommand(new Action(Unarchive));
            DeleteCommand = new DelegateCommand(new Action(DeletePatient));
            AddCommand = new DelegateCommand(new Action(GotoAddPatient));
            EditCommand = new DelegateCommand(new Action(GotoEditPatient));
            loadChartCommand = new DelegateCommand(new Action(loadChart));
            DentalChartPreviewViewModel = new DentalChartPreviewViewModel();
        }

        private DentalChartPreviewViewModel dentalChartPreviewViewModel;

        public void loadChart()
        {
            DentalChartPreviewViewModel.DentalChartViewModel.User = ActiveUser;
            DentalChartPreviewViewModel.Patient = Patient;
        }

        #region Methods
        protected override bool beforeUpdate()
        {
            DialogBoxViewModel.Answer = "None";
            DialogBoxViewModel.Mode = "Question";
            if (Patient.Status.Equals("Active"))
            {
                DialogBoxViewModel.Title = "Archive Patient";
                DialogBoxViewModel.Message = "Are you sure you want to archive this patient? Patient can no longer be scheduled.";
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
                LoadPatients();
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
                DialogBoxViewModel.Message = "Deleting patient. Please wait.";
                DialogBoxViewModel.Answer = "None";
                return true;
            }
            return false;
        }

        protected override void afterDelete(bool isSuccessful)
        {
            if (isSuccessful)
            {
                LoadPatients();
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

        protected override bool beforeCreate()
        {
            return true;
        }

        protected override void afterCreate(bool isSuccessful)
        {
        }

        protected override void beforeLoad(MySqlCommand command)
        {
            Patients = null;
            GC.Collect();
        }

        protected override void afterLoad(List<Patient> list)
        {
            Patients = list;
            FilterResult = "";
            if (list.Count > 1)
            {
                FilterResult = "Found " + list.Count + " result/s.";
            }
        }
        #endregion

        #region Properties
        public DelegateCommand LoadCommand { get => loadCommand; set => loadCommand = value; }
        public DelegateCommand ArchiveCommand { get => archiveCommand; set => archiveCommand = value; }
        public DelegateCommand UnarchiveCommand { get => unarchiveCommand; set => unarchiveCommand = value; }
        public DelegateCommand DeleteCommand { get => deleteCommand; set => deleteCommand = value; }
        public DelegateCommand AddCommand { get => addCommand; set => addCommand = value; }
        public DelegateCommand EditCommand { get => editCommand; set => editCommand = value; }

        public Patient Patient
        {
            get => patient;
            set
            {
                patient = value;
                OnPropertyChanged();

                ArchiveVisibility = "Collapsed";
                UnarchiveVisibility = "Collapsed";
                if (value != null)
                {
                    if(!value.Status.Equals("Scheduled"))
                    {
                        if (value.Status.Equals("Active"))
                        {
                            ArchiveVisibility = "Visible";
                        }
                        else
                        {
                            UnarchiveVisibility = "Visible";
                        }
                    }
                }
            }
        }
        public List<Patient> Patients { get => patients; set { patients = value; OnPropertyChanged(); } }

        public string ArchiveVisibility { get => archiveVisibility; set { archiveVisibility = value; OnPropertyChanged(); } }
        public string UnarchiveVisibility { get => unarchiveVisibility; set { unarchiveVisibility = value; OnPropertyChanged(); } }

        public DelegateCommand LoadChartCommand { get => loadChartCommand; set => loadChartCommand = value; }
        public DentalChartPreviewViewModel DentalChartPreviewViewModel { get => dentalChartPreviewViewModel; set => dentalChartPreviewViewModel = value; }
        #endregion

        #region Commands
        public void GotoAddPatient()
        {
            MenuViewModel.GotoAddPatientView();
        }

        public void LoadPatients()
        {
            startLoadFromDatabase("allaboutteeth_" + GetType().Namespace.Replace("AllAboutTeethDCMS.", ""), Filter);
        }

        public void GotoEditPatient()
        {
            MenuViewModel.GotoEditPatientView(Patient);
        }

        public void Archive()
        {
            startUpdateToDatabase(Patient, "allaboutteeth_" + GetType().Namespace.Replace("AllAboutTeethDCMS.", ""));
        }

        public void Unarchive()
        {
            startUpdateToDatabase(Patient, "allaboutteeth_" + GetType().Namespace.Replace("AllAboutTeethDCMS.", ""));
        }

        public void DeletePatient()
        {
            startDeleteFromDatabase(Patient, "allaboutteeth_" + GetType().Namespace.Replace("AllAboutTeethDCMS.", ""));
        }
        #endregion
    }
}
