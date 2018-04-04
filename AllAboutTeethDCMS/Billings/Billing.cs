using AllAboutTeethDCMS.Appointments;
using AllAboutTeethDCMS.Providers;
using AllAboutTeethDCMS.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllAboutTeethDCMS.Billings
{
    public class Billing : ModelBase
    {
        private int no = -1;
        private Appointment appointment;
        private double amountCharged = 0;
        private double balance = 0;
        private Provider provider;
        private DateTime dateAdded = DateTime.Now;
        private DateTime dateModified = DateTime.Now;
        private User addedBy;

        public int No { get => no; set => no = value; }
        public Appointment Appointment { get => appointment; set => appointment = value; }
        public double AmountCharged { get => amountCharged; set => amountCharged = value; }
        public double Balance { get => balance; set => balance = value; }
        public Provider Provider { get => provider; set => provider = value; }
        public DateTime DateAdded { get => dateAdded; set => dateAdded = value; }
        public DateTime DateModified { get => dateModified; set => dateModified = value; }
        public User AddedBy { get => addedBy; set => addedBy = value; }
    }
}
