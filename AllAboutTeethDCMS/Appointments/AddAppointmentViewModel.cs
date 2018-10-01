using AllAboutTeethDCMS.ActivityLogs;
using AllAboutTeethDCMS.DentalChart;
using AllAboutTeethDCMS.Patients;
using AllAboutTeethDCMS.Treatments;
using AllAboutTeethDCMS.Users;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Threading;

namespace AllAboutTeethDCMS.Appointments
{
    public class AddAppointmentViewModel : CRUDPage<Appointment>
    {
        private Appointment appointment;
        private Appointment copyAppointment;
        private List<Patient> patients;
        private List<Treatment> treatments;
        private List<User> dentists;
        private string filter = "";
        private DentalChartViewModel dentalChartViewModel;

        public ObservableCollection<Treatment> SelectedTreatments { get; set; }
        public DelegateCommand AddTreatmentCommand { get; set; }
        public DelegateCommand RemoveTreatmentCommand { get; set; }
        public Treatment SelectedTreament { get; set; }

        public void AddTreatment()
        {
            if (SelectedTreatments.All(x => x.No != Appointment.Treatment.No))
                SelectedTreatments.Add(Appointment.Treatment);
        }

        public void RemoveTreatment()
        {
            if (SelectedTreament != null)
                if (SelectedTreatments.Any(x => x.No == SelectedTreament.No))
                    SelectedTreatments.Remove(SelectedTreament);
        }

        private PatientViewModel patientViewModel;
        private TreatmentViewModel treatmentViewModel;
        private UserViewModel userViewModel;

        private Thread loadTreatmentsThread;

        public DelegateCommand SearchPatientCommand { get; set; }

        private void SearchPatient()
        {
            PatientViewModel.Filter = Filter;
            PatientViewModel.Patients = null;
            startLoadPatientsThread();
        }

        public void startLoadTreatmentsThread()
        {
            if (SelectedTreatments != null) SelectedTreatments.Clear();
            if (loadTreatmentsThread == null || !loadTreatmentsThread.IsAlive)
            {
                loadTreatmentsThread = new Thread(setTreatments);
                loadTreatmentsThread.IsBackground = true;
                loadTreatmentsThread.Start();
                TreatmentViewModel.LoadTreatments();
            }
        }

        public void setTreatments()
        {
            while (TreatmentViewModel.Treatments == null)
            {
                Thread.Sleep(100);
            }
            Treatments = null;
            Treatments = new List<Treatment>();
            foreach (Treatment treatment in TreatmentViewModel.Treatments)
            {
                if (treatment.Status.Equals("Active"))
                {
                    Treatments.Add(treatment);
                    OnPropertyChanged("Treatments");
                }
            }
        }

        private Thread loadPatientsThread;

        public void startLoadPatientsThread()
        {
            if (loadPatientsThread == null || !loadPatientsThread.IsAlive)
            {
                loadPatientsThread = new Thread(setPatients);
                loadPatientsThread.IsBackground = true;
                loadPatientsThread.Start();
                PatientViewModel.LoadPatients();
            }
        }

        public void setPatients()
        {
            while (PatientViewModel.Patients == null)
            {
                Thread.Sleep(100);
            }
            Patients = null;
            Patients = new List<Patient>();
            foreach (Patient patient in PatientViewModel.Patients)
            {
                if (patient.Status.Equals("Active"))
                {
                    Patients.Add(patient);
                    OnPropertyChanged("Patients");
                }
            }
        }

        public void Load()
        {
            startLoadFromDatabase("allaboutteeth_" + GetType().Namespace.Replace("AllAboutTeethDCMS.", ""), Filter);
        }

        private Thread loadUsersThread;

        public void startLoadUsersThread()
        {
            if (loadUsersThread == null || !loadUsersThread.IsAlive)
            {
                loadUsersThread = new Thread(setUsers);
                loadUsersThread.IsBackground = true;
                loadUsersThread.Start();
                UserViewModel.LoadUsers();
            }
        }

