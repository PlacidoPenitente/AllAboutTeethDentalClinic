using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllAboutTeethDCMS.Users
{
    public class UserViewModel : CRUDPage<User>
    {
        private User user;
        private List<User> users;
        private string filter = "";

        public User User { get => user; set { user = value; OnPropertyChanged(); } }
        public List<User> Users { get => users; set { users = value; OnPropertyChanged(); } }
        public string Filter { get => filter; set { filter = value; loadUsers(); OnPropertyChanged(); } }

        public void loadUsers()
        {
            startLoadFromDatabase("allaboutteeth_" + GetType().Namespace.Replace("AllAboutTeethDCMS.", ""), Filter);
        }

        public void deleteUser()
        {
            startDeleteFromDatabase(User, "allaboutteeth_" + GetType().Namespace.Replace("AllAboutTeethDCMS.", ""));
        }

        protected override void afterDelete()
        {
            loadUsers();
        }

        protected override void setLoaded(List<User> list)
        {
            Users = list;
        }
    }
}
