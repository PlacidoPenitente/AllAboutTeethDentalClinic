using AllAboutTeethDCMS.Suppliers;
using AllAboutTeethDCMS.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllAboutTeethDCMS.Medicines
{
    public class Medicine : ModelBase
    {
        private int no = -1;
        private string name = "";
        private string description = "";
        private Supplier supplier;
        private int quantity = 0;
        private int criticalAmount = 0;
        private DateTime dateAdded = DateTime.Now;
        private DateTime dateModified = DateTime.Now;
        private User addedBy;
        private string status = "Active";

        public int No { get => no; set => no = value; }
        public string Name { get => name; set => name = value; }
        public string Description { get => description; set => description = value; }
        public Supplier Supplier { get => supplier; set => supplier = value; }
        public int Quantity { get => quantity; set => quantity = value; }
        public int CriticalAmount { get => criticalAmount; set => criticalAmount = value; }
        public DateTime DateAdded { get => dateAdded; set => dateAdded = value; }
        public DateTime DateModified { get => dateModified; set => dateModified = value; }
        public User AddedBy { get => addedBy; set => addedBy = value; }
        public string Status { get => status; set => status = value; }
    }
}
