using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Windows;

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

        #endregion Fields

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

        public ObservableCollection<AppointmentGroup> AllAppointments
        {
            get => _allAppointments;
            set
            {
                _allAppointments = value;
                OnPropertyChanged();
            }
        }

        private Appointment _appointmentDelete;

        public Appointment AppointmentDelete
        {
            get { return _appointmentDelete; }
            set
            {
                _appointmentDelete = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<Appointment> UniqueAppointment
        {
            get => _uniqueAppointment;
            set
            {
                _uniqueAppointment = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<Appointment> IndividualAppointments { get; set; }

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
                UniqueAppointment = null;
                Appointments = null;
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
            if (ActiveUser.Type.Equals("Administrator") || ActiveUser.Type.Equals("Staff"))
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
            var keys = list.Select(x => x.Patient.No).ToList();
            keys = keys.Distinct().ToList();
            var temp = new List<Appointment>();
            foreach (var key in keys)
            {
                temp.Add(list.FirstOrDefault(x => x.Patient.No == key));
            }
            Appointments = temp;
            IndividualAppointments = new ObservableCollection<Appointment>(list);
            FilterResult = "";
            if (list.Count > 1)
            {
                FilterResult = "Found " + list.Count + " result/s.";
            }

            var allAppointments = new ObservableCollection<AppointmentGroup>();
            var allDentists = list.Select(x => x.Dentist.No);
            var uniqueDentist = allDentists.Distinct();
            //foreach (var dentist in uniqueDentist)
            //{
            //    allAppointments.Add(new AppointmentGroup
            //    {
            //        Dentist = list.FirstOrDefault(x => x.Dentist.No == dentist)?.Dentist,

            //        Session = new Session(this)
            //        {
            //            Time = list.FirstOrDefault(x => x.Dentist.No == dentist)?.Schedule.ToShortTimeString(),
            //            Patient = list.FirstOrDefault(x => x.Dentist.No == dentist)?.Patient,
            //            Appointments = new ObservableCollection<Appointment>(list.Where(x => x.Dentist.No == dentist))
            //        }
            //    });
            //}

            foreach (var dentist in uniqueDentist)
            {
                var patients = list.Where(x => x.Dentist.No == dentist).Select(x => x.Patient.No);
                allAppointments.Add(new AppointmentGroup
                {
                    AppointmentViewModel = this,
                    Dentist = list.FirstOrDefault(x => x.Dentist.No == dentist)?.Dentist
                });

                var uniquePatients = patients.Distinct();

                foreach (var patient in uniquePatients)
                {
                    var appointmentGroup = allAppointments.FirstOrDefault(x => x.Dentist.No == dentist);
                    if (appointmentGroup != null && appointmentGroup.Sessions == null)
                        appointmentGroup.Sessions = new ObservableCollection<Session>();
                    appointmentGroup?.Sessions.Add(new Session
                    {
                        Patient = list.FirstOrDefault(x => x.Patient.No == patient)?.Patient,
                        Appointments = new ObservableCollection<Appointment>(list.Where(x =>
                            x.Patient.No == patient && x.Dentist.No == dentist)),
                        Date = list.FirstOrDefault(x => x.Patient.No == patient && x.Dentist.No == dentist)
                            ?.Schedule.ToLongDateString(),
                        Time = list.FirstOrDefault(x => x.Patient.No == patient && x.Dentist.No == dentist)
                            ?.Schedule.ToShortTimeString() + " - " + list.FirstOrDefault(x => x.Patient.No == patient && x.Dentist.No == dentist)
                                   ?.Schedule.AddMinutes(list.Where(x =>
                                       x.Patient.No == patient && x.Dentist.No == dentist).Sum(x => x.Treatment.Duration)).ToShortTimeString()
                    });
                }
            }

            AllAppointments = allAppointments;
        }

        #endregion Methods

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
        private ObservableCollection<Appointment> _uniqueAppointment;

        public Appointment Appointment
        {
            get => appointment;
            set
            {
                appointment = null;
                appointment = value;
                if (appointment != null)
                {
                    UniqueAppointment = new ObservableCollection<Appointment>(IndividualAppointments.Where(x => x.Patient.No == value.Patient.No));
                    EndDate = value.Schedule.AddMinutes(UniqueAppointment.Sum(x => x.Treatment.Duration));
                }
                OnPropertyChanged();

                ArchiveVisibility = "Collapsed";
                UnarchiveVisibility = "Collapsed";
                ForAdminAndStaff = "Collapsed";
                ForAdminAndDenstist = "Collapsed";
                if (ActiveUser.Type.Equals("Administrator") || ActiveUser.Type.Equals("Staff"))
                {
                    ForAdminAndStaff = "Visible";
                }

                if (value != null && (ActiveUser.Type.Equals("Administrator") || ActiveUser.Type.Equals("Dentist")))
                {
                    if (value.Dentist.No == ActiveUser.No)
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

        #endregion Properties

        #region Commands

        public void GotoAddOperation()
        {
            if (ActiveUser.Type.Equals("Dentist") || ActiveUser.Type.Equals("Administrator"))
            {
                MenuViewModel.gotoAddOperationView(Appointment, IndividualAppointments.ToList());
            }
            else
            {
                MessageBox.Show("You're not allowed to perform this operation.", "Not Allowed", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private DateTime _endDate;
        private ObservableCollection<AppointmentGroup> _allAppointments;
        private Session _session;

        public DateTime EndDate
        {
            get { return _endDate; }
            set
            {
                _endDate = value;
                OnPropertyChanged();
            }
        }

        public Session Session
        {
            get => _session;
            set
            {
                foreach (var appointmentGroup in AllAppointments)
                {
                    appointmentGroup.SelectedSessionAlternate = null;
                }
                _session = value;
                Appointment = _session.Appointments.FirstOrDefault();
                OnPropertyChanged();
            }
        }

        public void GotoAddAppointment()
        {
            MenuViewModel.GotoAddAppointmentView(this);
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
            StartDeleteSession(Session, "allaboutteeth_" + GetType().Namespace.Replace("AllAboutTeethDCMS.", ""));
            //startDeleteFromDatabase(Appointment, "allaboutteeth_" + GetType().Namespace.Replace("AllAboutTeethDCMS.", ""));
        }

        #endregion Commands
    }
}