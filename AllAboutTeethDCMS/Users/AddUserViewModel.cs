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
        #region Fields
        private User user;
        private User copyUser;
        private string passwordCopy = "";
        private List<string> genders = new List<string>() { "Male", "Female" };
        private List<string> accountTypes = new List<string>() { "Administrator", "Dentist", "Staff" };
        #endregion

        #region Properties
        public User User { get => user; set  { user = value; foreach (PropertyInfo info in GetType().GetProperties()) OnPropertyChanged(info.Name); } }
        public User CopyUser { get => copyUser; set => copyUser = value; }
        public string PasswordCopy { get => passwordCopy; set => passwordCopy = value; }
        public List<string> Genders { get => genders; set => genders = value; }
        public List<string> AccountTypes { get => accountTypes; set => accountTypes = value; }
        #endregion

        #region Validation Members
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
        #endregion

        #region User Properties
        public string Username { get => User.Username; set { User.Username = value; OnPropertyChanged(); } }
        public string Password { get => User.Password; set { User.Password = value; OnPropertyChanged(); } }
        public string Type { get => User.Type; set { User.Type = value; OnPropertyChanged(); } }
        public string FirstName { get => User.FirstName; set { User.FirstName = value; OnPropertyChanged(); } }
        public string MiddleName { get => User.MiddleName; set { User.MiddleName = value; OnPropertyChanged(); } }
        public string LastName { get => User.LastName; set { User.LastName = value; OnPropertyChanged(); } }
        public string Gender { get => User.Gender; set { User.Gender = value; OnPropertyChanged(); } }
        public DateTime Birthdate { get => User.Birthdate; set { User.Birthdate = value; OnPropertyChanged(); } }
        public string Address { get => User.Address; set { User.Address = value; OnPropertyChanged(); } }
        public string ContactNo { get => User.ContactNo; set { User.ContactNo = value; OnPropertyChanged(); } }
        public string EmailAddress { get => User.EmailAddress; set { User.EmailAddress = value; OnPropertyChanged(); } }
        public string Question1 { get => User.Question1; set { User.Question1 = value; OnPropertyChanged(); } }
        public string Question2 { get => User.Question2; set { User.Question2 = value; OnPropertyChanged(); } }
        public string Answer1 { get => User.Answer1; set { User.Answer1 = value; OnPropertyChanged(); } }
        public string Answer2 { get => User.Answer2; set { User.Answer2 = value; OnPropertyChanged(); } }
        public string Specialization { get => User.Specialization; set { User.Specialization = value; OnPropertyChanged(); } }
        public int Rate { get => User.Rate; set { User.Rate = value; OnPropertyChanged(); } }
        public string Image { get => User.Image; set { User.Image = value; OnPropertyChanged(); } }
        #endregion

        public AddUserViewModel() : base()
        {
            user = new User();
            CopyUser = (User)user.Clone();
        }

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
    }
}
