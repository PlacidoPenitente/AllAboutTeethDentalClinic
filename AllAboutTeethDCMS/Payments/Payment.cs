using AllAboutTeethDCMS.Billings;
using AllAboutTeethDCMS.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllAboutTeethDCMS.Payments
{
    public class Payment : ModelBase
    {
        private int no = -1;
        private Billing billing;
        private double amountPaid = 0;
        private double balance = 0;
        private DateTime dateAdded = DateTime.Now;
        private DateTime dateModified = DateTime.Now;
        private User addedBy;

        public int No { get => no; set => no = value; }
        public Billing Billing { get => billing; set => billing = value; }
        public double AmountPaid { get => amountPaid; set => amountPaid = value; }
        public double Balance { get => balance; set => balance = value; }
        public DateTime DateAdded { get => dateAdded; set => dateAdded = value; }
        public DateTime DateModified { get => dateModified; set => dateModified = value; }
        public User AddedBy { get => addedBy; set => addedBy = value; }
    }
}
