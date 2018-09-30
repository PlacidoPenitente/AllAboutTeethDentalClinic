using System.Collections.ObjectModel;
using AllAboutTeethDCMS.Patients;

namespace AllAboutTeethDCMS.Appointments
{
    public class Session : ViewModelBase
    {
        private ObservableCollection<Appointment> _appointments;
        
        private Patient _patient;
        public Patient Patient
        {
            get => _patient;
            set
            {
                _patient = value;
                OnPropertyChanged();
            }
        }

        private string _time;
        public string Time
        {
            get => _time;
            set
            {
                _time = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<Appointment> Appointments
        {
            get => _appointments;
            set
            {
                _appointments = value;
                OnPropertyChanged();
            }
        }
    }
}