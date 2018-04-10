using MySql.Data.MySqlClient;
using System;
using System.ComponentModel;

namespace AllAboutTeethDCMS.Users
{
    public class LoginViewModel : PageViewModel
    {
        private User user;
        private string username = "jaymark";
        private string password = "jaymark";
        private string usernameError = "";
        private string passwordError = "";
        private string visibility = "Visible";
        private DelegateCommand loginCommand;
        private BackgroundWorker loginBackgroundWorker;
        private bool isValidUser = false;
        private double width = 0;

        public LoginViewModel()
        {
            User = new User();
            LoginBackgroundWorker = new BackgroundWorker();
            LoginBackgroundWorker.DoWork += Login;
            LoginBackgroundWorker.RunWorkerCompleted += LoginCompleted;
            LoginCommand = new DelegateCommand(new Action(StartLoginProcess));
        }

        public User User { get => user; set => user = value; }
        public DelegateCommand LoginCommand { get => loginCommand; set => loginCommand = value; }
        public BackgroundWorker LoginBackgroundWorker { get => loginBackgroundWorker; set => loginBackgroundWorker = value; }
        public bool IsValidUser { get => isValidUser; set => isValidUser = value; }

        public string Username { get => username; set { username = value; OnPropertyChanged(); } }
        public string Password { get => password; set { password = value; OnPropertyChanged(); } }
        public string UsernameError { get => usernameError; set { usernameError = value; OnPropertyChanged(); } }
        public string PasswordError { get => passwordError; set { passwordError = value; OnPropertyChanged(); } }
        public string Visibility { get => visibility; set { visibility = value; OnPropertyChanged(); } }
        public double Width { get => width; set { width = value; OnPropertyChanged(); } }

        public void StartLoginProcess()
        {
            if(!LoginBackgroundWorker.IsBusy)
            {
                LoginBackgroundWorker.RunWorkerAsync();
            }
        }

        public void Login(object sender, DoWorkEventArgs e)
        {
            using (MySqlConnection connection = CreateConnection())
            {
                using (MySqlCommand command = connection.CreateCommand())
                {
                    command.CommandText = "SELECT * FROM allaboutteeth_users where user_username=@username AND user_password=@password AND user_status='Active'";
                    command.Parameters.AddWithValue("@username", Username);
                    command.Parameters.AddWithValue("@password", Password);
                    using (MySqlDataReader reader = command.ExecuteReader())
                    {
                        if(reader.Read())
                        {
                            User = (User)LoadObject(User,reader.GetInt32("user_no"));
                            IsValidUser = true;
                        }
                        reader.Close();
                        connection.Close();
                    }
                }
            }
        }

        public void LoginCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if(IsValidUser)
            {
                MainWindowViewModel.ActiveUser = User;
                MainWindowViewModel.MenuViewModel.gotoDashboard();
                Visibility = "Collapsed";
            }
        }
    }
}
