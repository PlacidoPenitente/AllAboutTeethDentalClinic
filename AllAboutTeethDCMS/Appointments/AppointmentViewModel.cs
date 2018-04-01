using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace AllAboutTeethDCMS.Appointments
{
    public class AppointmentViewModel : CRUDPage<Appointment>
    {
        #region Fields
        private Appointment appointment;
        private List<Appointment> appointments;

        private DelegateCommand loadCommand;
        private DelegateCommand archiveCommand;
        private DelegateCommand unarchiveCommand;
        private DelegateCommand deleteCommand;
        private DelegateCommand addCommand;
        private DelegateCommand editCommand;

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
                LoadAppointments();
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
        }

        protected override void afterLoad(List<Appointment> list)
        {
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

        public Appointment Appointment
        {
            get => appointment;
            set
            {
                appointment = value;
                OnPropertyChanged();

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
                }
            }
        }
        public List<Appointment> Appointments { get => appointments; set { appointments = value; OnPropertyChanged(); } }

        public string ArchiveVisibility { get => archiveVisibility; set { archiveVisibility = value; OnPropertyChanged(); } }
        public string UnarchiveVisibility { get => unarchiveVisibility; set { unarchiveVisibility = value; OnPropertyChanged(); } }
        #endregion

        #region Commands
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
