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
        public string Filter { get => filter; set { filter = value; OnPropertyChanged(); } }

        public void loadPatients()
        {
            Appointments = loadFromDatabase("allaboutteeth_" + GetType().Namespace.Replace("AllAboutTeethDCMS.", ""), Filter);
        }

        public void deleteTreatment()
        {
            deleteFromDatabase(Appointment, "allaboutteeth_" + GetType().Namespace.Replace("AllAboutTeethDCMS.", ""));
            loadPatients();
        }
    }
}
