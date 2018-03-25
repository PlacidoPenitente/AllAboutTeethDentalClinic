using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AllAboutTeethDCMS.Users
{
    public class UserViewModel : CRUDPage<User>
    {
        private User user;
        private List<User> users;
        private string filter = "";
        private string filterResult = "";
        private DialogBoxViewModel archiveDialogBoxViewModel;
        private DialogBoxViewModel unarchiveDialogBoxViewModel;
        private DialogBoxViewModel deleteDialogBoxViewModel;

        public UserViewModel()
        {
            ArchiveDialogBoxViewModel = new DialogBoxViewModel();
            UnarchiveDialogBoxViewModel = new DialogBoxViewModel();
            DeleteDialogBoxViewModel = new DialogBoxViewModel();
        }

        private string archiveVisibility = "Collapsed";
        private string unarchiveVisibility = "Collapsed";

        public User User { get => user; set { user = value; OnPropertyChanged();
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
            } }

        public void archive()
        {
            startUpdateToDatabase(User, "allaboutteeth_" + GetType().Namespace.Replace("AllAboutTeethDCMS.", ""));
        }

        public void unarchive()
        {
            startUpdateToDatabase(User, "allaboutteeth_" + GetType().Namespace.Replace("AllAboutTeethDCMS.", ""));
        }

        public List<User> Users { get => users; set { users = value; OnPropertyChanged(); } }
        public string Filter { get => filter; set { filter = value; loadUsers(); OnPropertyChanged(); } }

        public string FilterResult { get => filterResult; set { filterResult = value; OnPropertyChanged(); } }

        public string ArchiveVisibility { get => archiveVisibility; set { archiveVisibility = value; OnPropertyChanged(); } }
        public string UnarchiveVisibility { get => unarchiveVisibility; set { unarchiveVisibility = value; OnPropertyChanged(); } }

        public DialogBoxViewModel ArchiveDialogBoxViewModel { get => archiveDialogBoxViewModel; set { archiveDialogBoxViewModel = value; OnPropertyChanged(); } }
        public DialogBoxViewModel UnarchiveDialogBoxViewModel { get => unarchiveDialogBoxViewModel; set { unarchiveDialogBoxViewModel = value; OnPropertyChanged(); } }
        public DialogBoxViewModel DeleteDialogBoxViewModel { get => deleteDialogBoxViewModel; set { deleteDialogBoxViewModel = value; OnPropertyChanged(); } }

        public void loadUsers()
        {
            if(Reader!=null&&!Reader.IsClosed)
            {
                Reader.Close();
            }
            startLoadFromDatabase("allaboutteeth_" + GetType().Namespace.Replace("AllAboutTeethDCMS.", ""), Filter);
        }

        public void deleteUser()
        {
            startDeleteFromDatabase(User, "allaboutteeth_" + GetType().Namespace.Replace("AllAboutTeethDCMS.", ""));
        }

        protected override void setLoaded(List<User> list)
        {
            FilterResult = "";
            Users = list;
            if(list.Count>0)
            {
                FilterResult = "Found " + list.Count + " result/s.";
            }
        }

        protected override bool beforeUpdate()
        {
            ArchiveDialogBoxViewModel.Answer = "None";
            ArchiveDialogBoxViewModel.Mode = "Question";
            if(User.Status.Equals("Active"))
            {
                ArchiveDialogBoxViewModel.Title = "Archive User";
                ArchiveDialogBoxViewModel.Message = "Are you sure you want to archive this user? Account can no longer be used.";
            }
            else
            {
                ArchiveDialogBoxViewModel.Title = "Unarchive User";
                ArchiveDialogBoxViewModel.Message = "Are you sure you want to activate this user?";
            }
            while (ArchiveDialogBoxViewModel.Answer.Equals("None"))
            {
                Thread.Sleep(100);
            }
            if(ArchiveDialogBoxViewModel.Answer.Equals("Yes"))
            {
                if(User.Status.Equals("Active"))
                {
                    User.Status = "Archived";
                }
                else
                {
                    User.Status = "Active";
                }
                return true;
            }
            return false;
        }

        protected override void afterUpdate()
        {
            ArchiveDialogBoxViewModel.Answer = "";
            loadUsers();
        }

        protected override bool beforeSave()
        {
            return true;
        }

        protected override void afterSave()
        {
            throw new NotImplementedException();
        }
    }
}
