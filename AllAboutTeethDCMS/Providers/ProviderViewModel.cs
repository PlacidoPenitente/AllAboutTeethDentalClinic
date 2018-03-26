using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AllAboutTeethDCMS.Providers
{
    public class ProviderViewModel : CRUDPage<Provider>
    {
        private Provider provider;
        private List<Provider> providers;
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
            startUpdateToDatabase(Provider, "allaboutteeth_" + GetType().Namespace.Replace("AllAboutTeethDCMS.", ""));
        }

        public void unarchive()
        {
            startUpdateToDatabase(Provider, "allaboutteeth_" + GetType().Namespace.Replace("AllAboutTeethDCMS.", ""));
        }
        protected override bool beforeUpdate()
        {
            DialogBoxViewModel.Answer = "None";
            DialogBoxViewModel.Mode = "Question";
            if (Provider.Status.Equals("Active"))
            {
                DialogBoxViewModel.Title = "Archive Provider";
                DialogBoxViewModel.Message = "Are you sure you want to archive this provider? Provider can no longer be used.";
            }
            else
            {
                DialogBoxViewModel.Title = "Unarchive Provider";
                DialogBoxViewModel.Message = "Are you sure you want to activate this provider?";
            }
            while (DialogBoxViewModel.Answer.Equals("None"))
            {
                Thread.Sleep(100);
            }
            if (DialogBoxViewModel.Answer.Equals("Yes"))
            {
                if (Provider.Status.Equals("Active"))
                {
                    Provider.Status = "Archived";
                    DialogBoxViewModel.Mode = "Progress";
                    DialogBoxViewModel.Message = "Archiving provider. Please wait.";
                    DialogBoxViewModel.Answer = "None";
                }
                else
                {
                    Provider.Status = "Active";
                    DialogBoxViewModel.Mode = "Progress";
                    DialogBoxViewModel.Message = "Activating provider. Please wait.";
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
                loadProviders();
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
            DialogBoxViewModel.Title = "Delete Provider";
            DialogBoxViewModel.Message = "Are you sure you want to totally delete this provider?";

            while (DialogBoxViewModel.Answer.Equals("None"))
            {
                Thread.Sleep(100);
            }

            if (DialogBoxViewModel.Answer.Equals("Yes"))
            {
                DialogBoxViewModel.Mode = "Progress";
                DialogBoxViewModel.Message = "Deleteing provider. Please wait.";
                DialogBoxViewModel.Answer = "None";
                return true;
            }
            return false;
        }

        protected override void afterDelete(bool isSuccessful)
        {
            if (isSuccessful)
            {
                loadProviders();
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

        public ProviderViewModel()
        {
            DialogBoxViewModel = new DialogBoxViewModel();
        }

        public Provider Provider
        {
            get => provider; set
            {
                provider = value; OnPropertyChanged();
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



        public List<Provider> Providers { get => providers; set { providers = value; OnPropertyChanged(); } }
        public string Filter { get => filter; set { filter = value; loadProviders(); OnPropertyChanged(); } }

        public void loadProviders()
        {
            startLoadFromDatabase("allaboutteeth_" + GetType().Namespace.Replace("AllAboutTeethDCMS.", ""), Filter);
        }

        public void deleteProvider()
        {
            startDeleteFromDatabase(Provider, "allaboutteeth_" + GetType().Namespace.Replace("AllAboutTeethDCMS.", ""));
        }

        protected override void setLoaded(List<Provider> list)
        {
            Providers = list;
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