        public void setUsers()
        {
            while (UserViewModel.Users == null)
            {
                Thread.Sleep(100);
            }
            Dentists = null;
            Dentists = new List<User>();
            foreach (User user in UserViewModel.Users)
            {
                if (user.Status.Equals("Active") && (user.Type.Equals("Dentist") || user.Type.Equals("Administrator")))
                {
                    Dentists.Add(user);
                    OnPropertyChanged("Dentists");
                }
            }
        }

        public AddAppointmentViewModel()
        {
            dentalChartViewModel = new DentalChartViewModel();
            dentalChartViewModel.TreatmentRecordViewModel = null;

            PatientViewModel = new PatientViewModel();
            TreatmentViewModel = new TreatmentViewModel();
            UserViewModel = new UserViewModel();

            Appointment = new Appointment();
            CopyAppointment = (Appointment)appointment.Clone();

            DialogBoxViewModel = new DialogBoxViewModel();

            SearchPatientCommand = new DelegateCommand(SearchPatient);

            startLoadTreatmentsThread();
            startLoadPatientsThread();
            startLoadUsersThread();
            startLoadFromDatabase("allaboutteeth_" + GetType().Namespace.Replace("AllAboutTeethDCMS.", ""), Filter);

            SelectedTreatments = new ObservableCollection<Treatment>();

            AddTreatmentCommand = new DelegateCommand(AddTreatment);
            RemoveTreatmentCommand = new DelegateCommand(RemoveTreatment);
            SetTimes();
        }

        public DateTime Schedule
        {
            get { return Appointment.Schedule; }
            set
            {
                var now = DateTime.Now;
                if (now.ToShortDateString().Equals(value.ToShortDateString()))
                {
                    var temp = now.Hour > 12 ? now.Hour - 12 : now.Hour;
                    var hour = temp < 10 ? "0" + temp : "" + temp;
                    temp = now.Minute > 12 ? now.Minute - 12 : now.Minute;
                    var minute = now.Minute < 10 ? "0" + temp : "" + temp;
                    var meridiem = now.Hour < 12 ? "AM" : "PM";
                    Appointment.Schedule = DateTime.Parse(value.ToShortDateString() + " " + hour + ":" + minute + ":00 " + meridiem);
                }
                else
                {
                    Appointment.Schedule = value;
                }
                SetTimes();
                OnPropertyChanged();
            }
        }

        public Patient Patient { get => Appointment.Patient; set { Appointment.Patient = value; DentalChartViewModel.Treatment = Treatment; DentalChartViewModel.TeethView.Clear(); DentalChartViewModel = new DentalChartViewModel() { TreatmentRecordViewModel = null }; DentalChartViewModel.User = ActiveUser; DentalChartViewModel.Patient = value; OnPropertyChanged(); } }
        public Treatment Treatment { get => Appointment.Treatment; set { Appointment.Treatment = value; OnPropertyChanged(); } }
        public User Dentist { get => Appointment.Dentist; set { Appointment.Dentist = value; OnPropertyChanged(); } }

        public string Notes
        {
            get
            {
                if (Patient != null)
                {
                    return Patient.Reason;
                }
                return "";
            }
            set
            {
                Patient.Reason = value;
                OnPropertyChanged();
            }
        }

        public Appointment Appointment
        {
            get => appointment; set
            {
                appointment = value; OnPropertyChanged();
                foreach (PropertyInfo info in GetType().GetProperties())
                {
                    OnPropertyChanged(info.Name);
                }
            }
        }

        public List<Patient> Patients
        {
            get
            {
                return patients;
            }
            set { patients = value; OnPropertyChanged(); }
        }

        public List<Treatment> Treatments
        {
            get
            {
                return treatments;
            }
            set { treatments = value; OnPropertyChanged(); }
        }

        public List<User> Dentists
        {
            get
            {
                return dentists;
            }
            set { dentists = value; OnPropertyChanged(); }
        }

        public ObservableCollection<string> Time { get; set; } = new ObservableCollection<string>();

