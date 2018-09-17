using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace AllAboutTeethDCMS.Treatments
{
    public class TreatmentViewModel : CRUDPage<Treatment>
    {
        #region Fields
        private Treatment treatment;
        private List<Treatment> treatments;

        private DelegateCommand loadCommand;
        private DelegateCommand archiveCommand;
        private DelegateCommand unarchiveCommand;
        private DelegateCommand deleteCommand;
        private DelegateCommand addCommand;
        private DelegateCommand editCommand;

        private string addVisibility = "Collapsed";
        public string AddVisibility { get => addVisibility; set { addVisibility = value; OnPropertyChanged(); } }

        private string archiveVisibility = "Collapsed";
        private string unarchiveVisibility = "Collapsed";
        #endregion

        public TreatmentViewModel()
        {
            LoadCommand = new DelegateCommand(new Action(LoadTreatments));
            ArchiveCommand = new DelegateCommand(new Action(Archive));
            UnarchiveCommand = new DelegateCommand(new Action(Unarchive));
            DeleteCommand = new DelegateCommand(new Action(DeleteTreatment));
            AddCommand = new DelegateCommand(new Action(GotoAddTreatment));
            EditCommand = new DelegateCommand(new Action(GotoEditTreatment));
        }

        #region Methods
        protected override bool beforeUpdate()
        {
            DialogBoxViewModel.Answer = "None";
            DialogBoxViewModel.Mode = "Question";
            if (Treatment.Status.Equals("Active"))
            {
                DialogBoxViewModel.Title = "Archive Service";
                DialogBoxViewModel.Message = "Are you sure you want to archive this service?";
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
                LoadTreatments();
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
                DialogBoxViewModel.Message = "Deleting service. Please wait.";
                DialogBoxViewModel.Answer = "None";
                return true;
            }
            return false;
        }

        protected override void afterDelete(bool isSuccessful)
        {
            if (isSuccessful)
            {
                LoadTreatments();
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

        protected override bool beforeCreate()
        {
            return true;
        }

        protected override void afterCreate(bool isSuccessful)
        {
        }

        protected override void beforeLoad(MySqlCommand command)
        {
            Treatments = null;
            GC.Collect();
        }

        protected override void afterLoad(List<Treatment> list)
        {
            Treatments = list;
            FilterResult = "";
            if (list.Count > 1)
            {
                FilterResult = "Found " + list.Count + " result/s.";
            }
        }
        #endregion

        #region Properties
        public DelegateCommand LoadCommand { get => loadCommand; set => loadCommand = value; }
        public DelegateCommand ArchiveCommand { get => archiveCommand; set => archiveCommand = value; }
        public DelegateCommand UnarchiveCommand { get => unarchiveCommand; set => unarchiveCommand = value; }
        public DelegateCommand DeleteCommand { get => deleteCommand; set => deleteCommand = value; }
        public DelegateCommand AddCommand { get => addCommand; set => addCommand = value; }
        public DelegateCommand EditCommand { get => editCommand; set => editCommand = value; }

        private string forAdminOnly = "Collapsed";

        public Treatment Treatment
        {
            get => treatment;
            set
            {
                treatment = null;
                treatment = value;
                OnPropertyChanged();

                ArchiveVisibility = "Collapsed";
                UnarchiveVisibility = "Collapsed";
                ForAdminOnly = "Collapsed";
                if (value != null&&(ActiveUser.Type.Equals("Administrator")))
                {
                    if (!value.Status.Equals("Scheduled"))
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
                    ForAdminOnly = "Visible";
                }
            }
        }
        public List<Treatment> Treatments { get => treatments; set { treatments = value; OnPropertyChanged(); } }

        public string ArchiveVisibility { get => archiveVisibility; set { archiveVisibility = value; OnPropertyChanged(); } }
        public string UnarchiveVisibility { get => unarchiveVisibility; set { unarchiveVisibility = value; OnPropertyChanged(); } }

        public string ForAdminOnly { get => forAdminOnly; set { forAdminOnly = value; OnPropertyChanged(); } }
        #endregion

        #region Commands
        public void GotoAddTreatment()
        {
            MenuViewModel.GotoAddTreatmentView();
        }

        public void LoadTreatments()
        {
            startLoadFromDatabase("allaboutteeth_" + GetType().Namespace.Replace("AllAboutTeethDCMS.", ""), Filter);
        }

        public void GotoEditTreatment()
        {
            MenuViewModel.GotoEditTreatmentView(Treatment);
        }

        public void Archive()
        {
            startUpdateToDatabase(Treatment, "allaboutteeth_" + GetType().Namespace.Replace("AllAboutTeethDCMS.", ""));
        }

        public void Unarchive()
        {
            startUpdateToDatabase(Treatment, "allaboutteeth_" + GetType().Namespace.Replace("AllAboutTeethDCMS.", ""));
        }

        public void DeleteTreatment()
        {
            startDeleteFromDatabase(Treatment, "allaboutteeth_" + GetType().Namespace.Replace("AllAboutTeethDCMS.", ""));
        }
        #endregion
    }
}
