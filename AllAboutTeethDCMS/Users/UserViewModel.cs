using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace AllAboutTeethDCMS.Users
{
    public class UserViewModel : CRUDPage<User>
    {
        #region Fields
        private User user;
        private List<User> users;

        private DelegateCommand loadCommand;
        private DelegateCommand archiveCommand;
        private DelegateCommand unarchiveCommand;
        private DelegateCommand deleteCommand;
        private DelegateCommand addCommand;
        private DelegateCommand editCommand;

        private string archiveVisibility = "Collapsed";
        private string unarchiveVisibility = "Collapsed";
        private string deleteVisibility = "Collapsed";
        private string editVisibility = "Collapsed";
        private string addVisibility = "Collapsed";
        #endregion

        public UserViewModel()
        {
            LoadCommand = new DelegateCommand(new Action(LoadUsers));
            ArchiveCommand = new DelegateCommand(new Action(Archive));
            UnarchiveCommand = new DelegateCommand(new Action(Unarchive));
            DeleteCommand = new DelegateCommand(new Action(DeleteUser));
            AddCommand = new DelegateCommand(new Action(GotoAddUser));
            EditCommand = new DelegateCommand(new Action(GotoEditUser));
        }

        #region Methods
        protected override bool beforeUpdate()
        {
            DialogBoxViewModel.Answer = "None";
            DialogBoxViewModel.Mode = "Question";
            if (User.Status.Equals("Active"))
            {
                DialogBoxViewModel.Title = "Archive User";
                DialogBoxViewModel.Message = "Are you sure you want to archive this user? Account can no longer be used.";
            }
            else
            {
                DialogBoxViewModel.Title = "Unarchive User";
                DialogBoxViewModel.Message = "Are you sure you want to activate this user?";
            }
            while (DialogBoxViewModel.Answer.Equals("None"))
            {
                Thread.Sleep(100);
            }
            if (DialogBoxViewModel.Answer.Equals("Yes"))
            {
                if (User.Status.Equals("Active"))
                {
                    User.Status = "Archived";
                    DialogBoxViewModel.Mode = "Progress";
                    DialogBoxViewModel.Message = "Archiving user. Please wait.";
                    DialogBoxViewModel.Answer = "None";
                }
                else
                {
                    User.Status = "Active";
                    DialogBoxViewModel.Mode = "Progress";
                    DialogBoxViewModel.Message = "Activating user. Please wait.";
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
                LoadUsers();
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
            DialogBoxViewModel.Title = "Delete User";
            DialogBoxViewModel.Message = "Are you sure you want to totally delete this user?";

            while (DialogBoxViewModel.Answer.Equals("None"))
            {
                Thread.Sleep(100);
            }

            if (DialogBoxViewModel.Answer.Equals("Yes"))
            {
                DialogBoxViewModel.Mode = "Progress";
                DialogBoxViewModel.Message = "Deleting user. Please wait.";
                DialogBoxViewModel.Answer = "None";
                return true;
            }
            return false;
        }

        protected override void afterDelete(bool isSuccessful)
        {
            if (isSuccessful)
            {
                LoadUsers();
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
            Users = null;
        }

        protected override void afterLoad(List<User> list)
        {
            Users = list;
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

        public User User { get => user;
            set
            {
                user = null;
                user = value;

                ArchiveVisibility = "Collapsed";
                UnarchiveVisibility = "Collapsed";
                DeleteVisibility = "Collapsed";
                EditVisibility = "Collapsed";
                AddVisibility = "Collapsed";
                if (value != null&&(ActiveUser.Type.Equals("Administrator")))
                {
                    if(!value.Status.Equals("Scheduled"))
                    {
                        if (value.Status.Equals("Active"))
                        {
                            ArchiveVisibility = "Visible";
                        }
                        else
                        {
                            UnarchiveVisibility = "Visible";
                        }
                        DeleteVisibility = "Visible";
                        EditVisibility = "Visible";
                        AddVisibility = "Visible";
                    }
                }
                else if(value != null && (ActiveUser.Username.Equals(value.Username)))
                {
                    EditVisibility = "Visible";
                }

                OnPropertyChanged();
            }
        }
        public List<User> Users { get => users; set { users = value; OnPropertyChanged(); } }

        public string ArchiveVisibility { get => archiveVisibility; set { archiveVisibility = value; OnPropertyChanged(); } }
        public string UnarchiveVisibility { get => unarchiveVisibility; set { unarchiveVisibility = value; OnPropertyChanged(); } }

        public string DeleteVisibility { get => deleteVisibility; set { deleteVisibility = value; OnPropertyChanged(); } }

        public string EditVisibility { get => editVisibility; set { editVisibility = value; OnPropertyChanged(); } }

        public string AddVisibility { get => addVisibility; set { addVisibility = value; OnPropertyChanged(); } }
        #endregion

        #region Commands
        public void GotoAddUser()
        {
            MenuViewModel.GotoAddUserView();
        }


        public void LoadUsers()
        {
            startLoadFromDatabase("allaboutteeth_" + GetType().Namespace.Replace("AllAboutTeethDCMS.", ""), Filter);
        }

        public void GotoEditUser()
        {
            MenuViewModel.GotoEditUserView(User);
        }

        public void Archive()
        {
            startUpdateToDatabase(User, "allaboutteeth_" + GetType().Namespace.Replace("AllAboutTeethDCMS.", ""));
        }

        public void Unarchive()
        {
            startUpdateToDatabase(User, "allaboutteeth_" + GetType().Namespace.Replace("AllAboutTeethDCMS.", ""));
        }

        public void DeleteUser()
        {
            startDeleteFromDatabase(User, "allaboutteeth_" + GetType().Namespace.Replace("AllAboutTeethDCMS.", ""));
        }
        #endregion
    }
}
