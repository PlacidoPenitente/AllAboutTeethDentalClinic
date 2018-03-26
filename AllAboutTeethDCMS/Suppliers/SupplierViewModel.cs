using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AllAboutTeethDCMS.Suppliers
{
    public class SupplierViewModel : CRUDPage<Supplier>
    {
        private Supplier supplier;
        private List<Supplier> suppliers;
        private string filter = "";

        private DialogBoxViewModel dialogBoxViewModel;
        private string archiveVisibility = "Collapsed";
        private string unarchiveVisibility = "Collapsed";
        private string filterResult = "";

        public DialogBoxViewModel DialogBoxViewModel { get => dialogBoxViewModel; set { dialogBoxViewModel = value; OnPropertyChanged(); } }
        public string ArchiveVisibility { get => archiveVisibility; set { archiveVisibility = value; OnPropertyChanged(); } }
        public string UnarchiveVisibility { get => unarchiveVisibility; set { unarchiveVisibility = value; OnPropertyChanged(); } }
        public string FilterResult { get => filterResult; set { filterResult = value; OnPropertyChanged(); } }

        public void archive()
        {
            startUpdateToDatabase(Supplier, "allaboutteeth_" + GetType().Namespace.Replace("AllAboutTeethDCMS.", ""));
        }

        public void unarchive()
        {
            startUpdateToDatabase(Supplier, "allaboutteeth_" + GetType().Namespace.Replace("AllAboutTeethDCMS.", ""));
        }
        protected override bool beforeUpdate()
        {
            DialogBoxViewModel.Answer = "None";
            DialogBoxViewModel.Mode = "Question";
            if (Supplier.Status.Equals("Active"))
            {
                DialogBoxViewModel.Title = "Archive Supplier";
                DialogBoxViewModel.Message = "Are you sure you want to archive this supplier? Supplier can no longer be used.";
            }
            else
            {
                DialogBoxViewModel.Title = "Unarchive Supplier";
                DialogBoxViewModel.Message = "Are you sure you want to activate this supplier?";
            }
            while (DialogBoxViewModel.Answer.Equals("None"))
            {
                Thread.Sleep(100);
            }
            if (DialogBoxViewModel.Answer.Equals("Yes"))
            {
                if (Supplier.Status.Equals("Active"))
                {
                    Supplier.Status = "Archived";
                    DialogBoxViewModel.Mode = "Progress";
                    DialogBoxViewModel.Message = "Archiving supplier. Please wait.";
                    DialogBoxViewModel.Answer = "None";
                }
                else
                {
                    Supplier.Status = "Active";
                    DialogBoxViewModel.Mode = "Progress";
                    DialogBoxViewModel.Message = "Activating supplier. Please wait.";
                    DialogBoxViewModel.Answer = "None";
                }
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
                loadSuppliers();
            }
            else
            {
                DialogBoxViewModel.Mode = "Error";
                DialogBoxViewModel.Message = "Operation failed.";
                DialogBoxViewModel.Answer = "None";
            }
            while (DialogBoxViewModel.Answer.Equals("None"))
            {
                Thread.Sleep(100);
            }
            DialogBoxViewModel.Answer = "";
        }

        protected override bool beforeDelete()
        {
            DialogBoxViewModel.Answer = "None";
            DialogBoxViewModel.Mode = "Question";
            DialogBoxViewModel.Title = "Delete Supplier";
            DialogBoxViewModel.Message = "Are you sure you want to totally delete this supplier?";

            while (DialogBoxViewModel.Answer.Equals("None"))
            {
                Thread.Sleep(100);
            }

            if (DialogBoxViewModel.Answer.Equals("Yes"))
            {
                DialogBoxViewModel.Mode = "Progress";
                DialogBoxViewModel.Message = "Deleteing supplier. Please wait.";
                DialogBoxViewModel.Answer = "None";
                return true;
            }
            return false;
        }

        protected override void afterDelete(bool isSuccessful)
        {
            if (isSuccessful)
            {
                loadSuppliers();
                DialogBoxViewModel.Mode = "Success";
                DialogBoxViewModel.Message = "Operation completed.";
                DialogBoxViewModel.Answer = "None";
            }
            else
            {
                DialogBoxViewModel.Mode = "Error";
                DialogBoxViewModel.Message = "Operation failed.";
                DialogBoxViewModel.Answer = "None";
            }
            while (DialogBoxViewModel.Answer.Equals("None"))
            {
                Thread.Sleep(100);
            }
            DialogBoxViewModel.Answer = "";
        }

        public SupplierViewModel()
        {
            DialogBoxViewModel = new DialogBoxViewModel();
        }

        public Supplier Supplier
        {
            get => supplier; set
            {
                supplier = value; OnPropertyChanged();
                ArchiveVisibility = "Collapsed";
                UnarchiveVisibility = "Collapsed";
                if (value != null)
                {
                    if (value.Status.Equals("Active"))
                    {
                        ArchiveVisibility = "Visible";
                    }
                    else
                    {
                        UnarchiveVisibility = "Visible";
                    }
                }
            }
        }



        public List<Supplier> Suppliers { get => suppliers; set { suppliers = value; OnPropertyChanged(); } }
        public string Filter { get => filter; set { filter = value; loadSuppliers(); OnPropertyChanged(); } }

        public void loadSuppliers()
        {
            startLoadFromDatabase("allaboutteeth_" + GetType().Namespace.Replace("AllAboutTeethDCMS.", ""), Filter);
        }

        public void deleteSupplier()
        {
            startDeleteFromDatabase(Supplier, "allaboutteeth_" + GetType().Namespace.Replace("AllAboutTeethDCMS.", ""));
        }

        protected override void setLoaded(List<Supplier> list)
        {
            Suppliers = list;
            FilterResult = "";
            if (list.Count > 0)
            {
                FilterResult = "Found " + list.Count + " result/s.";
            }
        }

        protected override bool beforeSave()
        {
            return true;
        }

        protected override void afterSave(bool isSuccessful)
        {
            throw new NotImplementedException();
        }
    }
}
