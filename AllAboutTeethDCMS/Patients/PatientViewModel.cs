using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllAboutTeethDCMS.Patients
{
    public class PatientViewModel : CRUDPage<Patient>
    {
        private Patient patient;
        private List<Patient> patients;
        private string filter = "";

        public Patient Patient { get => patient; set { patient = value; OnPropertyChanged(); } }
        public List<Patient> Patients { get => patients; set { patients = value; OnPropertyChanged(); } }
        public string Filter { get => filter; set { filter = value; OnPropertyChanged(); } }

        public void loadPatients()
        {
            Patients = loadFromDatabase("allaboutteeth_" + GetType().Namespace.Replace("AllAboutTeethDCMS.", ""), Filter);
        }

        public void deleteUser()
        {
            deleteFromDatabase(Patient, "allaboutteeth_" + GetType().Namespace.Replace("AllAboutTeethDCMS.", ""));
            loadPatients();
        }

        protected override void setLoaded(List<Patient> list)
        {
            throw new NotImplementedException();
        }
    }
}
