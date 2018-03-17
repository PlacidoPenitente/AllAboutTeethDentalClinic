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
        private string error = "";

        public AddSupplierViewModel()
        {
            supplier = new Supplier();
        }

        public virtual void resetForm()
        {
            Supplier = new Supplier();
        }

        public virtual void saveSupplier()
        {
            Supplier.AddedBy = ActiveUser;
            saveToDatabase(Supplier, "allaboutteeth_" + GetType().Namespace.Replace("AllAboutTeethDCMS.", ""));
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
        public string Name { get => Supplier.Name; set { Supplier.Name = value; OnPropertyChanged(); } }
        public string Address { get => Supplier.Address; set { Supplier.Address = value; OnPropertyChanged(); } }
        public string Products { get => Supplier.Products; set { Supplier.Products = value; OnPropertyChanged(); } }
        public Supplier CopySupplier { get => copySupplier; set { copySupplier = value; OnPropertyChanged(); } }
        public string Error { get => error; set { error = value; OnPropertyChanged(); } }
    }
}
