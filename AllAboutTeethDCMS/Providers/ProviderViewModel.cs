using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace AllAboutTeethDCMS.Providers
{
    public class ProviderViewModel : CRUDPage<Provider>
    {
        #region Fields
        private Provider provider;
        private List<Provider> providers;

        private DelegateCommand loadCommand;
        private DelegateCommand archiveCommand;
        private DelegateCommand unarchiveCommand;
        private DelegateCommand deleteCommand;
        private DelegateCommand addCommand;
        private DelegateCommand editCommand;

        private string archiveVisibility = "Collapsed";
        private string unarchiveVisibility = "Collapsed";
        #endregion

        public ProviderViewModel()
        {
            LoadCommand = new DelegateCommand(new Action(LoadProviders));
            ArchiveCommand = new DelegateCommand(new Action(Archive));
            UnarchiveCommand = new DelegateCommand(new Action(Unarchive));
            DeleteCommand = new DelegateCommand(new Action(DeleteProvider));
            AddCommand = new DelegateCommand(new Action(GotoAddProvider));
            EditCommand = new DelegateCommand(new Action(GotoEditProvider));
        }

        #region Methods
        protected override bool beforeUpdate()
        {
            DialogBoxViewModel.Answer = "None";
            DialogBoxViewModel.Mode = "Question";
            if (Provider.Status.Equals("Active"))
            {
                DialogBoxViewModel.Title = "Archive Provider";
                DialogBoxViewModel.Message = "Are you sure you want to archive this provider?";
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
                LoadProviders();
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
                DialogBoxViewModel.Message = "Deleting provider. Please wait.";
                DialogBoxViewModel.Answer = "None";
                return true;
            }
            return false;
        }

        protected override void afterDelete(bool isSuccessful)
        {
            if (isSuccessful)
            {
                LoadProviders();
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
            Providers = null;
            GC.Collect();
        }

        protected override void afterLoad(List<Provider> list)
        {
            Providers = list;
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
        
        public Provider Provider
        {
            get => provider;
            set
            {
                provider = null;
                provider = value;
                OnPropertyChanged();

                ArchiveVisibility = "Collapsed";
                UnarchiveVisibility = "Collapsed";
                ForAdminOnly = "Collapsed";

                if (value != null&&(ActiveUser.Type.Equals("Administrator")))
                {
                    if (value.Status.Equals("Active"))
                    {
                        ArchiveVisibility = "Visible";
                    }
                    else
                    {
                        UnarchiveVisibility = "Visible";
                    }
                    ForAdminOnly = "Visible";
                }
            }
        }
        public List<Provider> Providers { get => providers; set { providers = value; OnPropertyChanged(); } }

        public string ArchiveVisibility { get => archiveVisibility; set { archiveVisibility = value; OnPropertyChanged(); } }
        public string UnarchiveVisibility { get => unarchiveVisibility; set { unarchiveVisibility = value; OnPropertyChanged(); } }

        public string ForAdminOnly { get => forAdminOnly; set { forAdminOnly = value; OnPropertyChanged(); } }
        #endregion

        #region Commands
        public void GotoAddProvider()
        {
            MenuViewModel.GotoAddProviderView();
        }

        public void LoadProviders()
        {
            startLoadFromDatabase("allaboutteeth_" + GetType().Namespace.Replace("AllAboutTeethDCMS.", ""), Filter);
        }

        public void GotoEditProvider()
        {
            MenuViewModel.GotoEditProviderView(Provider);
        }

        public void Archive()
        {
            startUpdateToDatabase(Provider, "allaboutteeth_" + GetType().Namespace.Replace("AllAboutTeethDCMS.", ""));
        }

        public void Unarchive()
        {
            startUpdateToDatabase(Provider, "allaboutteeth_" + GetType().Namespace.Replace("AllAboutTeethDCMS.", ""));
        }

        public void DeleteProvider()
        {
            startDeleteFromDatabase(Provider, "allaboutteeth_" + GetType().Namespace.Replace("AllAboutTeethDCMS.", ""));
        }
        #endregion
    }
}
