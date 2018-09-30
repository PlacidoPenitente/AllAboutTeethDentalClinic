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
    public class AppointmentGroup : ViewModelBase, IComparer<Session>
    {
        private ObservableCollection<Session> _session;
        public ObservableCollection<Session> Sessions
        {
            get
            {
                if (_session != null)
                {
                    var temp = _session.ToArray();
                    Array.Sort(temp, this);
                    _session = new ObservableCollection<Session>(temp);
                }
                return _session;
            }

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
                AppointmentViewModel.Session = _selectedSession;
                OnPropertyChanged();
            }
        }

        public AppointmentViewModel AppointmentViewModel { get; set; }
        public int Compare(Session x, Session y)
        {
            return DateTime.Compare(x.Appointments.FirstOrDefault().Schedule, y.Appointments.FirstOrDefault().Schedule);
        }
    }
}
