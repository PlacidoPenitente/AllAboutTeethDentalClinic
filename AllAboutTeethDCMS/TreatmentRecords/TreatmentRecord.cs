using AllAboutTeethDCMS.Appointments;
using AllAboutTeethDCMS.DentalCharts;
using AllAboutTeethDCMS.Patients;
using AllAboutTeethDCMS.Treatments;
using AllAboutTeethDCMS.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllAboutTeethDCMS.TreatmentRecords
{
    public class TreatmentRecord : ModelBase
    {
        private int no = -1;
        private Patient patient;
        private Appointment appointment;
        private Tooth tooth;
        private Treatment treatment;
        private string notes = "";
        private DateTime dateAdded = DateTime.Now;
        private DateTime dateModified = DateTime.Now;
        private User addedBy;

        public int No { get => no; set => no = value; }
        public Patient Patient { get => patient; set => patient = value; }
        public Appointment Appointment { get => appointment; set => appointment = value; }
        public Tooth Tooth { get => tooth; set => tooth = value; }
        public Treatment Treatment { get => treatment; set => treatment = value; }
        public string Notes { get => notes; set => notes = value; }
        public DateTime DateAdded { get => dateAdded; set => dateAdded = value; }
        public DateTime DateModified { get => dateModified; set => dateModified = value; }
        public User AddedBy { get => addedBy; set => addedBy = value; }
    }
}
