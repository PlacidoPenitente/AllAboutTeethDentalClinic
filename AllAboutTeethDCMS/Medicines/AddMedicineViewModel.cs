using AllAboutTeethDCMS.Suppliers;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AllAboutTeethDCMS.Medicines
{
    public class AddMedicineViewModel : CRUDPage<Medicine>
    {
        private Medicine medicine;
        private Medicine copyMedicine;
        private List<Supplier> suppliers;

        private SupplierViewModel supplierViewModel = new SupplierViewModel();

        private Thread loadThread;

        public void startLoadThread()
        {
            if (loadThread == null || !loadThread.IsAlive)
            {
                loadThread = new Thread(setItemsSources);
                loadThread.IsBackground = true;
                loadThread.Start();
            }
        }

        public void setItemsSources()
        {
            while (supplierViewModel.Suppliers == null)
            {
                Thread.Sleep(100);
            }
            Suppliers = new List<Supplier>();
            foreach (Supplier supplier in supplierViewModel.Suppliers)
            {
                if (supplier.Status.Equals("Active"))
                {
                    Suppliers.Add(supplier);
                }
            }
        }

        public AddMedicineViewModel()
        {
            medicine = new Medicine();
            copyMedicine = (Medicine)medicine.Clone();
            startLoadThread();
            supplierViewModel.LoadSuppliers();

            DialogBoxViewModel = new DialogBoxViewModel();
        }

        private DialogBoxViewModel dialogBoxViewModel;
        public DialogBoxViewModel DialogBoxViewModel { get => dialogBoxViewModel; set { dialogBoxViewModel = value; OnPropertyChanged(); } }
        protected override bool beforeCreate()
        {
            DialogBoxViewModel.Answer = "None";
            DialogBoxViewModel.Mode = "Question";
            DialogBoxViewModel.Title = "Add Item";
            DialogBoxViewModel.Message = "Are you sure you want to add this item?";

            while (DialogBoxViewModel.Answer.Equals("None"))
            {
                Thread.Sleep(100);
            }

            if (DialogBoxViewModel.Answer.Equals("Yes"))
            {
                DialogBoxViewModel.Mode = "Progress";
                DialogBoxViewModel.Message = "Adding item. Please wait.";
                DialogBoxViewModel.Answer = "None";
                return true;
            }
            return false;
        }

        protected override void afterCreate(bool isSuccessful)
        {
            if (isSuccessful)
            {
                DialogBoxViewModel.Mode = "Success";
                DialogBoxViewModel.Message = "Operation completed.";
                DialogBoxViewModel.Answer = "None";
                while (DialogBoxViewModel.Answer.Equals("None"))
                {
                    Thread.Sleep(100);
                }
                DialogBoxViewModel.Answer = "";
                Medicine = new Medicine();
            }
            else
            {
                DialogBoxViewModel.Mode = "Error";
                DialogBoxViewModel.Message = "Operation failed.";
                DialogBoxViewModel.Answer = "None";
                while (DialogBoxViewModel.Answer.Equals("None"))
                {
                    Thread.Sleep(100);
                }
                DialogBoxViewModel.Answer = "";
            }
        }

        protected override bool beforeUpdate()
        {
            DialogBoxViewModel.Answer = "None";
            DialogBoxViewModel.Mode = "Question";
            DialogBoxViewModel.Title = "Update Item";
            DialogBoxViewModel.Message = "Are you sure you want to update this item?";

            while (DialogBoxViewModel.Answer.Equals("None"))
            {
                Thread.Sleep(100);
            }

            if (DialogBoxViewModel.Answer.Equals("Yes"))
            {
                DialogBoxViewModel.Mode = "Progress";
                DialogBoxViewModel.Message = "Updating item. Please wait.";
                DialogBoxViewModel.Answer = "None";
                return true;
            }
            return false;
        }

        protected override void afterUpdate(bool isSuccessful)
        {
            if (isSuccessful)
            {
                DialogBoxViewModel.Mode = "Success";
                DialogBoxViewModel.Message = "Operation completed.";
                DialogBoxViewModel.Answer = "None";
                while (DialogBoxViewModel.Answer.Equals("None"))
                {
                    Thread.Sleep(100);
                }
                DialogBoxViewModel.Answer = "";
                CopyMedicine = (Medicine)Medicine.Clone();
            }
            else
            {
                DialogBoxViewModel.Mode = "Error";
                DialogBoxViewModel.Message = "Operation failed.";
                DialogBoxViewModel.Answer = "None";
                while (DialogBoxViewModel.Answer.Equals("None"))
                {
                    Thread.Sleep(100);
                }
                DialogBoxViewModel.Answer = "";
            }
        }

        private Thread resetThread;

        public virtual void startResetThread()
        {
            DialogBoxViewModel.Answer = "None";
            DialogBoxViewModel.Mode = "Question";
            DialogBoxViewModel.Title = "Reset Form";
            DialogBoxViewModel.Message = "Are you sure you want to reset this form?";

            while (DialogBoxViewModel.Answer.Equals("None"))
            {
                Thread.Sleep(100);
            }

            if (DialogBoxViewModel.Answer.Equals("Yes"))
            {
                Medicine = new Medicine();
                foreach (PropertyInfo info in GetType().GetProperties())
                {
                    if (info.Name.EndsWith("Error"))
                    {
                        info.SetValue(this, "");
                    }
                }
            }
            DialogBoxViewModel.Answer = "";
        }

        public void resetForm()
        {
            resetThread = new Thread(startResetThread);
            resetThread.IsBackground = true;
            resetThread.Start();
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
                startSaveToDatabase(Medicine, "allaboutteeth_" + GetType().Namespace.Replace("AllAboutTeethDCMS.", ""));
            }
        }

        protected override void afterLoad(List<Medicine> list)
        {
            throw new NotImplementedException();
        }

        protected override bool beforeDelete()
        {
            throw new NotImplementedException();
        }

        protected override void afterDelete(bool isSuccessful)
        {
            throw new NotImplementedException();
        }

        protected override void beforeLoad(MySqlCommand command)
        {
            throw new NotImplementedException();
        }

        public string Name { get => Medicine.Name; set { Medicine.Name = value; NameError = ""; NameError = validateUniqueName(value, CopyMedicine.Name, "allaboutteeth_" + GetType().Namespace.Replace("AllAboutTeethDCMS.", "")); OnPropertyChanged(); } }
        public string Description { get => Medicine.Description; set { Medicine.Description = value; OnPropertyChanged(); } }
        public Supplier Supplier { get => Medicine.Supplier; set { Medicine.Supplier = value; OnPropertyChanged(); } }
        public string Quantity { get => Medicine.Quantity.ToString(); set {
                bool valid = true;
                foreach (char c in value.ToArray())
                {
                    if (!Char.IsDigit(c))
                    {
                        valid = false;
                        break;
                    }
                }
                if (valid)
                {
                    try
                    {
                        Medicine.Quantity = Int32.Parse(value);
                    }
                    catch(Exception ex)
                    {
                        ex.ToString();
                        Medicine.Quantity = 0;
                    }
                }
                OnPropertyChanged(); } }
        public string CriticalAmount { get => Medicine.CriticalAmount.ToString(); set {
                bool valid = true;
                foreach (char c in value.ToArray())
                {
                    if (!Char.IsDigit(c))
                    {
                        valid = false;
                        break;
                    }
                }
                if (valid)
                {
                    try
                    {
                        Medicine.CriticalAmount = Int32.Parse(value);
                    }
                    catch (Exception ex)
                    {
                        ex.ToString();
                        Medicine.CriticalAmount = 0;
                    }
                }
                OnPropertyChanged(); } }

        public Medicine Medicine
        {
            get => medicine;
            set
            {
                medicine = value;
                foreach (PropertyInfo info in GetType().GetProperties())
                {
                    OnPropertyChanged(info.Name);
                }
                OnPropertyChanged();
            }
        }

        public Medicine CopyMedicine { get => copyMedicine; set { copyMedicine = value; } }
        public string NameError { get => nameError; set { nameError = value; OnPropertyChanged(); } }
        public List<Supplier> Suppliers { get => suppliers; set { suppliers = value; OnPropertyChanged(); } }

        private string nameError = "";
    }
}
