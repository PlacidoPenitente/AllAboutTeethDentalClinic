using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace AllAboutTeethDCMS.Users
{
    public class RecoverPasswordViewModel : PageViewModel
    {
        private User user;

        private string username = "";

        private string question1 = "";
        private string question2 = "";
        private string answer1="";
        private string answer2 = "";
        private string password = "";
        private string passwordCopy = "";
        private string passwordError = "";
        private string passwordCopyError = "";
        private string answer1Error = "";
        private string answer2Error = "";

        public RecoverPasswordViewModel()
        {
            ChangedPasswordCommand = new DelegateCommand(new Action(updateUser));
            CancelCommand = new DelegateCommand(new Action(cancel));
        }

        public void cancel()
        {
            Username = "";
            answer1 = "";
            answer2 = "";
            OnPropertyChanged("Answer1");
            OnPropertyChanged("Answer2");
            Password = "";
            PasswordCopy = "";
            PasswordError = "";
            PasswordCopyError = "";
        }

        public void updateUser()
        {
            using (MySqlConnection connection = CreateConnection())
            {
                using (MySqlCommand command = connection.CreateCommand())
                {
                    if(Password.Equals(PasswordCopy))
                    {
                        if(MessageBox.Show("Are you sure you want to change your password?", "Change Password",MessageBoxButton.YesNo,MessageBoxImage.Question)==MessageBoxResult.Yes)
                        {
                            command.CommandText = "update allaboutteeth_users set user_password=@password where user_no=@no";
                            command.Parameters.AddWithValue("@password", Password);
                            command.Parameters.AddWithValue("@no", User.No);
                            command.ExecuteNonQuery();
                            MessageBox.Show("Successfully changed password.","Password Changed",MessageBoxButton.OK, MessageBoxImage.Information);
                            cancel();
                        }
                    }
                }
            }
        }

        private DelegateCommand changedPasswordCommand;
        private DelegateCommand cancelCommand;

        private Visibility visibility = Visibility.Collapsed;

        public string Answer1 { get => answer1;
            set
            {
                Answer1Error = "";
                if (value.Equals(User.Answer1) && Answer2.Equals(User.Answer2))
                {
                    Visibility = Visibility.Visible;
                }
                else
                {
                    Visibility = Visibility.Collapsed;
                }
                if (!value.Equals(User.Answer1))
                {
                    Answer1Error = "Answer is incorrect.";
                }
                else if(String.IsNullOrEmpty(value))
                {
                    Answer1Error = "Answer is required.";
                }
                answer1 = value;
                OnPropertyChanged();
            }
        }
        public string Answer2 { get => answer2;
            set
            {
                Answer2Error = "";
                if (Answer1.Equals(User.Answer1) && value.Equals(User.Answer2))
                {
                    Visibility = Visibility.Visible;
                }
                else
                {
                    Visibility = Visibility.Collapsed;
                }
                if (!value.Equals(User.Answer2))
                {
                    Answer2Error = "Answer is incorrect.";
                }
                else if (String.IsNullOrEmpty(value))
                {
                    Answer2Error = "Answer is required.";
                }
                answer2 = value;
                OnPropertyChanged();
            }
        }
        public string Question1 { get => question1; set { question1 = value; OnPropertyChanged(); } }
        public string Question2 { get => question2; set { question2 = value; OnPropertyChanged(); } }
        public string Username { get => username;
            set
            {
                Question1 = "";
                Question2 = "";
                if(!value.Contains(" "))
                {
                    if(value.Length<16)
                    {
                        username = value;
                        OnPropertyChanged();
                        using (MySqlConnection connection = CreateConnection())
                        {
                            using (MySqlCommand command = connection.CreateCommand())
                            {
                                command.CommandText = "select * from allaboutteeth_users where user_username = '"+value+"'";
                                using (MySqlDataReader reader = command.ExecuteReader())
                                {
                                    if (reader.Read())
                                    {
                                        User = (User)LoadObject(new User(), reader.GetInt32("user_no"));
                                        Question1 = user.Question1;
                                        Question2 = user.Question2;
                                    }
                                    else
                                    {
                                        User = null;
                                    }
                                    reader.Close();
                                    connection.Close();
                                }
                            }
                        }
                    }
                }
            }
        }

        public User User { get => user; set { user = value; OnPropertyChanged(); } }
        public string Password { get => password;
            set
            {
                if (!value.Contains(" "))
                {
                    bool valid = true;
                    if (!value.Equals(" "))
                    {
                        PasswordError = "";
                        if (!value.Equals(PasswordCopy))
                        {
                            PasswordCopyError = "Doesn't match with password.";
                        }
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

                    OnPropertyChanged("PasswordCopyError");
                    OnPropertyChanged();
                }
            }
        }
        public string PasswordCopy { get => passwordCopy;
            set
            {
                if (!value.Contains(" "))
                {
                    bool valid = true;
                    if (!value.Equals(" "))
                    {
                        PasswordCopyError = "";
                        if (!value.Equals(Password))
                        {
                            PasswordCopyError = "Doesn't match with password.";
                        }
                        if (String.IsNullOrEmpty(value))
                        {
                            valid = false;
                            passwordCopy = "";
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
                                passwordCopy = value;
                            }
                        }
                    }

                    OnPropertyChanged("PasswordCopyError");
                    OnPropertyChanged();
                }
            }
        }

        public string PasswordError { get => passwordError; set { passwordError = value; OnPropertyChanged(); } }
        public string PasswordCopyError { get => passwordCopyError; set { passwordCopyError = value; OnPropertyChanged(); } }

        public string Answer1Error { get => answer1Error; set { answer1Error = value; OnPropertyChanged(); } }
        public string Answer2Error { get => answer2Error; set { answer2Error = value; OnPropertyChanged(); } }

        public Visibility Visibility { get => visibility; set { visibility = value; OnPropertyChanged(); } }

        public DelegateCommand ChangedPasswordCommand { get => changedPasswordCommand; set => changedPasswordCommand = value; }
        public DelegateCommand CancelCommand { get => cancelCommand; set => cancelCommand = value; }
    }
}
