using AllAboutTeethDCMS.DentalChart;
using AllAboutTeethDCMS.Patients;
using AllAboutTeethDCMS.Treatments;
using AllAboutTeethDCMS.Users;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
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

        private PatientViewModel patientViewModel;
        private TreatmentViewModel treatmentViewModel;
        private UserViewModel userViewModel;

        private Thread loadTreatmentsThread;

        public void startLoadTreatmentsThread()
        {
            if (loadTreatmentsThread == null||!loadTreatmentsThread.IsAlive)
            {
                loadTreatmentsThread = new Thread(setTreatments);
                loadTreatmentsThread.IsBackground = true;
                loadTreatmentsThread.Start();
                treatmentViewModel.LoadTreatments();
            }
        }

        public void setTreatments()
        {
            while (treatmentViewModel.Treatments == null)
            {
                Thread.Sleep(100);
            }
            Treatments = new List<Treatment>();
            foreach (Treatment treatment in treatmentViewModel.Treatments)
            {
                if (treatment.Status.Equals("Active"))
                {
                    Treatments.Add(treatment);
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
                patientViewModel.LoadPatients();
            }
        }

        public void setPatients()
        {
            while (patientViewModel.Patients == null)
            {
                Thread.Sleep(100);
            }
            Patients = new List<Patient>();
            foreach (Patient patient in patientViewModel.Patients)
            {
                if (patient.Status.Equals("Active"))
                {
                    Patients.Add(patient);
                }
            }
        }

        private Thread loadUsersThread;

        public void startLoadUsersThread()
        {
            if (loadUsersThread == null || !loadUsersThread.IsAlive)
            {
                loadUsersThread = new Thread(setUsers);
                loadUsersThread.IsBackground = true;
                loadUsersThread.Start();
                userViewModel.LoadUsers();
            }
        }

        public void setUsers()
        {
            while (userViewModel.Users == null)
            {
                Thread.Sleep(100);
            }
            Dentists = new List<User>();
            foreach (User user in userViewModel.Users)
            {
                if (user.Status.Equals("Active")&& user.Type.Equals("Dentist"))
                {
                    Dentists.Add(user);
                }
            }
        }

        public AddAppointmentViewModel()
        {
            dentalChartViewModel = new DentalChartViewModel();

            patientViewModel = new PatientViewModel();
            treatmentViewModel = new TreatmentViewModel();
            userViewModel = new UserViewModel();

            Appointment = new Appointment();
            CopyAppointment = (Appointment)appointment.Clone();

            DialogBoxViewModel = new DialogBoxViewModel();

            startLoadTreatmentsThread();
            startLoadPatientsThread();
            startLoadUsersThread();
        }

        

        public Patient Patient { get => Appointment.Patient; set { Appointment.Patient = value; DentalChartViewModel.TeethView.Clear(); DentalChartViewModel = new DentalChartViewModel(); DentalChartViewModel.User = ActiveUser; DentalChartViewModel.Patient = value;  OnPropertyChanged(); } }
        public Treatment Treatment { get => Appointment.Treatment; set { Appointment.Treatment = value; OnPropertyChanged(); } }
        public User Dentist { get => Appointment.Dentist; set { Appointment.Dentist = value; OnPropertyChanged(); } }
        public string Notes { get => Appointment.Notes; set { Appointment.Notes = value; OnPropertyChanged(); } }
        public Appointment Appointment { get => appointment; set { appointment = value; OnPropertyChanged();
                foreach (PropertyInfo info in GetType().GetProperties())
                {
                    OnPropertyChanged(info.Name);
                }
            } }

        public List<Patient> Patients {
            get
            {
                if(patients==null)
                {
                    return new List<Patient>();
                }
                return patients;
            }
            set { patients = value; OnPropertyChanged(); } }
        public List<Treatment> Treatments {
            get
            {
                if(treatments==null)
                {
                    return new List<Treatment>();
                }
                return treatments;
            }
            set { treatments = value; OnPropertyChanged(); } }
        public List<User> Dentists {
            get
            {
                if(dentists==null)
                {
                    return new List<User>();
                }
                return dentists;
            }
            set { dentists = value; OnPropertyChanged(); } }

        public string Filter { get => filter; set { filter = value; patientViewModel.Filter = value;

                OnPropertyChanged(); } }

        public DentalChartViewModel DentalChartViewModel { get => dentalChartViewModel; set { dentalChartViewModel = value; OnPropertyChanged(); } }

        public Appointment CopyAppointment { get => copyAppointment; set => copyAppointment = value; }

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
            if (Treatment == null)
            {
                DialogBoxViewModel.Mode = "Error";
                DialogBoxViewModel.Title = "Treatment Error";
                DialogBoxViewModel.Message = "No treatment was selected.";
                startLoadDialogThread();
                return;
            }
            foreach (PropertyInfo info in GetType().GetProperties())
            {
                info.SetValue(this, info.GetValue(this));
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
                startSaveToDatabase(Appointment, "allaboutteeth_" + GetType().Namespace.Replace("AllAboutTeethDCMS.", ""));
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
        #endregion

        #region Unimplemented Methods
        protected override void beforeLoad(MySqlCommand command)
        {
            throw new NotImplementedException();
        }

        protected override void afterLoad(List<Appointment> list)
        {
            throw new NotImplementedException();
        }

        protected override bool beforeDelete()
        {
            throw new NotImplementedException();
        }

        protected override void afterDelete(bool isSuccessful)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
