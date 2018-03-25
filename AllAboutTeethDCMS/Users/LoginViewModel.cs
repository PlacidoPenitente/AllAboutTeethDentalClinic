using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AllAboutTeethDCMS.Users
{
    public class LoginViewModel : CRUDPage<User>
    {
        private User user;
        private List<User> users;
        private string filter = "";

        private string username = "";
        private string userNameError = "";
        private string password = "";
        private string passwordError = "";

        public User User { get => user; set { user = value; OnPropertyChanged(); } }
        public List<User> Users { get => users; set { users = value; OnPropertyChanged(); } }
        public string Filter { get => filter; set { filter = value; loadUsers(); OnPropertyChanged(); } }

        public string Username { get => username; set {
                bool valid = true;
                UserNameError = "";
                if (String.IsNullOrEmpty(value))
                {
                    valid = false;
                    username = "";
                    UserNameError = "Username is required.";
                }
                foreach (char c in value.ToArray())
                {
                    if (!Char.IsLetterOrDigit(c))
                    {
                        valid = false;
                        break;
                    }
                }
                if (valid)
                {
                    username = value;
                }
                OnPropertyChanged(); } }
        public string UserNameError { get => userNameError; set { userNameError = value; OnPropertyChanged(); } }
        public string Password { get => password; set { password = value; OnPropertyChanged(); } }
        public string PasswordError { get => passwordError; set { passwordError = value; OnPropertyChanged(); } }

        public void loadUsers()
        {
            startLoadFromDatabase("allaboutteeth_" + GetType().Namespace.Replace("AllAboutTeethDCMS.", ""), Filter);
        }

        public void deleteUser()
        {
            startDeleteFromDatabase(User, "allaboutteeth_" + GetType().Namespace.Replace("AllAboutTeethDCMS.", ""));
        }

        protected override void setLoaded(List<User> list)
        {
            if(list.Count>0)
            {
                User = list.ElementAt(0);
                MainWindowViewModel.ActiveUser = User;
            }
        }

        protected override bool beforeUpdate()
        {
            throw new NotImplementedException();
        }

        protected override void afterUpdate()
        {
            throw new NotImplementedException();
        }

        protected override bool beforeSave()
        {
            throw new NotImplementedException();
        }

        protected override void afterSave()
        {
            throw new NotImplementedException();
        }
    }
}
