using AllAboutTeethDCMS.Suppliers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AllAboutTeethDCMS.Medicines
{
    public class AddMedicineViewModel : CRUDPage<Medicine>
    {
        private Medicine medicine;
        private Medicine copyMedicine;
        private List<Supplier> suppliers;

        public AddMedicineViewModel()
        {
            medicine = new Medicine();
            copyMedicine = (Medicine)medicine.Clone();
            SupplierViewModel supplierViewModel = new SupplierViewModel();
            supplierViewModel.loadSuppliers();
            Suppliers = supplierViewModel.Suppliers;
        }

        public virtual void resetForm()
        {
            Medicine = new Medicine();
        }

        public virtual void saveMedicine()
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
                Medicine.AddedBy = ActiveUser;
                startSaveToDatabase(Medicine, "allaboutteeth_" + GetType().Namespace.Replace("AllAboutTeethDCMS.", ""));
            }
        }

        protected override void setLoaded(List<Medicine> list)
        {
            throw new NotImplementedException();
        }

        public string Name { get => Medicine.Name; set { Medicine.Name = value; NameError = ""; NameError = validateUniqueName(value, CopyMedicine.Name, "allaboutteeth_" + GetType().Namespace.Replace("AllAboutTeethDCMS.", "")); OnPropertyChanged(); } }
        public string Description { get => Medicine.Description; set { Medicine.Description = value; OnPropertyChanged(); } }
        public Supplier Supplier { get => Medicine.Supplier; set { Medicine.Supplier = value; OnPropertyChanged(); } }
        public string Quantity { get => Medicine.Quantity.ToString(); set {
                try
                {
                    Medicine.Quantity = Int32.Parse(value);
                }
                catch(Exception ex)
                {
                    ex.ToString();
                    Medicine.Quantity = 0;
                }
                if(Medicine.Quantity<0)
                {
                    Medicine.Quantity = 0;
                }
                OnPropertyChanged(); } }
        public string CriticalAmount { get => Medicine.CriticalAmount.ToString(); set {
                try
                {
                    Medicine.CriticalAmount = Int32.Parse(value);
                }
                catch (Exception ex)
                {
                    ex.ToString();
                    Medicine.CriticalAmount = 0;
                }
                if (Medicine.CriticalAmount < 0)
                {
                    Medicine.CriticalAmount = 0;
                }
                OnPropertyChanged(); } }

        public Medicine Medicine
        {
            get => medicine;
            set
            {
                medicine = value;
                OnPropertyChanged();
                foreach (PropertyInfo info in GetType().GetProperties())
                {
                    OnPropertyChanged(info.Name);
                }
            }
        }

        public Medicine CopyMedicine { get => copyMedicine; set { copyMedicine = value; } }
        public string NameError { get => nameError; set { nameError = value; OnPropertyChanged(); } }
        public List<Supplier> Suppliers { get => suppliers; set => suppliers = value; }

        private string nameError = "";
    }
}