        public void SetTimes()
        {
            Start = DateTime.Now;
            Time.Clear();

            var hour = 8;
            var minute = 0;
            while (hour < 21)
            {
                var temp = hour > 12 ? hour - 12 : hour;
                var time = DateTime.Parse(Schedule.ToShortDateString() + " " +
                    (temp < 10 ? "0" + temp : "" + temp) + ":" +
                    ((minute < 10) ? "0" + minute : "" + minute) + ":00 " +
                    (hour < 12 ? "AM" : "PM"));
                if (DateTime.Compare(time, Schedule) > 0)
                {
                    var a = DateTime.Compare(time, Schedule);
                    if (time.Hour > 12)
                        Time.Add((time.Hour - 12) < 10 ? "0" + time.ToShortTimeString() : "" + time.ToShortTimeString());
                    else
                        Time.Add(time.Hour < 10 ? "0" + time.ToShortTimeString() : "" + time.ToShortTimeString());
                }
                time = time.AddMinutes(15);
                hour = time.Hour;
                minute = time.Minute;
            }
            ScheduleTime = Time.FirstOrDefault();
        }

        private string _scheduleTime = "08:00 AM";

        public string ScheduleTime
        {
            get { return _scheduleTime; }
            set
            {
                _scheduleTime = value;
                OnPropertyChanged();
            }
        }

        public string Filter { get => filter; set { filter = value; OnPropertyChanged(); } }

        public DentalChartViewModel DentalChartViewModel { get => dentalChartViewModel; set { dentalChartViewModel = value; OnPropertyChanged(); } }

        public Appointment CopyAppointment { get => copyAppointment; set => copyAppointment = value; }
        public PatientViewModel PatientViewModel { get => patientViewModel; set => patientViewModel = value; }
        public TreatmentViewModel TreatmentViewModel { get => treatmentViewModel; set => treatmentViewModel = value; }
        public UserViewModel UserViewModel { get => userViewModel; set => userViewModel = value; }

        private Thread loadDialogThread;

        public void startLoadDialogThread()
        {
            if (loadDialogThread == null || !loadDialogThread.IsAlive)
            {
                loadDialogThread = new Thread(showDialog);
                loadDialogThread.IsBackground = true;
                loadDialogThread.Start();
            }
        }

        public void showDialog()
        {
            DialogBoxViewModel.Answer = "None";
            while (DialogBoxViewModel.Answer.Equals("None"))
            {
                Thread.Sleep(100);
            }
            DialogBoxViewModel.Answer = "";
        }

        protected override bool beforeCreate()
        {
            DialogBoxViewModel.Answer = "None";
            DialogBoxViewModel.Mode = "Question";
            DialogBoxViewModel.Title = "Add Appointment";
            DialogBoxViewModel.Message = "Are you sure you want to add this appointment?";

            while (DialogBoxViewModel.Answer.Equals("None"))
            {
                Thread.Sleep(100);
            }

            if (DialogBoxViewModel.Answer.Equals("Yes"))
            {
                DialogBoxViewModel.Mode = "Progress";
                DialogBoxViewModel.Message = "Adding appointment. Please wait.";
                DialogBoxViewModel.Answer = "None";
                return true;
            }
            return false;
        }

        protected override void afterCreate(bool isSuccessful)
        {
            if (isSuccessful)
            {
                DialogBoxViewModel.Mode = "Success";
                DialogBoxViewModel.Message = "Operation completed.";
                DialogBoxViewModel.Answer = "None";
                while (DialogBoxViewModel.Answer.Equals("None"))
                {
                    Thread.Sleep(100);
                }
                DialogBoxViewModel.Answer = "";

                MySqlConnection connection = CreateConnection();
                MySqlCommand command = connection.CreateCommand();
                command.CommandText = "update allaboutteeth_patients set patient_status='Scheduled', patient_addedby='" + ActiveUser.No + "' where patient_no='" + Patient.No + "'";
                command.ExecuteNonQuery();
                connection.Close();
                connection = null;

                PatientViewModel.Patients = null;
                startLoadPatientsThread();

                Appointment = new Appointment();
                CopyAppointment = (Appointment)Appointment.Clone();
            }
            else
            {
                DialogBoxViewModel.Mode = "Error";
                DialogBoxViewModel.Message = "Operation failed.";
                DialogBoxViewModel.Answer = "None";
                while (DialogBoxViewModel.Answer.Equals("None"))
                {
                    Thread.Sleep(100);
                }
                DialogBoxViewModel.Answer = "";
            }
            MenuViewModel.AppointmentViewModel = AppointmentViewModel;
            MenuViewModel.GobackToAppointments();
        }

