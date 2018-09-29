using AllAboutTeethDCMS.Patients;
using AllAboutTeethDCMS.Treatments;
using AllAboutTeethDCMS.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllAboutTeethDCMS.Appointments
{
    public class Appointment : ModelBase
    {
        private int no = -1;
        private Patient patient;
        private Treatment treatment;
        private User dentist;
        private string status = "Pending";
        private DateTime dateAdded = DateTime.Now;
        private DateTime dateModified = DateTime.Now;
        private User addedBy;

        public int No { get => no; set => no = value; }
        public Patient Patient { get => patient; set => patient = value; }
        public Treatment Treatment { get => treatment; set => treatment = value; }
        public User Dentist { get => dentist; set => dentist = value; }
        public DateTime Schedule { get; set; } = DateTime.Now;
        public string Status { get => status; set => status = value; }
        public DateTime DateAdded { get => dateAdded; set => dateAdded = value; }
        public DateTime DateModified { get => dateModified; set => dateModified = value; }
        public User AddedBy { get => addedBy; set => addedBy = value; }
    }
}