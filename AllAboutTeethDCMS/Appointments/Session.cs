using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Web.UI.WebControls;
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

        public string Date
        {
            get => _date;
            set
            {
                _date = value;
                OnPropertyChanged();
            }
        }

        private string _time;
        private string _date;

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
            get
            {
                return _appointments;
            }

            set
            {
                _appointments = value;
                OnPropertyChanged();
            }
        }
    }
}