using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AllAboutTeethDCMS.Treatments
{
    public class TreatmentViewModel : CRUDPage<Treatment>
    {
        private Treatment treatment;
        private List<Treatment> treatments;
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
            startUpdateToDatabase(Treatment, "allaboutteeth_" + GetType().Namespace.Replace("AllAboutTeethDCMS.", ""));
        }

        public void unarchive()
        {
            startUpdateToDatabase(Treatment, "allaboutteeth_" + GetType().Namespace.Replace("AllAboutTeethDCMS.", ""));
        }
        protected override bool beforeUpdate()
        {
            DialogBoxViewModel.Answer = "None";
            DialogBoxViewModel.Mode = "Question";
            if (Treatment.Status.Equals("Active"))
            {
                DialogBoxViewModel.Title = "Archive Service";
                DialogBoxViewModel.Message = "Are you sure you want to archive this service? Service can no longer be used.";
            }
            else
            {
                DialogBoxViewModel.Title = "Unarchive Service";
                DialogBoxViewModel.Message = "Are you sure you want to activate this service?";
            }
            while (DialogBoxViewModel.Answer.Equals("None"))
            {
                Thread.Sleep(100);
            }
            if (DialogBoxViewModel.Answer.Equals("Yes"))
            {
                if (Treatment.Status.Equals("Active"))
                {
                    Treatment.Status = "Archived";
                    DialogBoxViewModel.Mode = "Progress";
                    DialogBoxViewModel.Message = "Archiving service. Please wait.";
                    DialogBoxViewModel.Answer = "None";
                }
                else
                {
                    Treatment.Status = "Active";
                    DialogBoxViewModel.Mode = "Progress";
                    DialogBoxViewModel.Message = "Activating service. Please wait.";
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
                loadTreatments();
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
            DialogBoxViewModel.Title = "Delete Service";
            DialogBoxViewModel.Message = "Are you sure you want to totally delete this service?";

            while (DialogBoxViewModel.Answer.Equals("None"))
            {
                Thread.Sleep(100);
            }

            if (DialogBoxViewModel.Answer.Equals("Yes"))
            {
                DialogBoxViewModel.Mode = "Progress";
                DialogBoxViewModel.Message = "Deleteing treatment. Please wait.";
                DialogBoxViewModel.Answer = "None";
                return true;
            }
            return false;
        }

        protected override void afterDelete(bool isSuccessful)
        {
            if (isSuccessful)
            {
                loadTreatments();
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

        public TreatmentViewModel()
        {
            DialogBoxViewModel = new DialogBoxViewModel();
        }

        public Treatment Treatment
        {
            get => treatment; set
            {
                treatment = value; OnPropertyChanged();
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



        public List<Treatment> Treatments { get => treatments; set { treatments = value; OnPropertyChanged(); } }
        public string Filter { get => filter; set { filter = value; loadTreatments(); OnPropertyChanged(); } }

        public void loadTreatments()
        {
            startLoadFromDatabase("allaboutteeth_" + GetType().Namespace.Replace("AllAboutTeethDCMS.", ""), Filter);
        }

        public void deleteTreatment()
        {
            startDeleteFromDatabase(Treatment, "allaboutteeth_" + GetType().Namespace.Replace("AllAboutTeethDCMS.", ""));
        }

        protected override void setLoaded(List<Treatment> list)
        {
            Treatments = list;
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
