using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllAboutTeethDCMS.Appointments
{
    public class AppointmentViewModel : CRUDPage<Appointment>
    {
        private Appointment appointment;
        private List<Appointment> appointments;
        private string filter = "";

        public Appointment Appointment { get => appointment; set { appointment = value; OnPropertyChanged(); } }
        public List<Appointment> Appointments { get => appointments; set { appointments = value; OnPropertyChanged(); } }
        public string Filter { get => filter; set { filter = value; loadAppointments(); OnPropertyChanged(); } }

        public void loadAppointments()
        {
            startLoadFromDatabase("allaboutteeth_" + GetType().Namespace.Replace("AllAboutTeethDCMS.", ""), Filter);
        }

        public void deleteAppointment()
        {
            startDeleteFromDatabase(Appointment, "allaboutteeth_" + GetType().Namespace.Replace("AllAboutTeethDCMS.", ""));
        }

        protected override void setLoaded(List<Appointment> list)
        {
            Appointments = list;
        }
    }
}
