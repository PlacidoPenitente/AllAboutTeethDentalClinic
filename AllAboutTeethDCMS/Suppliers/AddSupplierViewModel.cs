using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AllAboutTeethDCMS.Suppliers
{
    public class AddSupplierViewModel : CRUDPage<Supplier>
    {
        private Supplier supplier;
        private Supplier copySupplier;

        public AddSupplierViewModel()
        {
            supplier = new Supplier();
            copySupplier = (Supplier)supplier.Clone();
        }

        public virtual void resetForm()
        {
            Supplier = new Supplier();
        }

        public virtual void saveSupplier()
        {
            foreach (PropertyInfo info in GetType().GetProperties())
            {
                info.SetValue(this, info.GetValue(this));
            }
            bool hasError = false;
            foreach (PropertyInfo info in GetType().GetProperties())
            {
                if (info.Name.EndsWith("Error"))
                {
                    if (!((string)info.GetValue(this)).Equals(""))
                    {
                        hasError = true;
                        break;
                    }
                }
            }
            if (!hasError)
            {
                Supplier.AddedBy = ActiveUser;
                startSaveToDatabase(Supplier, "allaboutteeth_" + GetType().Namespace.Replace("AllAboutTeethDCMS.", ""));
            }
        }

        protected override void setLoaded(List<Supplier> list)
        {
            throw new NotImplementedException();
        }

        public Supplier Supplier
        {
            get => supplier;
            set
            {
                supplier = value;
                OnPropertyChanged();
                foreach (PropertyInfo info in GetType().GetProperties())
                {
                    OnPropertyChanged(info.Name);
                }
            }
        }

        public string Name { get => Supplier.Name; set { Supplier.Name = value; NameError = ""; NameError = validateUniqueName(value, CopySupplier.Name, "allaboutteeth_" + GetType().Namespace.Replace("AllAboutTeethDCMS.", "")); OnPropertyChanged(); } }
        public string Address { get => Supplier.Address; set { Supplier.Address = value; AddressError = ""; AddressError = validate(value); OnPropertyChanged(); } }
        public string Products { get => Supplier.Products; set { Supplier.Products = value; OnPropertyChanged(); } }
        public Supplier CopySupplier { get => copySupplier; set { copySupplier = value; OnPropertyChanged(); } }
        public string ContactNo { get => Supplier.ContactNo; set { Supplier.ContactNo = value; OnPropertyChanged(); } }
        public string Schedule { get => Supplier.Schedule; set { Supplier.Schedule = value; OnPropertyChanged(); } }

        public string NameError { get => nameError; set { nameError = value; OnPropertyChanged(); } }
        public string AddressError { get => addressError; set { addressError = value; OnPropertyChanged(); } }

        private string nameError = "";
        private string addressError = "";
    }
}