        protected override bool beforeUpdate()
        {
            DialogBoxViewModel.Answer = "None";
            DialogBoxViewModel.Mode = "Question";
            DialogBoxViewModel.Title = "Update Appointment";
            DialogBoxViewModel.Message = "Are you sure you want to update this appointment?";

            while (DialogBoxViewModel.Answer.Equals("None"))
            {
                Thread.Sleep(100);
            }

            if (DialogBoxViewModel.Answer.Equals("Yes"))
            {
                DialogBoxViewModel.Mode = "Progress";
                DialogBoxViewModel.Message = "Updating appointment. Please wait.";
                DialogBoxViewModel.Answer = "None";
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
                while (DialogBoxViewModel.Answer.Equals("None"))
                {
                    Thread.Sleep(100);
                }
                DialogBoxViewModel.Answer = "";
                CopyAppointment = (Appointment)Appointment.Clone();

                AddActivityLogViewModel addActivityLog = new AddActivityLogViewModel();
                addActivityLog.ActivityLog = new ActivityLog();
                addActivityLog.ActiveUser = ActiveUser;
                addActivityLog.ActivityLog.Activity = "User created a new appointment for " + Dentist.FirstName + " " + Dentist.LastName + ".";
                addActivityLog.saveActivityLog();
            }
            else
            {
                DialogBoxViewModel.Mode = "Error";
                DialogBoxViewModel.Message = "Operation failed.";
                DialogBoxViewModel.Answer = "None";
                while (DialogBoxViewModel.Answer.Equals("None"))
                {
                    Thread.Sleep(100);
                }
                DialogBoxViewModel.Answer = "";
            }
        }

        public DateTime Start
        {
            get => _start;
            set
            {
                _start = value;
                OnPropertyChanged();
            }
        }

