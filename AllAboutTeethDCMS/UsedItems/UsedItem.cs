using AllAboutTeethDCMS.Appointments;
using AllAboutTeethDCMS.Medicines;
using AllAboutTeethDCMS.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllAboutTeethDCMS.UsedItems
{
    public class UsedItem : ModelBase
    {
        private int no = -1;
        private Medicine medicine;
        private Appointment appointment;
        private int quantity = 0;
        private DateTime dateAdded = DateTime.Now;
        private DateTime dateModified = DateTime.Now;
        private User addedBy;

        public int No { get => no; set => no = value; }
        public Medicine Medicine { get => medicine; set => medicine = value; }
        public Appointment Appointment { get => appointment; set => appointment = value; }
        public int Quantity { get => quantity; set => quantity = value; }
        public DateTime DateAdded { get => dateAdded; set => dateAdded = value; }
        public DateTime DateModified { get => dateModified; set => dateModified = value; }
        public User AddedBy { get => addedBy; set => addedBy = value; }
    }
}
