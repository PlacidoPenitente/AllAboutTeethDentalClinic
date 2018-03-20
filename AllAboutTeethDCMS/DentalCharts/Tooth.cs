using AllAboutTeethDCMS.Patients;
using AllAboutTeethDCMS.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllAboutTeethDCMS.DentalCharts
{
    public class Tooth : ModelBase
    {
        private int no = -1;
        private Patient owner;
        private string condition = "";
        private string toothNo = "";
        private DateTime dateAdded = DateTime.Now;
        private DateTime dateModified = DateTime.Now;
        private User addedBy;

        public int No { get => no; set => no = value; }
        public Patient Owner { get => owner; set => owner = value; }
        public string Condition { get => condition; set => condition = value; }
        public string ToothNo { get => toothNo; set => toothNo = value; }
        public DateTime DateAdded { get => dateAdded; set => dateAdded = value; }
        public DateTime DateModified { get => dateModified; set => dateModified = value; }
        public User AddedBy { get => addedBy; set => addedBy = value; }
    }
}
