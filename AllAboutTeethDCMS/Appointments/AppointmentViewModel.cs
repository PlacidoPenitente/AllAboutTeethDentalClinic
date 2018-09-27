using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using MySql.Data.MySqlClient;

namespace AllAboutTeethDCMS.Appointments
{
    public class AppointmentViewModel : CRUDPage<Appointment>
    {
        #region Fields
        private Appointment appointment;
        private List<Appointment> appointments;

        private string operateVisibility = "Visible";

        private DelegateCommand loadCommand;
        private DelegateCommand archiveCommand;
        private DelegateCommand unarchiveCommand;
        private DelegateCommand deleteCommand;
        private DelegateCommand addCommand;
        private DelegateCommand editCommand;
        private DelegateCommand treatmentCommand;

        private string archiveVisibility = "Collapsed";
        private string unarchiveVisibility = "Collapsed";
        #endregion

        public AppointmentViewModel()
        {
            LoadCommand = new DelegateCommand(new Action(LoadAppointments));
            ArchiveCommand = new DelegateCommand(new Action(Archive));
            UnarchiveCommand = new DelegateCommand(new Action(Unarchive));
            DeleteCommand = new DelegateCommand(new Action(DeleteAppointment));
            AddCommand = new DelegateCommand(new Action(GotoAddAppointment));
            EditCommand = new DelegateCommand(new Action(GotoEditAppointment));
            TreatmentCommand = new DelegateCommand(new Action(GotoAddOperation));
        }

        #region Methods
        protected override bool beforeUpdate()
        {
            DialogBoxViewModel.Answer = "None";
            DialogBoxViewModel.Mode = "Question";
            if (Appointment.Status.Equals("Active"))
            {
                DialogBoxViewModel.Title = "Archive Appointment";
                DialogBoxViewModel.Message = "Are you sure you want to archive this appointment?";
            }
            else
            {
                DialogBoxViewModel.Title = "Unarchive Appointment";
                DialogBoxViewModel.Message = "Are you sure you want to activate this appointment?";
            }
            while (DialogBoxViewModel.Answer.Equals("None"))
            {
                Thread.Sleep(100);
            }
            if (DialogBoxViewModel.Answer.Equals("Yes"))
            {
                if (Appointment.Status.Equals("Active"))
                {
                    Appointment.Status = "Archived";
                    DialogBoxViewModel.Mode = "Progress";
                    DialogBoxViewModel.Message = "Archiving appointment. Please wait.";
                    DialogBoxViewModel.Answer = "None";
                }
                else
                {
                    Appointment.Status = "Active";
                    DialogBoxViewModel.Mode = "Progress";
                    DialogBoxViewModel.Message = "Activating appointment. Please wait.";
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
                LoadAppointments();
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
            DialogBoxViewModel.Title = "Delete Appointment";
            DialogBoxViewModel.Message = "Are you sure you want to totally delete this appointment?";

            while (DialogBoxViewModel.Answer.Equals("None"))
            {
                Thread.Sleep(100);
            }

            if (DialogBoxViewModel.Answer.Equals("Yes"))
            {
                DialogBoxViewModel.Mode = "Progress";
                DialogBoxViewModel.Message = "Deleting appointment. Please wait.";
                DialogBoxViewModel.Answer = "None";
                return true;
            }
            return false;
        }

        protected override void afterDelete(bool isSuccessful)
        {
            if (isSuccessful)
            {
                DialogBoxViewModel.Mode = "Success";
                DialogBoxViewModel.Message = "Operation completed.";
                DialogBoxViewModel.Answer = "None";

                MySqlConnection connection = CreateConnection();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "update allaboutteeth_patients set patient_status='Active', patient_addedby='" + ActiveUser.No + "' where patient_no='" + Appointment.Patient.No + "'";
                command.ExecuteNonQuery();
                connection.Close();
                connection = null;

                LoadAppointments();
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
            command.Parameters.Clear();
            if(ActiveUser.Type.Equals("Administrator")||ActiveUser.Type.Equals("Staff"))
            {
                command.CommandText = "select * from allaboutteeth_appointments where appointment_status=@status";
                command.Parameters.AddWithValue("@status", "Pending");
            }
            else
            {
                command.CommandText = "select * from allaboutteeth_appointments where appointment_status=@status && appointment_dentist=@dentist";
                command.Parameters.AddWithValue("@status", "Pending");
                command.Parameters.AddWithValue("@dentist", ActiveUser.No);
            }
            
        }

        protected override void afterLoad(List<Appointment> list)
        {
            Appointment = null;
            Appointments = list;
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
        public DelegateCommand TreatmentCommand { get => treatmentCommand; set => treatmentCommand = value; }

        private string forAdminAndStaff = "Collapsed";
        private string forAdminAndDenstist = "Collapsed";

        public Appointment Appointment
        {
            get => appointment;
            set
            {
                appointment = null;
                appointment = value;
                OnPropertyChanged();

                ArchiveVisibility = "Collapsed";
                UnarchiveVisibility = "Collapsed";
                ForAdminAndStaff = "Collapsed";
                ForAdminAndDenstist = "Collapsed";
                if (ActiveUser.Type.Equals("Administrator")||ActiveUser.Type.Equals("Staff"))
                {
                    ForAdminAndStaff = "Visible";
                }

                if(value != null && (ActiveUser.Type.Equals("Administrator") || ActiveUser.Type.Equals("Dentist")))
                {
                    if(value.Dentist.No==ActiveUser.No)
                    {
                        ForAdminAndDenstist = "Visible";
                    }
                }
            }
        }
        public List<Appointment> Appointments { get => appointments; set { appointments = value; OnPropertyChanged(); } }

        public string ArchiveVisibility { get => archiveVisibility; set { archiveVisibility = value; OnPropertyChanged(); } }
        public string UnarchiveVisibility { get => unarchiveVisibility; set { unarchiveVisibility = value; OnPropertyChanged(); } }

        public string OperateVisibility { get => operateVisibility; set { operateVisibility = value; OnPropertyChanged(); } }

        public string ForAdminAndStaff { get => forAdminAndStaff; set { forAdminAndStaff = value; OnPropertyChanged(); } }

        public string ForAdminAndDenstist { get => forAdminAndDenstist; set { forAdminAndDenstist = value; OnPropertyChanged(); } }

        #endregion

        #region Commands
        public void GotoAddOperation()
        {
            if(ActiveUser.Type.Equals("Dentist")|| ActiveUser.Type.Equals("Administrator"))
            {
                MenuViewModel.gotoAddOperationView(Appointment, Appointments);
            }
            else
            {
                MessageBox.Show("You're not allowed to perform this operation.", "Not Allowed", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void GotoAddAppointment()
        {
            MenuViewModel.GotoAddAppointmentView();
        }

        public void LoadAppointments()
        {
            startLoadFromDatabase("allaboutteeth_" + GetType().Namespace.Replace("AllAboutTeethDCMS.", ""), Filter);
        }

        public void GotoEditAppointment()
        {
            MenuViewModel.GotoEditAppointmentView(Appointment);
        }

        public void Archive()
        {
            startUpdateToDatabase(Appointment, "allaboutteeth_" + GetType().Namespace.Replace("AllAboutTeethDCMS.", ""));
        }

        public void Unarchive()
        {
            startUpdateToDatabase(Appointment, "allaboutteeth_" + GetType().Namespace.Replace("AllAboutTeethDCMS.", ""));
        }

        public void DeleteAppointment()
        {
            startDeleteFromDatabase(Appointment, "allaboutteeth_" + GetType().Namespace.Replace("AllAboutTeethDCMS.", ""));
        }
        #endregion
    }
}
