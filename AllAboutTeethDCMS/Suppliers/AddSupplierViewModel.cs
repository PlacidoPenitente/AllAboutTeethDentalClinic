using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AllAboutTeethDCMS.ActivityLogs;
using MySql.Data.MySqlClient;

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
            DialogBoxViewModel = new DialogBoxViewModel();
        }

        protected override bool beforeCreate()
        {
            DialogBoxViewModel.Answer = "None";
            DialogBoxViewModel.Mode = "Question";
            DialogBoxViewModel.Title = "Add Supplier";
            DialogBoxViewModel.Message = "Are you sure you want to add this supplier?";

            while (DialogBoxViewModel.Answer.Equals("None"))
            {
                Thread.Sleep(100);
            }

            if (DialogBoxViewModel.Answer.Equals("Yes"))
            {
                DialogBoxViewModel.Mode = "Progress";
                DialogBoxViewModel.Message = "Adding supplier. Please wait.";
                DialogBoxViewModel.Answer = "None";
                return true;
            }
            return false;
        }

        protected override void afterCreate(bool isSuccessful)
        {
            if (isSuccessful)
            {
                AddActivityLogViewModel addActivityLog = new AddActivityLogViewModel();
                addActivityLog.ActivityLog = new ActivityLog();
                addActivityLog.ActiveUser = ActiveUser;
                addActivityLog.ActivityLog.Activity = "User created a new supplier named " + Supplier.Name + ".";
                addActivityLog.saveActivityLog();

                DialogBoxViewModel.Mode = "Success";
                DialogBoxViewModel.Message = "Operation completed.";
                DialogBoxViewModel.Answer = "None";
                while (DialogBoxViewModel.Answer.Equals("None"))
                {
                    Thread.Sleep(100);
                }
                DialogBoxViewModel.Answer = "";
                Supplier = new Supplier();
                CopySupplier = (Supplier)Supplier.Clone();
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
            DialogBoxViewModel.Title = "Update Supplier";
            DialogBoxViewModel.Message = "Are you sure you want to update this supplier?";

            while (DialogBoxViewModel.Answer.Equals("None"))
            {
                Thread.Sleep(100);
            }

            if (DialogBoxViewModel.Answer.Equals("Yes"))
            {
                DialogBoxViewModel.Mode = "Progress";
                DialogBoxViewModel.Message = "Updating supplier. Please wait.";
                DialogBoxViewModel.Answer = "None";
                return true;
            }
            return false;
        }

        protected override void afterUpdate(bool isSuccessful)
        {
            if (isSuccessful)
            {
                AddActivityLogViewModel addActivityLog = new AddActivityLogViewModel();
                addActivityLog.ActivityLog = new ActivityLog();
                addActivityLog.ActiveUser = ActiveUser;
                addActivityLog.ActivityLog.Activity = "User updated a supplier named " + Supplier.Name + ".";
                addActivityLog.saveActivityLog();

                DialogBoxViewModel.Mode = "Success";
                DialogBoxViewModel.Message = "Operation completed.";
                DialogBoxViewModel.Answer = "None";
                while (DialogBoxViewModel.Answer.Equals("None"))
                {
                    Thread.Sleep(100);
                }
                DialogBoxViewModel.Answer = "";
                CopySupplier = (Supplier)Supplier.Clone();
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
                startSaveToDatabase(Supplier, "allaboutteeth_" + GetType().Namespace.Replace("AllAboutTeethDCMS.", ""));
            }
            else
            {
                DialogBoxViewModel.Mode = "Error";
                DialogBoxViewModel.Title = "Save Failed";
                DialogBoxViewModel.Message = "Form contains errors. Please check all required fields.";
                DialogBoxViewModel.Answer = "None";
            }
        }

        #region Reset Thread

        private Thread resetThread;

        public virtual void startResetThread()
        {
            DialogBoxViewModel.Answer = "None";
            DialogBoxViewModel.Mode = "Confirm";
            DialogBoxViewModel.Title = "Reset Form";
            DialogBoxViewModel.Message = "Resetting form will restore previous values. Proceed?";

            while (DialogBoxViewModel.Answer.Equals("None"))
            {
                Thread.Sleep(100);
            }

            if (DialogBoxViewModel.Answer.Equals("OK"))
            {
                Supplier = new Supplier();
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
        #endregion

        #region Unimplemented Methods
        protected override void beforeLoad(MySqlCommand command)
        {
            throw new NotImplementedException();
        }

        protected override void afterLoad(List<Supplier> list)
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
        #endregion

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

        public string Name { get => Supplier.Name;
            set
            {
                if (!value.Contains("  ") && !value.StartsWith(" "))
                {
                    Supplier.Name = value;
                    NameError = "";
                    NameError = validateUniqueName(value, CopySupplier.Name, "allaboutteeth_" + GetType().Namespace.Replace("AllAboutTeethDCMS.", ""));
                    OnPropertyChanged();
                }
                
            }
        }
        public string Address { get => Supplier.Address;
            set
            {
                if (!value.Contains("  ") && !value.StartsWith(" "))
                {
                    Supplier.Address = value; AddressError = "";
                    AddressError = validate(value);
                    OnPropertyChanged();
                }
                
            }
        }
        public string Products { get => Supplier.Products;
            set
            {
                if (!value.Contains("  ") && !value.StartsWith(" "))
                {
                    Supplier.Products = value;
                    OnPropertyChanged();
                }
                
            }
        }
        public Supplier CopySupplier { get => copySupplier; set { copySupplier = value; OnPropertyChanged(); } }
        public string ContactNo { get => Supplier.ContactNo; set {
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
                    Supplier.ContactNo = value;
                }
                OnPropertyChanged(); } }
        public string Schedule { get => Supplier.Schedule;
            set
            {
                if (!value.Contains("  ") && !value.StartsWith(" "))
                {
                    Supplier.Schedule = value;
                    OnPropertyChanged();
                }
            }
        }

        public string NameError { get => nameError; set { nameError = value; OnPropertyChanged(); } }
        public string AddressError { get => addressError; set { addressError = value; OnPropertyChanged(); } }

        private string nameError = "";
        private string addressError = "";
    }
}
