﻿using AllAboutTeethDCMS.ActivityLogs;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;

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
        private string questionsVisibility = "Collapsed";

        #endregion Fields

        #region Properties

        public User User
        {
            get => user;
            set
            {
                user = value;
                AccountTypeVisibility = "Collapsed";
                SensitiveFieldVisibility = "Collapsed";
                if (ActiveUser.Type.Equals("Administrator"))
                {
                    AccountTypeVisibility = "Visible";
                }
                if (value.No == ActiveUser.No)
                {
                    SensitiveFieldVisibility = "Visible";
                }
                foreach (PropertyInfo info in GetType().GetProperties()) OnPropertyChanged(info.Name);
            }
        }

        public User CopyUser { get => copyUser; set => copyUser = value; }

        public string PasswordCopy
        {
            get => passwordCopy;
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

        public List<string> Genders { get => genders; set => genders = value; }
        public List<string> AccountTypes { get => accountTypes; set => accountTypes = value; }

        #endregion Properties

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
        private DateTime dateEnd;
        private DateTime dateStart;

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
        public DateTime DateEnd { get => dateEnd; set => dateEnd = value; }
        public DateTime DateStart { get => dateStart; set => dateStart = value; }

        #endregion Validation Members

        #region User Properties

        public string Username
        {
            get => User.Username;
            set
            {
                if (!value.Contains(" "))
                {
                    bool valid = true;
                    if (!value.Equals(" "))
                    {
                        UsernameError = "";
                        UsernameError = ValidateUsername(value, CopyUser.Username);
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
                            if (value.Length < 21)
                            {
                                User.Username = value;
                            }
                        }
                    }
                    OnPropertyChanged();
                }
            }
        }

        public string Password
        {
            get => User.Password;
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
                            User.Password = "";
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
                                User.Password = value;
                            }
                        }
                    }

                    OnPropertyChanged("PasswordCopyError");
                    OnPropertyChanged();
                }
            }
        }

        public string Type { get => User.Type; set { User.Type = value; OnPropertyChanged(); } }

        public string FirstName
        {
            get => User.FirstName;
            set
            {
                if (!value.Contains("  ") && !value.StartsWith(" "))
                {
                    FirstNameError = "";
                    if (!Validate(value))
                    {
                        FirstNameError = "First Name is required.";
                    }
                    User.FirstName = value;
                    OnPropertyChanged();
                }
            }
        }

        public string MiddleName
        {
            get => User.MiddleName;
            set
            {
                if (!value.Contains("  ") && !value.StartsWith(" "))
                {
                    User.MiddleName = value;
                    OnPropertyChanged();
                }
            }
        }

        public string LastName
        {
            get => User.LastName;
            set
            {
                if (!value.Contains("  ") && !value.StartsWith(" "))
                {
                    LastNameError = "";
                    if (!Validate(value))
                    {
                        LastNameError = "Last Name is required.";
                    }
                    User.LastName = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Gender { get => User.Gender; set { User.Gender = value; OnPropertyChanged(); } }
        public DateTime Birthdate { get => User.Birthdate; set { User.Birthdate = value; OnPropertyChanged(); } }

        public string Address
        {
            get => User.Address;
            set
            {
                if (!value.Contains("  ") && !value.StartsWith(" "))
                {
                    AddressError = "";
                    if (!Validate(value))
                    {
                        AddressError = "Address is required.";
                    }
                    User.Address = value;
                    OnPropertyChanged();
                }
            }
        }

        public string ContactNo
        {
            get => User.ContactNo;
            set
            {
                if (ValidateNumberOnly(value))
                {
                    ContactNoError = "";
                    if (!Validate(value))
                    {
                        ContactNoError = "Contact No. is required.";
                        User.ContactNo = value;
                        OnPropertyChanged();
                    }
                    else if (value.Length < 12)
                    {
                        if (value.Length < 11)
                        {
                            ContactNoError = "Enter your 11-digit Mobile No.";
                        }
                        User.ContactNo = value;
                        OnPropertyChanged();
                    }
                }
            }
        }

        public string EmailAddress
        {
            get => User.EmailAddress;
            set
            {
                if (!value.Contains(" "))
                {
                    User.EmailAddress = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Question1
        {
            get => User.Question1;
            set
            {
                if (!value.Contains("  ") && !value.StartsWith(" "))
                {
                    Question1Error = "";
                    if (!Validate(value))
                    {
                        Question1Error = "Question No. 1 is required.";
                    }
                    User.Question1 = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Question2
        {
            get => User.Question2;
            set
            {
                if (!value.Contains("  ") && !value.StartsWith(" "))
                {
                    Question2Error = "";
                    if (!Validate(value))
                    {
                        Question2Error = "Quesrion No. 2 is required.";
                    }
                    User.Question2 = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Answer1
        {
            get => User.Answer1;
            set
            {
                if (!value.Contains("  ") && !value.StartsWith(" "))
                {
                    Answer1Error = "";
                    if (!Validate(value))
                    {
                        Answer1Error = "Answer for Question No. 1 is required.";
                    }
                    User.Answer1 = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Answer2
        {
            get => User.Answer2;
            set
            {
                if (!value.Contains("  ") && !value.StartsWith(" "))
                {
                    Answer2Error = "";
                    if (!Validate(value))
                    {
                        Answer2Error = "Answer for Question No. 2 is required.";
                    }
                    User.Answer2 = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Specialization
        {
            get => User.Specialization;
            set
            {
                if (!value.Contains("  ") && !value.StartsWith(" "))
                {
                    User.Specialization = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Image { get => User.Image; set { User.Image = value; OnPropertyChanged(); Console.WriteLine(value); } }

        #endregion User Properties

        public AddUserViewModel() : base()
        {
            user = new User();
            CopyUser = (User)user.Clone();
            if ((DateTime.Now.Year - 75) % 4 != 0 && DateTime.Now.Month == 2 && DateTime.Now.Day == 29)
            {
                DateStart = DateTime.Parse("3/1/" + (DateTime.Now.Year - 75));
            }
            else
            {
                DateStart = DateTime.Parse(DateTime.Now.Month + "/" + DateTime.Now.Day + "/" + (DateTime.Now.Year - 75));
            }
            if ((DateTime.Now.Year - 18) % 4 != 0 && DateTime.Now.Month == 2 && DateTime.Now.Day == 29)
            {
                DateEnd = DateTime.Parse("2/28/" + (DateTime.Now.Year - 18));
            }
            else
            {
                DateEnd = DateTime.Parse(DateTime.Now.Month + "/" + DateTime.Now.Day + "/" + (DateTime.Now.Year - 18));
            }

            StartCameraCommand = new DelegateCommand(new Action(startCamera));
            CaptureCommand = new DelegateCommand(new Action(capture));
            GoBackCommand = new DelegateCommand(new Action(goBack));
        }

        protected override bool beforeCreate()
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

        protected override void afterCreate(bool isSuccessful)
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
                AddActivityLogViewModel addActivityLog = new AddActivityLogViewModel();
                addActivityLog.ActivityLog = new ActivityLog();
                addActivityLog.ActiveUser = ActiveUser;
                addActivityLog.ActivityLog.Activity = "User created a new user account named " + Username + ".";
                addActivityLog.saveActivityLog();

                DialogBoxViewModel.Answer = "";
                User = new User();
                CopyUser = (User)User.Clone();
                PasswordCopy = "";
                PasswordError = "";
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

                AddActivityLogViewModel addActivityLog = new AddActivityLogViewModel();
                addActivityLog.ActivityLog = new ActivityLog();
                addActivityLog.ActiveUser = ActiveUser;
                addActivityLog.ActivityLog.Activity = "User updated account named " + Username + ".";
                addActivityLog.saveActivityLog();

                DialogBoxViewModel.Answer = "";
                CopyUser = (User)User.Clone();
                PasswordCopy = "";
                PasswordError = "";
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

        public virtual void saveUser()
        {
            foreach (PropertyInfo info in GetType().GetProperties())
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
            else
            {
                DialogBoxViewModel.Mode = "Error";
                DialogBoxViewModel.Title = "Save Failed";
                DialogBoxViewModel.Message = "Form contains errors. Please check all required fields.";
                DialogBoxViewModel.Answer = "None";
            }
        }

        #region Reset Thread

        private Thread resetThread;

        public virtual void startResetThread()
        {
            DialogBoxViewModel.Answer = "None";
            DialogBoxViewModel.Mode = "Confirm";
            DialogBoxViewModel.Title = "Reset Form";
            DialogBoxViewModel.Message = "Resetting form will restore previous values. Proceed?";

            while (DialogBoxViewModel.Answer.Equals("None"))
            {
                Thread.Sleep(100);
            }

            if (DialogBoxViewModel.Answer.Equals("OK"))
            {
                User = new User();
                foreach (PropertyInfo info in GetType().GetProperties())
                {
                    if (info.Name.EndsWith("Error"))
                    {
                        info.SetValue(this, "");
                    }
                }
                PasswordCopy = "";
                PasswordError = "";
            }
            DialogBoxViewModel.Answer = "";
        }

        public void resetForm()
        {
            resetThread = new Thread(startResetThread);
            resetThread.IsBackground = true;
            resetThread.Start();
        }

        #endregion Reset Thread

        private DelegateCommand startCameraCommand;
        private DelegateCommand captureCommand;
        private WebCam webCam;

        public WebCam WebCam { get => webCam; set => webCam = value; }
        public DelegateCommand StartCameraCommand { get => startCameraCommand; set => startCameraCommand = value; }
        public DelegateCommand CaptureCommand { get => captureCommand; set => captureCommand = value; }
        public DelegateCommand GoBackCommand { get => goBackCommand; set => goBackCommand = value; }
        public string AccountTypeVisibility { get => accountTypeVisibility; set { accountTypeVisibility = value; OnPropertyChanged(); } }

        public string SensitiveFieldVisibility { get => questionsVisibility; set { questionsVisibility = value; OnPropertyChanged(); } }

        private bool isWebcamStarted = false;

        public void startCamera()
        {
            if (WebCam == null)
            {
                WebCam = new WebCam();
                WebCam.InitializeWebCam(ref imageCamera);
            }
            WebCam.Continue();
            WebCam.Start();
            isWebcamStarted = true;
        }

        public void capture()
        {
            if (isWebcamStarted)
            {
                WebCam.Stop();
                isWebcamStarted = false;
                Image = convertToString(imageCamera.Source);
            }
        }

        private string accountTypeVisibility = "Collapsed";

        private DelegateCommand goBackCommand;

        public void goBack()
        {
            MenuViewModel.gotoUsers();
            if (isWebcamStarted)
            {
                WebCam.Stop();
                isWebcamStarted = false;
            }
        }

        #region Unimplemented Methods

        protected override void beforeLoad(MySqlCommand command)
        {
            throw new NotImplementedException();
        }

        protected override void afterLoad(List<User> list)
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

        #endregion Unimplemented Methods
    }
}