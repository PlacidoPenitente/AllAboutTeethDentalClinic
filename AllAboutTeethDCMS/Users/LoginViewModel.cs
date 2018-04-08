using AllAboutTeethDCMS.Menu;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace AllAboutTeethDCMS.Users
{
    public class LoginViewModel : CRUDPage<User>
    {
        private User user;
        private List<User> users;

        private string username = "";
        private string userNameError = "";
        private string password = "";
        private string passwordError = "";

        public LoginViewModel()
        {
            LoginCommand = new DelegateCommand(new Action(login));
        }

        public void login()
        {
            MainWindowViewModel.MenuViewModel.gotoDashboard();
            loadUsers();
        }

        public User User { get => user; set { user = value; OnPropertyChanged(); } }
        public List<User> Users { get => users; set { users = value; OnPropertyChanged(); } }

        public string Username { get => username; set {
                bool valid = true;
                if(!value.Equals(" "))
                {
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
                        if (value.Length < 21)
                        {
                            username = value;
                        }
                    }
                }
                OnPropertyChanged();
            }
        }
        public string UserNameError { get => userNameError; set { userNameError = value; OnPropertyChanged(); } }
        public string Password
        {
            get => password;
            set
            {
                bool valid = true;
                if(!value.Equals(" "))
                {
                    PasswordError = "";
                    if (String.IsNullOrEmpty(value))
                    {
                        valid = false;
                        password = "";
                        PasswordError = "Password is required.";
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
                        if (value.Length < 21)
                        {
                            password = value;
                        }
                    }
                }
                OnPropertyChanged();
            }
        }

        private DelegateCommand loginCommand;

        public string PasswordError { get => passwordError; set { passwordError = value; OnPropertyChanged(); } }

        public DelegateCommand LoginCommand { get => loginCommand; set => loginCommand = value; }

        public void loadUsers()
        {
            UserNameError = "";
            PasswordError = "";
            if (String.IsNullOrEmpty(Username.Trim()))
            {
                UserNameError = "Username is required.";
                if (String.IsNullOrEmpty(Password.Trim()))
                {
                    PasswordError = "Password is required.";
                }
            }
            else
            {
                if (String.IsNullOrEmpty(Password.Trim()))
                {
                    PasswordError = "Password is required.";
                }
                else
                {
                    startLoadFromDatabase("allaboutteeth_" + GetType().Namespace.Replace("AllAboutTeethDCMS.", ""), Filter);
                }
            }
        }

        public void deleteUser()
        {
            startDeleteFromDatabase(User, "allaboutteeth_" + GetType().Namespace.Replace("AllAboutTeethDCMS.", ""));
        }

        protected override void afterLoad(List<User> list)
        {
            if(list.Count>0)
            {
                bool valid = false;
                foreach(User user in list)
                {
                    if(user.Password.Equals(Password))
                    {
                        valid = true;
                        User = user;

                        MainWindowViewModel.MenuViewModel = new MenuViewModel();
                        MainWindowViewModel.MenuViewModel.MainWindowViewModel = MainWindowViewModel;

                        MainWindowViewModel.ActiveUser = user;
                        MainWindowViewModel.MenuViewModel.ActiveUser = user;
                        Username = "";
                        Password = "";
                        break;
                    }
                }
                if(!valid)
                {
                    PasswordError = "Incorrect password. Please try again.";
                }
            }
            else
            {
                UserNameError = "Account does not exist.";
            }
        }

        protected override bool beforeUpdate()
        {
            throw new NotImplementedException();
        }

        protected override void afterUpdate(bool isSuccessful)
        {
            throw new NotImplementedException();
        }

        protected override bool beforeCreate()
        {
            throw new NotImplementedException();
        }

        protected override void afterCreate(bool isSuccessful)
        {
            throw new NotImplementedException();
        }

        protected override bool beforeDelete()
        {
            throw new NotImplementedException();
        }

        protected override void afterDelete(bool isSuccessful)
        {
            throw new NotImplementedException();
        }

        protected override void beforeLoad(MySqlCommand command)
        {
            command.Parameters.Clear();
            command.CommandText = "select * from allaboutteeth_users where user_username=@username";
            command.Parameters.AddWithValue("@username", Username);
        }
    }
}