        public virtual void saveAppointment()
        {
            if (Patient == null)
            {
                DialogBoxViewModel.Mode = "Error";
                DialogBoxViewModel.Title = "Patient Error";
                DialogBoxViewModel.Message = "No patient was selected.";
                startLoadDialogThread();
                return;
            }
            if (Dentist == null)
            {
                DialogBoxViewModel.Mode = "Error";
                DialogBoxViewModel.Title = "Dentist Error";
                DialogBoxViewModel.Message = "No dentist was selected.";
                startLoadDialogThread();
                return;
            }
            if (SelectedTreatments.Count < 1)
            {
                DialogBoxViewModel.Mode = "Error";
                DialogBoxViewModel.Title = "Treatment Error";
                DialogBoxViewModel.Message = "No treatment was selected.";
                startLoadDialogThread();
                return;
            }

            if (ScheduleTime == null)
            {
                DialogBoxViewModel.Mode = "Error";
                DialogBoxViewModel.Title = "Schedule Error";
                DialogBoxViewModel.Message = "Please select a time.";
                startLoadDialogThread();
                return;
            }

            Appointment.Schedule = DateTime.Parse(Appointment.Schedule.ToShortDateString() + " " + ScheduleTime);

            if (DateTime.Compare(Appointment.Schedule, DateTime.Now) < 0)
            {
                DialogBoxViewModel.Mode = "Error";
                DialogBoxViewModel.Title = "Schedule Error";
                DialogBoxViewModel.Message = "Past dates cannot be selected.";
                startLoadDialogThread();
                return;
            }
            var earlierAppointmentDuration = 0;
            var earlierAppointment = Appointments.LastOrDefault(x => DateTime.Compare(x.Schedule, Appointment.Schedule) <= 0 && x.Status.Equals("Pending") && x.Dentist.No == Dentist.No);
            if (earlierAppointment != null)
            {
                earlierAppointmentDuration = Appointments.Where(x => x.Patient.No == earlierAppointment.Patient.No && x.Status.Equals("Pending")).Sum(x => x.Treatment.Duration);

                if (DateTime.Compare(earlierAppointment.Schedule.AddMinutes(earlierAppointmentDuration), Appointment.Schedule) > 0)
                {
                    var d1 = earlierAppointment.Schedule.AddMinutes(earlierAppointmentDuration);
                    var d2 = Appointment.Schedule;
                    DialogBoxViewModel.Mode = "Error";
                    DialogBoxViewModel.Title = "Schedule Error";
                    DialogBoxViewModel.Message = "Schedule is already taken.";
                    startLoadDialogThread();
                    return;
                }
            }

            var currentAppointmentDuration = SelectedTreatments.Sum(x => x.Duration);
            earlierAppointment = Appointments.LastOrDefault(x =>
                DateTime.Compare(x.Schedule, Appointment.Schedule.AddMinutes(currentAppointmentDuration)) < 0 &&
                DateTime.Compare(x.Schedule, Appointment.Schedule) >= 0 &&
                x.Status.Equals("Pending") &&
                x.Dentist.No == Dentist.No);

            if (earlierAppointment != null)
            {
                DialogBoxViewModel.Mode = "Error";
                DialogBoxViewModel.Title = "Schedule Error";
                DialogBoxViewModel.Message = "Schedule conflicts with next appointment.";
                startLoadDialogThread();
                return;
            }

            bool hasError = false;
            foreach (PropertyInfo info in GetType().GetProperties())
            {
                if (info.Name.EndsWith("Error"))
                {
                    if (!((string)info.GetValue(this)).Equals(""))
                    {
                        hasError = true;
                        break;
                    }
                }
            }
            if (!hasError)
            {
                var appointments = new ObservableCollection<Appointment>();
                foreach (var item in SelectedTreatments)
                {
                    var a = (Appointment)Appointment.Clone();
                    a.Treatment = item;
                    appointments.Add(a);
                }
                StartSavingAppointments(appointments, "allaboutteeth_" + GetType().Namespace.Replace("AllAboutTeethDCMS.", ""));
            }
            else
            {
                DialogBoxViewModel.Mode = "Error";
                DialogBoxViewModel.Title = "Save Failed";
                DialogBoxViewModel.Message = "Form contains errors. Please check all required fields.";
                DialogBoxViewModel.Answer = "None";
            }
        }

        #region Reset Thread

        private Thread resetThread;
        private DateTime _start;

        public virtual void startResetThread()
        {
            DialogBoxViewModel.Answer = "None";
            DialogBoxViewModel.Mode = "Confirm";
            DialogBoxViewModel.Title = "Reset Form";
            DialogBoxViewModel.Message = "Resetting form will restore previous values. Proceed?";

            while (DialogBoxViewModel.Answer.Equals("None"))
            {
                Thread.Sleep(100);
            }

            if (DialogBoxViewModel.Answer.Equals("OK"))
            {
                Appointment = new Appointment();
                foreach (PropertyInfo info in GetType().GetProperties())
                {
                    if (info.Name.EndsWith("Error"))
                    {
                        info.SetValue(this, "");
                    }
                }
            }
            DialogBoxViewModel.Answer = "";
        }

        public void resetForm()
        {
            resetThread = new Thread(startResetThread);
            resetThread.IsBackground = true;
            resetThread.Start();
        }

        #endregion Reset Thread

        #region Unimplemented Methods

        protected override void beforeLoad(MySqlCommand command)
        {
        }

        public List<Appointment> Appointments { get; set; }
        public AppointmentViewModel AppointmentViewModel { get; internal set; }

        protected override void afterLoad(List<Appointment> list)
        {
            Appointments = list.Where(x => DateTime.Compare(x.Schedule.AddMinutes(30), DateTime.Now) > -1 && x.Status.Equals("Pending")).ToList(); ;
        }

        protected override bool beforeDelete()
        {
            throw new NotImplementedException();
        }

        protected override void afterDelete(bool isSuccessful)
        {
            throw new NotImplementedException();
        }

        #endregion Unimplemented Methods
    }
}