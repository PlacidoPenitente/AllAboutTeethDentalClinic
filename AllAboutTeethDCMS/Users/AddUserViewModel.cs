using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AllAboutTeethDCMS.Users
{
    public class AddUserViewModel : CRUDPage<User>
    {
        private User user;
        private List<string> genders = new List<string>() { "Male", "Female" };
        private List<string> accountTypes = new List<string>() { "Administrator", "Staff" };
        private string error = "";
        private User copyUser;

        public AddUserViewModel()
        {
            user = new User();
        }

        public virtual void resetForm()
        {
            User = new User();
        }

        public virtual void saveUser()
        {
            User.AddedBy = ActiveUser;
            saveToDatabase(User, "allaboutteeth_"+ GetType().Namespace.Replace("AllAboutTeethDCMS.", ""));
        }

        public User User
        {
            get => user;
            set
            {
                user = value;
                OnPropertyChanged();
                foreach (PropertyInfo info in GetType().GetProperties())
                {
                    OnPropertyChanged(info.Name);
                }
            }
        }
        public string Username { get => User.Username; set { User.Username = value; Error = user.validate(); OnPropertyChanged(); } }
        public string Password { get => User.Password; set { User.Password = value; Error = user.validate(); OnPropertyChanged(); } }
        public string AccountType { get => User.Type; set { User.Type = value; Error = user.validate(); OnPropertyChanged(); } }
        public string FirstName { get => User.FirstName; set { User.FirstName = value; Error = user.validate(); OnPropertyChanged(); } }
        public string LastName { get => User.LastName; set { User.LastName = value; Error = user.validate(); OnPropertyChanged(); } }
        public string MiddleName { get => User.MiddleName; set { User.MiddleName = value; Error = user.validate(); OnPropertyChanged(); } }
        public DateTime Birthdate { get => User.Birthdate; set { User.Birthdate = value; Error = user.validate(); OnPropertyChanged(); } }
        public string Gender { get => User.Gender; set { User.Gender = value; Error = user.validate(); OnPropertyChanged(); } }
        public string Address { get => User.Address; set { User.Address = value; Error = user.validate(); OnPropertyChanged(); } }
        public string ContactNo { get => User.ContactNo; set { User.ContactNo = value; Error = user.validate(); OnPropertyChanged(); } }
        public string EmailAddress { get => User.EmailAddress; set { User.EmailAddress = value; Error = user.validate(); OnPropertyChanged(); } }
        public string Question1 { get => User.Question1; set { User.Question1 = value; Error = user.validate(); OnPropertyChanged(); } }
        public string Question2 { get => User.Question2; set { User.Question2 = value; Error = user.validate(); OnPropertyChanged(); } }
        public string Answer1 { get => User.Answer1; set { User.Answer1 = value; Error = user.validate(); OnPropertyChanged(); } }
        public string Answer2 { get => User.Answer2; set { User.Answer2 = value; Error = user.validate(); OnPropertyChanged(); } }

        public List<string> Genders { get => genders; set => genders = value; }
        public List<string> AccountTypes { get => accountTypes; set => accountTypes = value; }
        public string Error { get => error; set { error = value; OnPropertyChanged(); } }

        public User CopyUser { get => copyUser; set { copyUser = value; OnPropertyChanged(); } }
    }
}
