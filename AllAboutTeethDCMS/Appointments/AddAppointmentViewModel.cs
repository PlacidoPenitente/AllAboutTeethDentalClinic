using AllAboutTeethDCMS.Patients;
using AllAboutTeethDCMS.Treatments;
using AllAboutTeethDCMS.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AllAboutTeethDCMS.Appointments
{
    public class AddAppointmentViewModel : CRUDPage<Appointment>
    {
        private Appointment appointment;
        private List<Patient> patients;
        private List<Treatment> treatments;
        private List<User> dentists;
        private Thread loadThread;
        private string filter = "";

        private PatientViewModel patientViewModel;
        private TreatmentViewModel treatmentViewModel;
        private UserViewModel userViewModel;

        public void startLoadThread()
        {
            if (loadThread == null||!loadThread.IsAlive)
            {
                loadThread = new Thread(setItemsSources);
                loadThread.IsBackground = true;
                loadThread.Start();
            }
        }

        public void setItemsSources()
        {
            while(Patients==null)
            {
                Patients = patientViewModel.Patients;
            }
            while (Treatments == null)
            {
                Treatments = treatmentViewModel.Treatments;
            } while (Dentists == null)
            {
                Dentists = userViewModel.Users;
            }
        }

        public AddAppointmentViewModel()
        {
            patientViewModel = new PatientViewModel();
            treatmentViewModel = new TreatmentViewModel();
            userViewModel = new UserViewModel();

            Appointment = new Appointment();
            startLoadThread();

            patientViewModel.loadPatients();
            patients = patientViewModel.Patients;

            treatmentViewModel.loadTreatments();
            treatments = treatmentViewModel.Treatments;

            userViewModel.Filter = "Dentist";
            userViewModel.loadUsers();
            Dentists = userViewModel.Users;
        }

        public Patient Patient { get => Appointment.Patient; set { Appointment.Patient = value; OnPropertyChanged(); } }
        public Treatment Treatment { get => Appointment.Treatment; set { Appointment.Treatment = value; OnPropertyChanged(); } }
        public User Dentist { get => Appointment.Dentist; set { Appointment.Dentist = value; OnPropertyChanged(); } }
        public string Notes { get => Appointment.Notes; set { Appointment.Notes = value; OnPropertyChanged(); } }
        public Appointment Appointment { get => appointment; set { appointment = value; OnPropertyChanged();
                foreach (PropertyInfo info in GetType().GetProperties())
                {
                    OnPropertyChanged(info.Name);
                }
            } }

        public List<Patient> Patients { get => patients; set { patients = value; OnPropertyChanged(); } }
        public List<Treatment> Treatments { get => treatments; set { treatments = value; OnPropertyChanged(); } }
        public List<User> Dentists { get => dentists; set { dentists = value; OnPropertyChanged(); } }

        public string Filter { get => filter; set { filter = value; patientViewModel.Filter = value; patientViewModel.Patients = null; Patients = null; patientViewModel.loadPatients(); startLoadThread(); OnPropertyChanged(); } }

        protected override void setLoaded(List<Appointment> list)
        {
            throw new NotImplementedException();
        }

        public virtual void resetForm()
        {
            Appointment = new Appointment();
        }

        public virtual void saveAppointment()
        {
            foreach (PropertyInfo info in GetType().GetProperties())
            {
                if(!info.Name.Equals("Filter"))
                {
                    info.SetValue(this, info.GetValue(this));
                }
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
                Appointment.AddedBy = ActiveUser;
                startSaveToDatabase(Appointment, "allaboutteeth_" + GetType().Namespace.Replace("AllAboutTeethDCMS.", ""));
            }
        }
    }
}
