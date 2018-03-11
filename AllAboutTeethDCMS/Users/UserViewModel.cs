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
        public string Filter { get => filter; set { filter = value; OnPropertyChanged(); } }

        public void loadUsers()
        {
            Users = loadFromDatabase("account", "allaboutteeth_" + GetType().Namespace.Replace("AllAboutTeethDCMS.", ""), Filter);
        }

        public void deleteUser()
        {
            deleteFromDatabase(User, "account", "allaboutteeth_" + GetType().Namespace.Replace("AllAboutTeethDCMS.", ""));
            loadUsers();
        }
    }
}
