﻿using AllAboutTeethDCMS.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllAboutTeethDCMS.Suppliers
{
    public class Supplier : ModelBase
    {
        private int no = -1;
        private string name = "";
        private string address = "";
        private string contactNo = "";
        private string schedule = "";
        private string products = "";
        private DateTime dateAdded = DateTime.Now;
        private DateTime dateModified = DateTime.Now;
        private User addedBy;
        private string status = "Active";

        public int No { get => no; set => no = value; }
        public string Name { get => name; set => name = value; }
        public string Address { get => address; set => address = value; }
        public string ContactNo { get => contactNo; set => contactNo = value; }
        public string Schedule { get => schedule; set => schedule = value; }
        public string Products { get => products; set => products = value; }
        public DateTime DateAdded { get => dateAdded; set => dateAdded = value; }
        public DateTime DateModified { get => dateModified; set => dateModified = value; }
        public User AddedBy { get => addedBy; set => addedBy = value; }
        public string Status { get => status; set => status = value; }

        public override string ToString()
        {
            return Name;
        }
    }
}
