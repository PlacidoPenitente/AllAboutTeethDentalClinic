using AllAboutTeethDCMS.Users;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AllAboutTeethDCMS.Patients;

namespace AllAboutTeethDCMS.Appointments
{
    public class AppointmentGroup : ViewModelBase
    {
        private ObservableCollection<Session> _session;
        public ObservableCollection<Session> Sessions
        {
            get => _session;
            set
            {
                _session = value;
                OnPropertyChanged();
            }
        }

        public User Dentist { get; set; }


        private Session _selectedSession;
        public Session SelectedSession
        {
            get => _selectedSession;
            set
            {
                _selectedSession = value;
                AppointmentViewModel.Appointment = _selectedSession.Appointments.FirstOrDefault();
                OnPropertyChanged();
            }
        }

        public AppointmentViewModel AppointmentViewModel { get; set; }
    }
}
