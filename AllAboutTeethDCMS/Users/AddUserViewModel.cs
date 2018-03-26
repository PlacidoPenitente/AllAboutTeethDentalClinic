using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AllAboutTeethDCMS.Users
{
    public class AddUserViewModel : CRUDPage<User>
    {
        private User user;
        private List<string> genders = new List<string>() { "Male", "Female" };
        private List<string> accountTypes = new List<string>() { "Administrator", "Dentist", "Staff" };
        private User copyUser;

        private DialogBoxViewModel dialogBoxViewModel;
        public DialogBoxViewModel DialogBoxViewModel { get => dialogBoxViewModel; set { dialogBoxViewModel = value; OnPropertyChanged(); } }
        protected override bool beforeSave()
        {
            DialogBoxViewModel.Answer = "None";
            DialogBoxViewModel.Mode = "Question";
            DialogBoxViewModel.Title = "Add User";
            DialogBoxViewModel.Message = "Are you sure you want to add this user?";
            
            while (DialogBoxViewModel.Answer.Equals("None"))
            {
                Thread.Sleep(100);
            }

            if (DialogBoxViewModel.Answer.Equals("Yes"))
            {
                DialogBoxViewModel.Mode = "Progress";
                DialogBoxViewModel.Message = "Adding user. Please wait.";
                DialogBoxViewModel.Answer = "None";
                return true;
            }
            return false;
        }

        protected override void afterSave(bool isSuccessful)
        {
            if (isSuccessful)
            {
                DialogBoxViewModel.Mode = "Success";
                DialogBoxViewModel.Message = "Operation completed.";
                DialogBoxViewModel.Answer = "None";
                while (DialogBoxViewModel.Answer.Equals("None"))
                {
                    Thread.Sleep(100);
                }
                DialogBoxViewModel.Answer = "";
                User = new User();
            }
            else
            {
                DialogBoxViewModel.Mode = "Error";
                DialogBoxViewModel.Message = "Operation failed.";
                DialogBoxViewModel.Answer = "None";
                while (DialogBoxViewModel.Answer.Equals("None"))
                {
                    Thread.Sleep(100);
                }
                DialogBoxViewModel.Answer = "";
            }
        }

        protected override bool beforeUpdate()
        {
            DialogBoxViewModel.Answer = "None";
            DialogBoxViewModel.Mode = "Question";
            DialogBoxViewModel.Title = "Update User";
            DialogBoxViewModel.Message = "Are you sure you want to update this user?";

            while (DialogBoxViewModel.Answer.Equals("None"))
            {
                Thread.Sleep(100);
            }

            if (DialogBoxViewModel.Answer.Equals("Yes"))
            {
                DialogBoxViewModel.Mode = "Progress";
                DialogBoxViewModel.Message = "Updating user. Please wait.";
                DialogBoxViewModel.Answer = "None";
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
                while (DialogBoxViewModel.Answer.Equals("None"))
                {
                    Thread.Sleep(100);
                }
                DialogBoxViewModel.Answer = "";
                CopyUser = (User)User.Clone();
            }
            else
            {
                DialogBoxViewModel.Mode = "Error";
                DialogBoxViewModel.Message = "Operation failed.";
                DialogBoxViewModel.Answer = "None";
                while (DialogBoxViewModel.Answer.Equals("None"))
                {
                    Thread.Sleep(100);
                }
                DialogBoxViewModel.Answer = "";
            }
        }

        private Thread resetThread;

        public virtual void startResetThread()
        {
            DialogBoxViewModel.Answer = "None";
            DialogBoxViewModel.Mode = "Question";
            DialogBoxViewModel.Title = "Reset Form";
            DialogBoxViewModel.Message = "Are you sure you want to reset this form?";

            while (DialogBoxViewModel.Answer.Equals("None"))
            {
                Thread.Sleep(100);
            }

            if (DialogBoxViewModel.Answer.Equals("Yes"))
            {
                User = new User();
                foreach (PropertyInfo info in GetType().GetProperties())
                {
                    if (info.Name.EndsWith("Error"))
                    {
                        info.SetValue(this, "");
                    }
                }
            }
            DialogBoxViewModel.Answer = "";
        }

        public void resetForm()
        {
            resetThread = new Thread(startResetThread);
            resetThread.IsBackground = true;
            resetThread.Start();
        }

        public AddUserViewModel()
        {
            user = new User();
            copyUser = (User)user.Clone();
            DialogBoxViewModel = new DialogBoxViewModel();
        }

        public virtual void saveUser()
        {
            foreach(PropertyInfo info in GetType().GetProperties())
            {
                info.SetValue(this, info.GetValue(this));
            }
            bool hasError = false;
            foreach (PropertyInfo info in GetType().GetProperties())
            {
                if (info.Name.EndsWith("Error"))
                {
                    if (!((string)info.GetValue(this)).Equals(""))
                    {
                        hasError = true;
                        break;
                    }
                }
            }
            if (!hasError)
            {
                startSaveToDatabase(User, "allaboutteeth_" + GetType().Namespace.Replace("AllAboutTeethDCMS.", ""));
            }
        }

        protected override void setLoaded(List<User> list)
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
        public string Username { get => User.Username;
            set {
                bool valid = true;
                UsernameError = "";
                if (String.IsNullOrEmpty(value))
                {
                    valid = false;
                    User.Username = "";
                    UsernameError = "Username is required.";
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
                    UsernameError = "";
                    UsernameError = validateUsername(value, CopyUser.Username);
                    if(value.Length<61)
                    {
                        User.Username = value;
                    }
                    if (value.Length < 5)
                    {
                        UsernameError = "Must be atleast 5 characters.";
                    }
                }
                OnPropertyChanged();
            } }
        public string Password { get => User.Password; set { User.Password = value; PasswordError = ""; PasswordError = validatePassword(value); PasswordCopy = PasswordCopy; OnPropertyChanged(); } }
        public string AccountType { get => User.Type; set { User.Type = value;  OnPropertyChanged(); } }
        public string FirstName { get => User.FirstName; set { User.FirstName = value; FirstNameError = ""; FirstNameError = validate(value); OnPropertyChanged(); } }
        public string LastName { get => User.LastName; set { User.LastName = value; LastNameError = ""; LastNameError = validate(value); OnPropertyChanged(); } }
        public string MiddleName { get => User.MiddleName; set { User.MiddleName = value; OnPropertyChanged(); } }
        public DateTime Birthdate { get => User.Birthdate; set { User.Birthdate = value;  OnPropertyChanged(); } }
        public string Gender { get => User.Gender; set { User.Gender = value;  OnPropertyChanged(); } }
        public string Address { get => User.Address; set { User.Address = value;  OnPropertyChanged(); AddressError = ""; AddressError = validate(value); } }
        public string ContactNo { get => User.ContactNo; set {
                bool valid = true;
                ContactNoError = "";
                if (String.IsNullOrEmpty(value))
                {
                    valid = false;
                    User.ContactNo = "";
                    ContactNoError = "Contact No. is required.";
                }
                foreach (char c in value.ToArray())
                {
                    if (!Char.IsDigit(c))
                    {
                        valid = false;
                        break;
                    }
                }
                if (valid)
                {
                    if (value.Length < 12)
                    {
                        User.ContactNo = value;
                    }
                    if(value.Length<11)
                    {
                        ContactNoError = "Must be an 11-digit number.";
                    }
                }
                OnPropertyChanged(); } }
        public string EmailAddress { get => User.EmailAddress; set { User.EmailAddress = value;  OnPropertyChanged(); } }
        public string Question1 { get => User.Question1; set { User.Question1 = value; Question1Error = ""; Question1Error = validate(value); OnPropertyChanged(); } }
        public string Question2 { get => User.Question2; set { User.Question2 = value; Question2Error = ""; Question2Error = validate(value); OnPropertyChanged(); } }
        public string Answer1 { get => User.Answer1; set { User.Answer1 = value; Answer1Error = ""; Answer1Error = validate(value); OnPropertyChanged(); } }
        public string Answer2 { get => User.Answer2; set { User.Answer2 = value; Answer2Error = ""; Answer2Error = validate(value); OnPropertyChanged(); } }

        public List<string> Genders { get => genders; set => genders = value; }
        public List<string> AccountTypes { get => accountTypes; set => accountTypes = value; }
        public User CopyUser { get => copyUser; set { copyUser = value; OnPropertyChanged(); } }

        public string UsernameError { get => usernameError; set { usernameError = value; OnPropertyChanged(); } }
        public string PasswordError { get => passwordError; set { passwordError = value; OnPropertyChanged(); } }
        public string PasswordCopyError { get => passwordCopyError; set { passwordCopyError = value; OnPropertyChanged(); } }
        public string FirstNameError { get => firstNameError; set { firstNameError = value; OnPropertyChanged(); } }
        public string LastNameError { get => lastNameError; set { lastNameError = value; OnPropertyChanged(); } }
        public string AddressError { get => addressError; set { addressError = value; OnPropertyChanged(); } }
        public string ContactNoError { get => contactNoError; set { contactNoError = value; OnPropertyChanged(); } }
        public string Question1Error { get => question1Error; set { question1Error = value; OnPropertyChanged(); } }
        public string Question2Error { get => question2Error; set { question2Error = value; OnPropertyChanged(); } }
        public string Answer1Error { get => answer1Error; set { answer1Error = value; OnPropertyChanged(); } }
        public string Answer2Error { get => answer2Error; set { answer2Error = value; OnPropertyChanged(); } }

        public string PasswordCopy { get => passwordCopy; set { passwordCopy = value;
                PasswordCopyError = "";
                if (!Password.Equals(PasswordCopy)) {
                    PasswordCopyError = "Passwords do not match.";
                }
                OnPropertyChanged(); } }

        private string usernameError = "";
        private string passwordError = "";
        private string passwordCopyError = "";
        private string firstNameError = "";
        private string lastNameError = "";
        private string addressError = "";
        private string contactNoError = "";
        private string question1Error = "";
        private string question2Error = "";
        private string answer1Error = "";
        private string answer2Error = "";
        private string passwordCopy = "";
    }
}
